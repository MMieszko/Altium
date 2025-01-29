using System.Diagnostics;
using System.IO;
using System.Text;
using Altium.FileSorter.Models;
using CSharpFunctionalExtensions;

namespace Altium.FileSorter.Algorithm;

public class DataGenerator
{
    private static readonly Random Random = new();
    private readonly FileManager _fileManager;
    private readonly Logger _logger;

    public DataGenerator(FileManager fileManager, Logger logger)
    {
        _fileManager = fileManager;
        _logger = logger;
    }

    public Result<string> Generate(long size)
    {
        var stopwatch = Stopwatch.StartNew();

        const int newLineSize = 2;

        _logger.LogInformation($"Generating sample file with requested size: {size} bytes");
        _fileManager.RemoveSampleFile();

        long currentSize = 0;

        using (var writer = new StreamWriter(_fileManager.SampleFilePath, false, Encoding.UTF8))
        {
            while (currentSize < size)
            {
                try
                {
                    var number = RandomNumber();
                    var text = RandomString(Random.Next(1500, 2000));
                    var line = new FileLine(number, text);

                    writer.WriteLine(line);

                    currentSize += Encoding.UTF8.GetByteCount(line.ToString()) + newLineSize;
                }
                catch (Exception ex)
                {
                    return Result.Failure<string>(ex.ToString());
                }
            }

            writer.Flush();
        }

        stopwatch.Stop();

        _logger.LogInformation($"Finished generating file. {stopwatch.Elapsed.TotalSeconds} seconds");

        return _fileManager.SampleFilePath;
    }

    private static uint RandomNumber() => (uint)Random.Next(0, int.MaxValue);

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}