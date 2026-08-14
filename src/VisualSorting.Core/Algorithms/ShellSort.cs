namespace VisualSorting.Core.Algorithms;

public sealed class ShellSort : ISortAlgorithm
{
    public string Name => "Shell Sort";
    public string TimeBest => "O(n log n)";
    public string TimeAverage => "O(n^1.5)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 16;

    public string Description => """
        Shell sort is insertion sort with a running start. Plain insertion sort is slow because elements move only one position per step, so a value far from home needs many moves. Shell sort first sorts elements that are a large gap apart, letting values leap across the array in a few big hops, then repeats with smaller and smaller gaps until a final ordinary insertion-sort pass (gap 1) finishes the job cheaply.

        This implementation halves the gap each round: n/2, n/4, ..., 1. At gap g, every g-th element forms a subsequence, and each subsequence is insertion-sorted independently — an element compares and swaps with the element g positions to its left, not its immediate neighbor.

        In the visualization the early rounds look chaotic: comparisons and swaps jump between widely separated bars, and the array rapidly becomes "roughly sorted" — every element close to its final home. The later small-gap rounds then look like a fast, low-effort insertion sort with almost nothing left to fix.

        The key fact making this work: an array sorted at gap g stays g-sorted after being sorted at a smaller gap, so progress is never undone. Performance depends on the gap sequence; with halving gaps the worst case is O(n²), average around O(n^1.5). It is in-place but not stable — long jumps can reorder equal elements.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        int n = items.Length;
        for (int gap = n / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < n; i++)
            {
                for (int j = i; j >= gap; j -= gap)
                {
                    yield return SortStep.Compare(j - gap, j);
                    if (items[j - gap] <= items[j])
                        break;
                    (items[j - gap], items[j]) = (items[j], items[j - gap]);
                    yield return SortStep.Swap(j - gap, j);
                }
            }
        }
    }
}
