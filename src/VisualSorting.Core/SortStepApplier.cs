namespace VisualSorting.Core;

/// <summary>
/// Replays sort steps against an array. Used by the visualizer's playback engine
/// and by tests to prove a step stream faithfully reproduces the sort.
/// </summary>
public static class SortStepApplier
{
    public static void Apply(int[] array, in SortStep step)
    {
        switch (step.Type)
        {
            case SortStepType.Swap:
                (array[step.IndexA], array[step.IndexB]) = (array[step.IndexB], array[step.IndexA]);
                break;
            case SortStepType.Overwrite:
                array[step.IndexA] = step.Value;
                break;
        }
    }

    public static void ApplyAll(int[] array, IEnumerable<SortStep> steps)
    {
        foreach (var step in steps)
            Apply(array, step);
    }
}
