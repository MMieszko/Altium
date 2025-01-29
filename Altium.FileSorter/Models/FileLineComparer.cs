namespace Altium.FileSorter.Models;

public class FileLineComparer : IComparer<FileLine>
{
    public static FileLineComparer Instance { get; } = new();
    private FileLineComparer() { }

    public int Compare(FileLine line1, FileLine line2)
    {
        var stringComparison = string.Compare(line1.Text, line2.Text, StringComparison.Ordinal);
        return stringComparison != 0
            ? stringComparison
            : line1.Number.CompareTo(line2.Number);
    }
}