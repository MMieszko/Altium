using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Altium.FileSorter.Algorithm;
using Altium.FileSorter.Mvvm;
using AsyncAwaitBestPractices.MVVM;
using CSharpFunctionalExtensions;

namespace Altium.FileSorter.Windows;
public class HomeViewModel : ViewModel
{
    public long MaximumSampleFileSize => Constants.Gigabyte * 100;

    public ObservableCollection<LogMessage> Logs
    {
        get => GetPropertyValue<ObservableCollection<LogMessage>>();
        set => SetPropertyValue(value);
    }

    public bool IsProcessing
    {
        get => GetPropertyValue<bool>();
        set => SetPropertyValue(value);
    }

    public FileInfo? SampleFile
    {
        get => GetPropertyValue<FileInfo?>();
        set => SetPropertyValue(value);
    }

    public FileInfo? SortedFile
    {
        get => GetPropertyValue<FileInfo?>();
        set => SetPropertyValue(value);
    }

    public long SampleFileGenerationSize
    {
        get => GetPropertyValue<long>();
        set => SetPropertyValue(value);
    }

    public ICommand GenerateFileCommand { get; }
    public ICommand SortCommand { get; }
    public ICommand ClearLogsCommand { get; }

    private readonly FileManager _fileManager;
    private readonly DataGenerator _dataGenerator;
    private readonly FileLineFilesMerger _filesMerger;
    private readonly FileLineSplitter _fileSplitter;
    private readonly Logger _logger;

    public HomeViewModel(FileManager fileManager, DataGenerator dataGenerator, Logger logger, FileLineFilesMerger filesMerger, FileLineSplitter fileSplitter)
    {
        Logs = new ObservableCollection<LogMessage>();

        _fileManager = fileManager;
        _dataGenerator = dataGenerator;
        _logger = logger;
        _filesMerger = filesMerger;
        _fileSplitter = fileSplitter;
        _logger.LogAdded += (_, e) => AddLog(e);

        GenerateFileCommand = new AsyncCommand(OnGenerateFile, _ => !IsProcessing);
        SortCommand = new AsyncCommand(OnSortFile, _ => !IsProcessing);
        ClearLogsCommand = new RelayCommand(_ => true, _ => this.Logs.Clear());
    }

    public void Initialize()
    {
        this.SampleFile = _fileManager.GetSampleFile();
        this.SampleFileGenerationSize = Constants.Gigabyte;
        this.SortedFile = _fileManager.GetSortedFile();
        this.IsProcessing = false;

        _logger.LogInformation($"Main window loaded. Program is running. {Guid.NewGuid()}");
    }

    private void AddLog(LogMessage value) => Application.Current.Dispatcher.Invoke(() => Logs.Add(value));

    private async Task OnSortFile()
    {
        if (this.SampleFile == null)
        {
            _logger.LogWarning("Attempt to sort but no sample file exist");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        IsProcessing = true;

        await Task.Run(() => _fileSplitter.SplitAndSortSampleFile().Bind(chunks => _filesMerger.MergeChunks(chunks.ToList())))
            .Tap(_ =>
            {
                this.SortedFile = _fileManager.GetSortedFile();
                var speed = Math.Round(this.SampleFile.GetFileSizeInGb() / stopwatch.Elapsed.TotalMinutes, 2);
                _logger.LogSuccess($"Total time: {stopwatch.Elapsed:mm\\:ss\\.fff}. Speed: {speed} GB/min");
            })
            .TapError(error => AddLog(new LogMessage(LogLevel.Error, error, DateTime.Now)))
            .ConfigureAwait(false);

        stopwatch.Stop();
        IsProcessing = false;
    }

    private async Task OnGenerateFile()
    {
        IsProcessing = true;
        var stopwatch = Stopwatch.StartNew();

        await Task.Run(() => _dataGenerator.Generate(SampleFileGenerationSize))
            .Tap(_ =>
            {
                this.SampleFile = _fileManager.GetSampleFile();
                this._logger.LogSuccess($"Generation file time: {stopwatch.Elapsed:mm\\:ss\\.fff}");
            })
            .TapError(error => _logger.LogError(error))
            .ConfigureAwait(false);

        stopwatch.Stop();
        IsProcessing = false;
    }
}