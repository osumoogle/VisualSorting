using VisualSorting.Core.Algorithms;

namespace VisualSorting.Core;

public static class AlgorithmRegistry
{
    public static IReadOnlyList<ISortAlgorithm> CreateAll() => new ISortAlgorithm[]
    {
        new BubbleSort(),
        new CocktailShakerSort(),
        new CombSort(),
        new GnomeSort(),
        new HeapSort(),
        new InsertionSort(),
        new MergeSort(),
        new QuickSort(),
        new SelectionSort(),
        new ShellSort(),
    };
}
