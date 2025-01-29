namespace Altium.FileSorter;

public class Logger
{
    public event EventHandler<LogMessage>? LogAdded;

    public void Log(LogLevel level, string value)
    {
        LogAdded?.Invoke(this, new LogMessage(level, value, DateTime.Now));
    }

    public void LogInformation(string value) => Log(LogLevel.Information, value);
    public void LogError(string value) => Log(LogLevel.Error, value);
    public void LogWarning(string value) => Log(LogLevel.Warning, value);
    public void LogSuccess(string value) => Log(LogLevel.Success, value);
}

public enum LogLevel
{
    None = 0,
    Success = 1,
    Information = 10,
    Warning = 20,
    Error = 30
}

public record LogMessage(LogLevel Level, string Value, DateTime DateTime);
