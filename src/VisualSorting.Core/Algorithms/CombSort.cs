namespace VisualSorting.Core.Algorithms;

public sealed class CombSort : ISortAlgorithm
{
    public string Name => "Comb Sort";
    public string TimeBest => "O(n log n)";
    public string TimeAverage => "O(n²/2ᵖ)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 16;

    public string Description => """
        Comb sort is to bubble sort what Shell sort is to insertion sort: the same basic pass, but comparing elements a wide gap apart instead of adjacent neighbors, with the gap shrinking each pass until it reaches 1.

        Each pass sweeps left to right comparing positions i and i + gap, swapping when out of order. After every pass the gap is divided by the "shrink factor" 1.3 — a value found empirically to work well — until it reaches 1, after which the algorithm behaves exactly like bubble sort and keeps passing until no swaps occur.

        The wide early gaps exist to kill bubble sort's worst enemy: "turtles," small values near the right end that plain bubble sort drags left only one position per pass. A large gap lets a turtle jump most of the way home in one swap. By the time the gap reaches 1, few inversions remain and the final bubble passes terminate quickly.

        In the visualization the early passes show comparisons between distant bars with occasional long-range swaps, and the array quickly takes on its overall shape; the finale looks like a brisk bubble sort over nearly-sorted data. Average behavior is close to O(n log n) in practice (often written O(n²/2ᵖ) where p is the number of shrink steps), but the worst case is still O(n²). In-place, not stable.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        int n = items.Length;
        int gap = n;
        bool swapped = true;

        while (gap > 1 || swapped)
        {
            gap = Math.Max(1, (int)(gap / 1.3));
            swapped = false;
            for (int i = 0; i + gap < n; i++)
            {
                yield return SortStep.Compare(i, i + gap);
                if (items[i] > items[i + gap])
                {
                    (items[i], items[i + gap]) = (items[i + gap], items[i]);
                    yield return SortStep.Swap(i, i + gap);
                    swapped = true;
                }
            }
        }
    }
}
