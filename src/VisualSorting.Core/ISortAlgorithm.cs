namespace VisualSorting.Core;

public interface ISortAlgorithm
{
    string Name { get; }

    /// <summary>Big-O time complexity, best case (e.g. "O(n)").</summary>
    string TimeBest { get; }

    /// <summary>Big-O time complexity, average case.</summary>
    string TimeAverage { get; }

    /// <summary>Big-O time complexity, worst case.</summary>
    string TimeWorst { get; }

    /// <summary>Big-O auxiliary space complexity (memory beyond the input array).</summary>
    string SpaceComplexity { get; }

    /// <summary>
    /// Detailed plain-text explanation of how the algorithm works: its mechanism,
    /// what to watch for in the visualization, and its performance characteristics.
    /// Paragraphs are separated by blank lines.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Estimated auxiliary memory in bytes the algorithm needs for an input of
    /// <paramref name="n"/> 32-bit integers, matching <see cref="SpaceComplexity"/>.
    /// </summary>
    long AuxiliaryBytes(int n);

    /// <summary>
    /// Sorts <paramref name="items"/> in place, yielding each observable operation.
    /// The array is mutated as the sequence is enumerated; enumerate fully to finish the sort.
    /// </summary>
    IEnumerable<SortStep> Sort(int[] items);
}
