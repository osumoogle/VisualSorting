namespace VisualSorting.Core.Algorithms;

public sealed class GnomeSort : ISortAlgorithm
{
    public string Name => "Gnome Sort";
    public string TimeBest => "O(n)";
    public string TimeAverage => "O(n²)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 8;

    public string Description => """
        Gnome sort is named after a garden gnome sorting flower pots: look at the pot beside you — if it and the previous pot are in order, step forward; if they're out of order, swap them and step backward. When you reach the far end, everything is sorted.

        The algorithm keeps a single position marker. If the element at the marker is at least as large as the one before it, the marker advances. Otherwise the two are swapped and the marker steps back to re-check the previous pair — the swapped element keeps walking left until it fits, then the marker walks forward again over ground it has already confirmed.

        In the visualization this looks like a cursor that mostly creeps rightward, but stutters backward in little bursts whenever it hits an out-of-place element, escorting it leftward to its home. The effect is the same element motion as insertion sort, just discovered by local back-stepping instead of an explicit inner loop.

        Because it re-walks forward over territory it backed through, it does extra comparisons compared to insertion sort, but the same swaps. Best case O(n) on sorted input, O(n²) average and worst. It is stable, in-place, and prized for having one of the shortest correct implementations of any sort.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        int pos = 0;
        while (pos < items.Length)
        {
            if (pos == 0)
            {
                pos++;
                continue;
            }
            yield return SortStep.Compare(pos - 1, pos);
            if (items[pos - 1] <= items[pos])
            {
                pos++;
            }
            else
            {
                (items[pos - 1], items[pos]) = (items[pos], items[pos - 1]);
                yield return SortStep.Swap(pos - 1, pos);
                pos--;
            }
        }
    }
}
