using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VisualSorting.Core;

namespace VisualSorting.App;

public partial class MainWindow : Window
{
    private Brush _defaultBrush = Brushes.Transparent;
    private Brush _compareBrush = Brushes.Transparent;
    private Brush _swapBrush = Brushes.Transparent;
    private Brush _overwriteBrush = Brushes.Transparent;
    private Brush _sortedBrush = Brushes.Transparent;

    private readonly Random _random = new();
    private int[] _values = Array.Empty<int>();
    private Rectangle[] _bars = Array.Empty<Rectangle>();
    private readonly HashSet<int> _sortedIndices = new();
    private readonly List<int> _highlighted = new();

    private CancellationTokenSource? _cts;
    private bool _isRunning;
    private long _comparisons;
    private long _writes;

    public MainWindow()
    {
        InitializeComponent();
        AlgorithmCombo.ItemsSource = AlgorithmRegistry.CreateAll();
        AlgorithmCombo.SelectedIndex = 0;
        LoadThemeBrushes();
        GenerateData((int)SizeSlider.Value);
        UpdateAlgorithmInfo();
        Loaded += (_, _) => RebuildBars();
    }

    // ---- Theming ----------------------------------------------------------

    private void LoadThemeBrushes()
    {
        _defaultBrush = (Brush)FindResource("BarDefaultBrush");
        _compareBrush = (Brush)FindResource("BarCompareBrush");
        _swapBrush = (Brush)FindResource("BarSwapBrush");
        _overwriteBrush = (Brush)FindResource("BarOverwriteBrush");
        _sortedBrush = (Brush)FindResource("BarSortedBrush");
    }

    private void ApplyTheme(string themeName)
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        dictionaries.Clear();
        dictionaries.Add(new ResourceDictionary
        {
            Source = new Uri($"Themes/{themeName}Theme.xaml", UriKind.Relative),
        });
        LoadThemeBrushes();
        RepaintBars();
    }

    private void RepaintBars()
    {
        // Transient compare/swap highlights are restored by the next playback frame.
        for (int i = 0; i < _bars.Length; i++)
            _bars[i].Fill = _sortedIndices.Contains(i) ? _sortedBrush : _defaultBrush;
    }

    // ---- Data + rendering -------------------------------------------------

    private void GenerateData(int size)
    {
        // Distinct values 1..size give every bar a unique height.
        _values = Enumerable.Range(1, size).ToArray();
        for (int i = _values.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_values[i], _values[j]) = (_values[j], _values[i]);
        }
        _sortedIndices.Clear();
        _highlighted.Clear();
        _comparisons = 0;
        _writes = 0;
        UpdateStats(0, 0);
        StatusText.Text = "Ready";
    }

    private void RebuildBars()
    {
        BarCanvas.Children.Clear();
        _bars = new Rectangle[_values.Length];
        for (int i = 0; i < _values.Length; i++)
        {
            var bar = new Rectangle { Fill = _defaultBrush };
            _bars[i] = bar;
            BarCanvas.Children.Add(bar);
        }
        LayoutBars();
    }

    private void LayoutBars()
    {
        double width = BarCanvas.ActualWidth;
        double height = BarCanvas.ActualHeight;
        if (width <= 0 || height <= 0 || _bars.Length == 0)
            return;

        int n = _values.Length;
        double slot = width / n;
        double gap = slot > 3 ? 1 : 0;
        for (int i = 0; i < n; i++)
        {
            var bar = _bars[i];
            bar.Width = Math.Max(1, slot - gap);
            bar.Height = Math.Max(1, height * _values[i] / n);
            Canvas.SetLeft(bar, i * slot);
            Canvas.SetTop(bar, height - bar.Height);
        }
    }

    private void UpdateBar(int index)
    {
        double height = BarCanvas.ActualHeight;
        if (height <= 0)
            return;
        var bar = _bars[index];
        bar.Height = Math.Max(1, height * _values[index] / _values.Length);
        Canvas.SetTop(bar, height - bar.Height);
    }

    private void Highlight(int index, Brush brush)
    {
        _bars[index].Fill = brush;
        _highlighted.Add(index);
    }

    private void ClearHighlights()
    {
        foreach (int index in _highlighted)
            _bars[index].Fill = _sortedIndices.Contains(index) ? _sortedBrush : _defaultBrush;
        _highlighted.Clear();
    }

    // ---- Playback ---------------------------------------------------------

    private async Task RunSortAsync(ISortAlgorithm algorithm, double durationSeconds, CancellationToken token)
    {
        // Reset visuals to the current (unsorted) data.
        _sortedIndices.Clear();
        ClearHighlights();
        foreach (var bar in _bars)
            bar.Fill = _defaultBrush;
        _comparisons = 0;
        _writes = 0;

        // Generate the full step stream up front on a copy, then replay it
        // against the on-screen array at a pace that fits the chosen duration.
        var working = (int[])_values.Clone();
        var steps = await Task.Run(() => algorithm.Sort(working).ToList(), token);

        double perStepMs = durationSeconds * 1000.0 / Math.Max(1, steps.Count);
        const double minFrameMs = 10; // below this, apply steps in batches per frame
        int batchSize = perStepMs >= minFrameMs ? 1 : (int)Math.Ceiling(minFrameMs / perStepMs);
        double frameDelayMs = perStepMs * batchSize;

        StatusText.Text = $"Sorting: {algorithm.Name}";

        int applied = 0;
        while (applied < steps.Count)
        {
            token.ThrowIfCancellationRequested();
            ClearHighlights();

            int end = Math.Min(applied + batchSize, steps.Count);
            for (; applied < end; applied++)
                ApplyStepVisual(steps[applied]);

            UpdateStats(applied, steps.Count);
            await Task.Delay(TimeSpan.FromMilliseconds(frameDelayMs), token);
        }

        ClearHighlights();
        await FinalSweepAsync(token);
        StatusText.Text = $"Done: {algorithm.Name}";
    }

    private void ApplyStepVisual(in SortStep step)
    {
        switch (step.Type)
        {
            case SortStepType.Compare:
                _comparisons++;
                Highlight(step.IndexA, _compareBrush);
                Highlight(step.IndexB, _compareBrush);
                break;

            case SortStepType.Swap:
                _writes++;
                (_values[step.IndexA], _values[step.IndexB]) = (_values[step.IndexB], _values[step.IndexA]);
                UpdateBar(step.IndexA);
                UpdateBar(step.IndexB);
                Highlight(step.IndexA, _swapBrush);
                Highlight(step.IndexB, _swapBrush);
                break;

            case SortStepType.Overwrite:
                _writes++;
                _values[step.IndexA] = step.Value;
                UpdateBar(step.IndexA);
                Highlight(step.IndexA, _overwriteBrush);
                break;

            case SortStepType.MarkSorted:
                _sortedIndices.Add(step.IndexA);
                _bars[step.IndexA].Fill = _sortedBrush;
                break;
        }
    }

    private async Task FinalSweepAsync(CancellationToken token)
    {
        int n = _bars.Length;
        int batch = Math.Max(1, n / 60);
        for (int i = 0; i < n; i += batch)
        {
            for (int j = i; j < Math.Min(i + batch, n); j++)
            {
                _sortedIndices.Add(j);
                _bars[j].Fill = _sortedBrush;
            }
            await Task.Delay(10, token);
        }
    }

    private void UpdateStats(int step, int totalSteps)
    {
        ComparisonsText.Text = $"Comparisons: {_comparisons:N0}";
        WritesText.Text = $"Swaps / writes: {_writes:N0}";
        ProgressText.Text = totalSteps > 0 ? $"Step {step:N0} of {totalSteps:N0}" : "";
    }

    private void UpdateAlgorithmInfo()
    {
        if (AlgorithmCombo.SelectedItem is not ISortAlgorithm algorithm)
            return;
        int n = _values.Length;
        TimeBestText.Text = $"best {algorithm.TimeBest}";
        TimeAvgText.Text = $"average {algorithm.TimeAverage}";
        TimeWorstText.Text = $"worst {algorithm.TimeWorst}";
        SpaceText.Text = $"{algorithm.SpaceComplexity} auxiliary";
        AuxMemoryText.Text = $"≈ {FormatBytes(algorithm.AuxiliaryBytes(n))} at n = {n} (plus the {FormatBytes(4L * n)} input array)";
    }

    private static string FormatBytes(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:0.#} KB",
        _ => $"{bytes / (1024.0 * 1024.0):0.#} MB",
    };

    private void SetRunning(bool running)
    {
        _isRunning = running;
        RunButton.IsEnabled = !running;
        StopButton.IsEnabled = running;
        ShuffleButton.IsEnabled = !running;
        AlgorithmCombo.IsEnabled = !running;
        SizeSlider.IsEnabled = !running;
        DurationSlider.IsEnabled = !running;
    }

    // ---- Event handlers ---------------------------------------------------

    private async void RunButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isRunning || AlgorithmCombo.SelectedItem is not ISortAlgorithm algorithm)
            return;

        // If the previous run finished, start from a fresh shuffle.
        if (_sortedIndices.Count == _values.Length && _values.Length > 0)
        {
            GenerateData(_values.Length);
            RebuildBars();
        }

        _cts = new CancellationTokenSource();
        SetRunning(true);
        try
        {
            await RunSortAsync(algorithm, DurationSlider.Value, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            StatusText.Text = "Stopped";
        }
        finally
        {
            SetRunning(false);
            _cts.Dispose();
            _cts = null;
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e) => _cts?.Cancel();

    private void ShuffleButton_Click(object sender, RoutedEventArgs e)
    {
        GenerateData(_values.Length);
        RebuildBars();
    }

    private void AlgorithmCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateAlgorithmInfo();

    private void HowItWorksButton_Click(object sender, RoutedEventArgs e)
    {
        if (AlgorithmCombo.SelectedItem is not ISortAlgorithm algorithm)
            return;
        // Non-modal so the description can be read while the sort animates.
        new AlgorithmInfoWindow(algorithm, _values.Length) { Owner = this }.Show();
    }

    private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
            return; // fires during InitializeComponent for the default selection
        ApplyTheme(ThemeCombo.SelectedIndex == 1 ? "Light" : "Dark");
    }

    private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsLoaded || _isRunning)
            return;
        SizeLabel.Text = $"Elements: {(int)e.NewValue}";
        GenerateData((int)e.NewValue);
        RebuildBars();
        UpdateAlgorithmInfo();
    }

    private void DurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (DurationLabel != null)
            DurationLabel.Text = $"Duration: {(int)e.NewValue} s";
    }

    private void BarCanvas_SizeChanged(object sender, SizeChangedEventArgs e) => LayoutBars();
}
