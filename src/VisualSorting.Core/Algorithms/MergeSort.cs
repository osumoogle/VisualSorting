namespace VisualSorting.Core.Algorithms;

public sealed class MergeSort : ISortAlgorithm
{
    public string Name => "Merge Sort";
    public string TimeBest => "O(n log n)";
    public string TimeAverage => "O(n log n)";
    public string TimeWorst => "O(n log n)";
    public string SpaceComplexity => "O(n)";
    public long AuxiliaryBytes(int n) => 4L * n; // temp buffer of n 32-bit ints

    public string Description => """
        Merge sort is built on one observation: combining two already-sorted lists into one sorted list is easy and fast. Just repeatedly take the smaller of the two front elements. The algorithm makes every element a sorted list of length one, then merges pairs of lists into sorted runs of 2, 4, 8, ... until a single sorted run covers the whole array.

        This is the bottom-up variant: a first sweep merges neighboring pairs, the next sweep merges neighboring runs of two, and so on — log₂(n) sweeps in total, each touching every element once. Merging copies a run pair into a temporary buffer, then writes elements back in sorted order; those writes appear as purple "overwrite" highlights in the visualization instead of red swaps, because merge sort moves values rather than exchanging pairs.

        Watch the array organize itself in layers: after the first sweep every pair is internally sorted, then every group of four, and the staircase pattern gets smoother with each pass — order emerges everywhere at once rather than from one end.

        Merge sort's great virtues are its guaranteed O(n log n) time on ANY input — sorted, reversed, or adversarial — and its stability (equal elements never reorder), which is why it underlies stable library sorts like Timsort. The cost is the O(n) temporary buffer: it is the one algorithm here that needs scratch memory proportional to the input, visible in the memory panel as the array size grows.
        """;

    public IEnumerable<SortStep> Sort(int[] items)
    {
        // Bottom-up merge sort: merge runs of width 1, 2, 4, ... Writes are
        // surfaced as Overwrite steps since merging is not swap-based.
        int n = items.Length;
        for (int width = 1; width < n; width *= 2)
        {
            for (int lo = 0; lo + width < n; lo += 2 * width)
            {
                int mid = lo + width - 1;
                int hi = Math.Min(lo + 2 * width - 1, n - 1);

                var temp = new int[hi - lo + 1];
                Array.Copy(items, lo, temp, 0, temp.Length);

                int left = 0;                // index into temp (left run)
                int right = mid - lo + 1;    // index into temp (right run)
                int leftEnd = mid - lo;
                int rightEnd = hi - lo;

                for (int k = lo; k <= hi; k++)
                {
                    if (left <= leftEnd && right <= rightEnd)
                        yield return SortStep.Compare(lo + left, mid + 1 + (right - (mid - lo + 1)));

                    int value;
                    if (right > rightEnd || (left <= leftEnd && temp[left] <= temp[right]))
                        value = temp[left++];
                    else
                        value = temp[right++];

                    if (items[k] != value)
                    {
                        items[k] = value;
                        yield return SortStep.Overwrite(k, value);
                    }
                }
            }
        }
    }
}
