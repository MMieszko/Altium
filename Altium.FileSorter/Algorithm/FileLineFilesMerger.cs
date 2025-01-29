using Altium.FileSorter.Models;
using CSharpFunctionalExtensions;

namespace Altium.FileSorter.Algorithm;

public class FileLineFilesMerger : FilesMerger<FileLine>
{
    public FileLineFilesMerger(FileManager fileManager, Logger logger)
        : base(fileManager, logger)
    {
    }

    protected override Result<FileLine> ReadLine(string? line)
        => FileLine.Create(line).ToResult($"Invalid file line format: {line}");
}