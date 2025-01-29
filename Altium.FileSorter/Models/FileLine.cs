using CSharpFunctionalExtensions;

namespace Altium.FileSorter.Models;

public readonly struct FileLine : IComparable<FileLine>
{
    public ulong Number { get; }
    public string Text { get; }

    public static implicit operator string(FileLine line) => line.ToString();

    public FileLine(ulong number, string text)
    {
        this.Number = number;
        this.Text = text;
    }

    public static Maybe<FileLine> Create(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            return Maybe<FileLine>.None;

        var dotIndex = raw.IndexOf('.');

        if (dotIndex == -1 || !ulong.TryParse(raw.AsSpan(0, dotIndex), out var number))
            return Maybe<FileLine>.None;

        var text = raw[(dotIndex + 2)..];

        return new FileLine(number, text);
    }
    public override string ToString() => $"{Number}. {Text}";
    public int CompareTo(FileLine other) => FileLineComparer.Instance.Compare(this, other);
}

