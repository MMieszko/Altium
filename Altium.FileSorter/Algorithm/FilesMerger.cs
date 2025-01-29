using System.Diagnostics;
using System.IO;
using System.Text;
using CSharpFunctionalExtensions;

namespace Altium.FileSorter.Algorithm;

public abstract class FilesMerger<T>
    where T : IComparable<T>
{
    private readonly FileManager _fileManager;
    private readonly Logger _logger;

    protected FilesMerger(FileManager fileManager, Logger logger)
    {
        _fileManager = fileManager;
        _logger = logger;
    }

    public Result<string> MergeChunks(List<string> chunks)
    {
        var readers = new List<StreamReader>();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            readers = chunks.Select(chunk => new StreamReader(chunk, Encoding.UTF8)).ToList();

            _logger.LogInformation($"Starting merge process with {chunks.Count} chunks");

            var queue = new PriorityQueue<StreamReader, T>(readers.Count);

            foreach (var reader in readers)
            {
                var line = ReadLine(reader.ReadLine());

                if (line.IsFailure)
                    return line.Error;

                queue.Enqueue(reader, line.Value);
            }

            using var writer = new StreamWriter(_fileManager.SortedFilePath, false, Encoding.UTF8);

            while (queue.Count > 0)
            {
                if (!queue.TryDequeue(out var currentReader, out var currentFileLine))
                    continue;

                writer.WriteLine(currentFileLine);

                if (currentReader.EndOfStream) { currentReader.Close(); continue; }

                var nextLine = ReadLine(currentReader.ReadLine());

                if (nextLine.IsFailure)
                    return nextLine.Error;

                queue.Enqueue(currentReader, nextLine.Value);
            }

            _fileManager.CleanTempFile();
            _logger.LogInformation($"Finished merging process within {stopwatch.Elapsed:mm\\:ss\\.fff}");

            return _fileManager.SortedFilePath;
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(ex.ToString());
        }
        finally
        {
            stopwatch.Stop();
            readers.ForEach(x => x.Dispose());
        }
    }

    protected abstract Result<T> ReadLine(string? line);
}