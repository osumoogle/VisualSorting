namespace VisualSorting.Core.Algorithms;

public sealed class CocktailShakerSort : ISortAlgorithm
{
    public string Name => "Cocktail Shaker Sort";
    public string TimeBest => "O(n)";
    public string TimeAverage => "O(n²)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 16;

    public string Description => """
        Cocktail shaker sort is bubble sort that changes direction on every pass, like liquid sloshing back and forth in a shaker. A forward pass bubbles the largest remaining value to the right end, then a backward pass sinks the smallest remaining value to the left end.

        The sorted region therefore grows from BOTH ends toward the middle — in the visualization you'll see green appear alternately on the right edge and the left edge, and the active comparison sweep ping-pong between them across an ever-narrowing window.

        The bidirectional sweep fixes bubble sort's "turtle" problem: in plain bubble sort a small value near the right end crawls left only one position per pass, but the backward pass carries it all the way left in a single sweep. Like bubble sort, the algorithm stops early when a pass completes with no swaps.

        Despite the improvement, it does the same kind of neighbor-only work, so the complexity class is unchanged: O(n) best case on nearly-sorted input, O(n²) average and worst case. It is stable and in-place, and mainly of interest as a teaching refinement of bubble sort.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        int start = 0;
        int end = items.Length - 1;
        bool swapped = true;

        while (swapped && start < end)
        {
            swapped = false;
            for (int i = start; i < end; i++)
            {
                yield return SortStep.Compare(i, i + 1);
                if (items[i] > items[i + 1])
                {
                    (items[i], items[i + 1]) = (items[i + 1], items[i]);
                    yield return SortStep.Swap(i, i + 1);
                    swapped = true;
                }
            }
            if (!swapped)
                break;

            yield return SortStep.MarkSorted(end);
            end--;

            swapped = false;
            for (int i = end - 1; i >= start; i--)
            {
                yield return SortStep.Compare(i, i + 1);
                if (items[i] > items[i + 1])
                {
                    (items[i], items[i + 1]) = (items[i + 1], items[i]);
                    yield return SortStep.Swap(i, i + 1);
                    swapped = true;
                }
            }
            yield return SortStep.MarkSorted(start);
            start++;
        }
    }
}
