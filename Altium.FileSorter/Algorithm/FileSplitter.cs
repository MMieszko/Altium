using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using CSharpFunctionalExtensions;

namespace Altium.FileSorter.Algorithm;

public abstract class FileSplitter<T>
    where T : IComparable<T>
{
    private readonly FileManager _fileManager;
    private readonly Logger _logger;
    private readonly int _maxLinesPerFile;

    protected FileSplitter(FileManager fileManager, Logger logger, int maxLinesPerFile = 70000)
    {
        _fileManager = fileManager;
        _logger = logger;
        _maxLinesPerFile = maxLinesPerFile;
    }

    public Task<Result<string[]>> SplitAndSortSampleFile()
    {
        try
        {
            return Impl();
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Failure<string[]>(ex.ToString()));
        }
    }

    private async Task<Result<string[]>> Impl()
    {
        if (!File.Exists(_fileManager.SampleFilePath))
            return Result.Failure<string[]>("No sample file found");

        _logger.LogInformation("Starting split and sorting files");

        var stopwatch = Stopwatch.StartNew();
        var outputFileFormat = Path.Combine(_fileManager.TempFilePath, "chunk-{0}.txt");
        var sortTasks = new ConcurrentBag<Task<Result>>();
        var iteration = 0;
        var currentLines = 0;
        StreamWriter? outfile = null;

        using var inputFileReader = new StreamReader(_fileManager.SampleFilePath);

        while (!inputFileReader.EndOfStream)
        {
            var line = inputFileReader.ReadLine();

            if (currentLines < _maxLinesPerFile)
            {
                outfile ??= new StreamWriter(string.Format(outputFileFormat, iteration), false, inputFileReader.CurrentEncoding);
                outfile.WriteLine(line);
                currentLines++;
            }
            else
            {

                if (outfile == null)
                    continue;

                outfile.WriteLine(line);
                outfile.Dispose();
                outfile = null;
                currentLines = 0;

                sortTasks.Add(CreateSortingTask(string.Format(outputFileFormat, iteration++)));
            }
        }

        if (outfile != null)
        {
            outfile.Dispose();
            sortTasks.Add(CreateSortingTask(string.Format(outputFileFormat, iteration)));
        }

        inputFileReader.Dispose();

        _logger.LogInformation($"Finished file split within {stopwatch.Elapsed:mm\\:ss\\.fff}");

        var sortResults = await Task.WhenAll(sortTasks);

        if (sortResults.Any(x => x.IsFailure))
            return Result.Failure<string[]>("Failed to sort file");

        _logger.LogInformation($"Finished split files sort within {stopwatch.Elapsed:mm\\:ss\\.fff}");

        return Directory.GetFiles(_fileManager.TempFilePath);
    }

    private Task<Result> CreateSortingTask(string file) => Task.Run(() =>
    {
        try
        {
            using var reader = new StreamReader(file);
            using var writer = new StreamWriter(file.Remove(file.Length - 3, 3) + "sorted.txt");
            var lines = new List<T>();

            while (reader.ReadLine() is { } line)
            {
                var fileLine = ReadLine(line);

                if (fileLine.IsFailure)
                    return fileLine;

                lines.Add(fileLine.Value);
            }

            lines.Sort(GetComparer());

            foreach (var sortedLine in lines)
            {
                writer.WriteLine(sortedLine);
            }

            reader.Close();
            reader.Dispose();

            _fileManager.RemoveFile(file);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.ToString());
        }
    });

    protected abstract Result<T> ReadLine(string? line);
    protected abstract IComparer<T> GetComparer();
}