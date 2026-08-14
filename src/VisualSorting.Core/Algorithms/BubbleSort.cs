namespace VisualSorting.Core.Algorithms;

public sealed class BubbleSort : ISortAlgorithm
{
    public string Name => "Bubble Sort";
    public string TimeBest => "O(n)";
    public string TimeAverage => "O(n²)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 12; // loop counters and swap temporary

    public string Description => """
        Bubble sort repeatedly walks the array from left to right, comparing each pair of neighboring elements and swapping them whenever the left one is larger. Each full pass "bubbles" the largest remaining value up to its final position at the right end, the way a large air bubble rises through water.

        After the first pass, the biggest element is guaranteed to be last; after the second pass, the second-biggest is in place; and so on. That means each pass can stop one position earlier than the last. As an optimization, if a whole pass completes without a single swap, the array is already sorted and the algorithm stops early — this is what makes its best case O(n) on nearly-sorted data.

        In the visualization, watch the comparison highlight sweep left to right over and over, and notice the green "sorted" region growing in from the right edge, one element per pass. Small values move left only one position per pass ("turtles"), which is the intuitive reason bubble sort is slow: a value that belongs at the far left may need n passes to crawl there.

        Bubble sort is simple and stable (equal elements keep their order), but its O(n²) average and worst case make it impractical beyond teaching. It performs roughly n²/2 comparisons and up to n²/2 swaps on random data.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - 1 - i; j++)
            {
                yield return SortStep.Compare(j, j + 1);
                if (items[j] > items[j + 1])
                {
                    (items[j], items[j + 1]) = (items[j + 1], items[j]);
                    yield return SortStep.Swap(j, j + 1);
                    swapped = true;
                }
            }
            yield return SortStep.MarkSorted(n - 1 - i);
            if (!swapped)
                break;
        }
    }
}
