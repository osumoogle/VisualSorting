namespace VisualSorting.Core.Algorithms;

public sealed class InsertionSort : ISortAlgorithm
{
    public string Name => "Insertion Sort";
    public string TimeBest => "O(n)";
    public string TimeAverage => "O(n²)";
    public string TimeWorst => "O(n²)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 12;

    public string Description => """
        Insertion sort works the way most people sort a hand of playing cards: keep the left part of the array sorted, take the next unsorted element, and walk it leftward past every larger element until it lands in its correct slot within the sorted part.

        In the visualization, watch each new bar travel leftward through a chain of quick compare-and-swap steps until it stops. The left portion of the array stays sorted at all times (though it isn't shown green until the end, because elements there can still be pushed right by later insertions).

        Its efficiency depends entirely on how far elements must travel. If the array is already nearly sorted, each element moves barely at all and the algorithm runs in O(n) — insertion sort is the standard choice for "finish the job" work inside hybrid sorts like Timsort and introsort, and for small arrays. On random input each element travels half the sorted region on average, giving O(n²); on reversed input every element travels the full distance, the worst case.

        The number of swaps equals exactly the number of inversions in the input (pairs that are out of order), making insertion sort a physical measurement of how unsorted the data is. It is stable and in-place.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        // Swap-based variant so every data movement is visible as a single step.
        for (int i = 1; i < items.Length; i++)
        {
            for (int j = i; j > 0; j--)
            {
                yield return SortStep.Compare(j - 1, j);
                if (items[j - 1] <= items[j])
                    break;
                (items[j - 1], items[j]) = (items[j], items[j - 1]);
                yield return SortStep.Swap(j - 1, j);
            }
        }
    }
}
