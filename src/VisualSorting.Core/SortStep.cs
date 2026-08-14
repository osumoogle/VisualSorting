namespace VisualSorting.Core;

public enum SortStepType
{
    /// <summary>Two indices were compared. IndexA and IndexB are the compared positions.</summary>
    Compare,

    /// <summary>The values at IndexA and IndexB were exchanged.</summary>
    Swap,

    /// <summary>Value was written into position IndexA (used by non-swap algorithms such as merge sort).</summary>
    Overwrite,

    /// <summary>The element at IndexA is in its final sorted position.</summary>
    MarkSorted,
}

/// <summary>
/// A single observable operation performed by a sorting algorithm.
/// Replaying the full step stream against a copy of the original input
/// must reproduce the sorted array (see <see cref="SortStepApplier"/>).
/// </summary>
public readonly record struct SortStep(SortStepType Type, int IndexA, int IndexB, int Value = 0)
{
    public static SortStep Compare(int a, int b) => new(SortStepType.Compare, a, b);
    public static SortStep Swap(int a, int b) => new(SortStepType.Swap, a, b);
    public static SortStep Overwrite(int index, int value) => new(SortStepType.Overwrite, index, index, value);
    public static SortStep MarkSorted(int index) => new(SortStepType.MarkSorted, index, index);
}
