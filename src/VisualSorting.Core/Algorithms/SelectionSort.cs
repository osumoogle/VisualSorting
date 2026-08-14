namespace VisualSorting.Core.Algorithms;

public sealed class SelectionSort : ISortAlgorithm
{
    public string Name => "Selection Sort";
    public string TimeBest => "O(n²)";
    public string TimeAverage => "O(n²)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 12;

    public string Description => """
        Selection sort divides the array into a sorted prefix and an unsorted remainder. On each round it scans the entire unsorted part to find the smallest element, swaps it into the next position of the prefix, and grows the sorted region by one.

        In the visualization this gives selection sort a very distinctive rhythm: a long stretch of comparison highlights scanning left to right (the search for the minimum), then a single red swap, then the leftmost unsorted bar turning green. The green region grows steadily from the left, one element per round, and elements never move again once placed.

        Its defining property is doing the fewest swaps of any comparison sort here — at most n − 1 total, one per round — while doing the most predictable amount of comparison work: always about n²/2 comparisons regardless of the input. Even a fully sorted array is scanned completely, which is why best, average, and worst case are all O(n²).

        This trade-off (many reads, very few writes) once made it attractive when writes were expensive, such as flash memory. The common implementation, including this one, is not stable: the long-distance swap can move an element past an equal one.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < n; j++)
            {
                yield return SortStep.Compare(min, j);
                if (items[j] < items[min])
                    min = j;
            }
            if (min != i)
            {
                (items[i], items[min]) = (items[min], items[i]);
                yield return SortStep.Swap(i, min);
            }
            yield return SortStep.MarkSorted(i);
        }
        if (n > 0)
            yield return SortStep.MarkSorted(n - 1);
    }
}
