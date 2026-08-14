namespace VisualSorting.Core.Algorithms;

public sealed class QuickSort : ISortAlgorithm
{
    public string Name => "Quick Sort";
    public string TimeBest => "O(n log n)";
    public string TimeAverage => "O(n log n)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(log n)";
    public long AuxiliaryBytes(int n) =>
        // Explicit partition stack: one (lo, hi) pair per pending range, ~log2(n) deep on average.
        n <= 1 ? 8 : 8 * (long)Math.Ceiling(Math.Log2(n));

    public string Description => """
        Quicksort sorts by divide and conquer: pick one element as the "pivot," partition the array so everything smaller than the pivot ends up on its left and everything larger on its right, then sort the two sides independently the same way. After partitioning, the pivot itself is in its final position forever.

        This implementation uses Lomuto partitioning with the last element of each range as the pivot: a scan runs left to right comparing every element against the pivot, swapping each smaller element into a growing "small values" zone at the front. When the scan ends, the pivot is swapped just past that zone — landing exactly where it belongs — and the two halves are pushed onto an explicit stack to be partitioned in turn.

        In the visualization, watch each partition: comparisons sweep across a range against a fixed bar at its right edge, red swaps compact the smaller bars leftward, then the pivot jumps into place and immediately turns green. Green bars appear scattered across the array rather than growing from one end — each one a pivot that has found its home.

        Average case is O(n log n) with excellent constants and cache behavior, which is why quicksort variants power most standard-library sorts. The weakness: if the pivot is repeatedly the extreme value — for example, last-element pivot on already-sorted data — one side of every partition is empty and it degrades to O(n²). Production versions avoid this with random or median pivots. In-place aside from the O(log n) recursion stack; not stable.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        if (items.Length == 0)
            yield break;

        var stack = new Stack<(int Lo, int Hi)>();
        stack.Push((0, items.Length - 1));

        while (stack.Count > 0)
        {
            var (lo, hi) = stack.Pop();
            if (lo >= hi)
            {
                if (lo == hi)
                    yield return SortStep.MarkSorted(lo);
                continue;
            }

            // Lomuto partition with items[hi] as the pivot.
            int pivot = items[hi];
            int i = lo;
            for (int j = lo; j < hi; j++)
            {
                yield return SortStep.Compare(j, hi);
                if (items[j] < pivot)
                {
                    if (i != j)
                    {
                        (items[i], items[j]) = (items[j], items[i]);
                        yield return SortStep.Swap(i, j);
                    }
                    i++;
                }
            }
            if (i != hi)
            {
                (items[i], items[hi]) = (items[hi], items[i]);
                yield return SortStep.Swap(i, hi);
            }
            yield return SortStep.MarkSorted(i);

            stack.Push((lo, i - 1));
            stack.Push((i + 1, hi));
        }
    }
}
