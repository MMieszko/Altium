using System.IO;

namespace Altium.FileSorter;

public class FileManager
{
    private readonly string _workingDirectory;
    private const string SampleFileName = "sample.txt";
    private const string SortedFileName = "sorted.txt";

    public string TempFilePath => Path.Combine(_workingDirectory, "tmp");
    public string SortedFilePath => Path.Combine(_workingDirectory, SortedFileName);
    public string SampleFilePath => Path.Combine(_workingDirectory, SampleFileName);

    public FileManager(string workingDirectory)
    {
        _workingDirectory = workingDirectory;

        Directory.CreateDirectory(TempFilePath);
    }

    public void CleanTempFile() => Directory.GetFiles(TempFilePath).ToList().ForEach(RemoveFile);

    public void RemoveFile(string fileName) => File.Delete(fileName);

    public bool RemoveSampleFile()
    {
        if (!File.Exists(SampleFilePath)) return false;

        File.Delete(SampleFilePath);
        return true;
    }

    public FileInfo? GetSortedFile() => File.Exists(SortedFilePath) ? new FileInfo(SortedFilePath) : null;

    public FileInfo? GetSampleFile() => File.Exists(SampleFilePath) ? new FileInfo(SampleFilePath) : null;
}