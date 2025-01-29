using System.IO;

namespace Altium.FileSorter;

public static class Extensions
{
    public static double GetFileSizeInGb(this FileInfo @this) => @this.Length / (double)Constants.Gigabyte;
}