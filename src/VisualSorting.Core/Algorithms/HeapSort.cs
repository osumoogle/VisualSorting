namespace VisualSorting.Core.Algorithms;

public sealed class HeapSort : ISortAlgorithm
{
    public string Name => "Heap Sort";
    public string TimeBest => "O(n log n)";
    public string TimeAverage => "O(n log n)";
    public string TimeWorst => "O(n log n)";
    public string SpaceComplexity => "O(1)";
    public long AuxiliaryBytes(int n) => 16;

    public string Description => """
        Heap sort treats the array itself as a binary tree: the element at index i has children at 2i + 1 and 2i + 2. It runs in two phases.

        Phase one ("heapify") reorganizes the array into a max-heap — a tree where every parent is at least as large as its children, which puts the largest element at index 0. It does this by "sifting down" each internal node from the middle of the array backward: compare a parent with its larger child and swap downward until the parent dominates its subtree.

        Phase two repeatedly extracts the maximum: swap the root (largest) with the last element of the heap, declare that last slot sorted, shrink the heap by one, and sift the new root down to restore the heap property. In the visualization phase two is unmistakable — a bar from the left edge repeatedly leaps to the boundary of the growing green region on the right, followed by a cascade of comparisons trickling down the tree from position 0.

        Heap sort is the only algorithm in this collection that guarantees O(n log n) in every case while using O(1) extra memory — quicksort can degrade to O(n²) and merge sort needs O(n) scratch space. The price is poor cache behavior (parent/child indices are far apart) and instability. Heapify itself is only O(n); the log factor comes from the n extractions.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        int n = items.Length;
        if (n <= 1)
        {
            if (n == 1)
                yield return SortStep.MarkSorted(0);
            yield break;
        }

        for (int i = n / 2 - 1; i >= 0; i--)
            foreach (var step in SiftDown(items, i, n))
                yield return step;

        for (int end = n - 1; end > 0; end--)
        {
            (items[0], items[end]) = (items[end], items[0]);
            yield return SortStep.Swap(0, end);
            yield return SortStep.MarkSorted(end);
            foreach (var step in SiftDown(items, 0, end))
                yield return step;
        }
        yield return SortStep.MarkSorted(0);
    }

    private static IEnumerable<SortStep> SiftDown(int[] items, int root, int count)
    {
        while (true)
        {
            int child = 2 * root + 1;
            if (child >= count)
                yield break;

            if (child + 1 < count)
            {
                yield return SortStep.Compare(child, child + 1);
                if (items[child + 1] > items[child])
                    child++;
            }

            yield return SortStep.Compare(root, child);
            if (items[root] >= items[child])
                yield break;

            (items[root], items[child]) = (items[child], items[root]);
            yield return SortStep.Swap(root, child);
            root = child;
        }
    }
}
