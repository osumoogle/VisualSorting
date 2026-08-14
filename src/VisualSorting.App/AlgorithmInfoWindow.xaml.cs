using System.Windows;
using VisualSorting.Core;

namespace VisualSorting.App;

public partial class AlgorithmInfoWindow : Window
{
    public AlgorithmInfoWindow(ISortAlgorithm algorithm, int currentArraySize)
    {
        InitializeComponent();
        Title = $"How it works — {algorithm.Name}";
        TitleText.Text = algorithm.Name;
        ComplexityText.Text =
            $"Time: best {algorithm.TimeBest}, average {algorithm.TimeAverage}, worst {algorithm.TimeWorst}   ·   Space: {algorithm.SpaceComplexity} auxiliary";
        MemoryText.Text =
            $"At the current size (n = {currentArraySize}) that is roughly {FormatBytes(algorithm.AuxiliaryBytes(currentArraySize))} of working memory, plus the {FormatBytes(4L * currentArraySize)} input array.";
        DescriptionText.Text = algorithm.Description;
    }

    private static string FormatBytes(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:0.#} KB",
        _ => $"{bytes / (1024.0 * 1024.0):0.#} MB",
    };

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
