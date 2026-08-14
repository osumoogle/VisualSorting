using VisualSorting.Core;
using Xunit;

namespace VisualSorting.Core.Tests;

public class SortAlgorithmTests
{
    private static readonly Dictionary<string, int[]> Inputs = new()
    {
        ["empty"] = Array.Empty<int>(),
        ["single"] = new[] { 42 },
        ["sorted"] = Enumerable.Range(1, 20).ToArray(),
        ["reversed"] = Enumerable.Range(1, 20).Reverse().ToArray(),
        ["duplicates"] = new[] { 5, 3, 5, 1, 3, 5, 2, 2, 4, 1 },
        ["all-equal"] = Enumerable.Repeat(7, 15).ToArray(),
        ["random"] = MakeRandom(120, seed: 42),
    };

    private static int[] MakeRandom(int n, int seed)
    {
        var random = new Random(seed);
        return Enumerable.Range(0, n).Select(_ => random.Next(1, 1000)).ToArray();
    }

    public static TheoryData<string, string> AlgorithmInputPairs()
    {
        var data = new TheoryData<string, string>();
        foreach (var algorithm in AlgorithmRegistry.CreateAll())
            foreach (var inputName in Inputs.Keys)
                data.Add(algorithm.Name, inputName);
        return data;
    }

    private static ISortAlgorithm Resolve(string name) =>
        AlgorithmRegistry.CreateAll().Single(a => a.Name == name);

    [Theory]
    [MemberData(nameof(AlgorithmInputPairs))]
    public void SortProducesSortedArray(string algorithmName, string inputName)
    {
        var algorithm = Resolve(algorithmName);
        var array = (int[])Inputs[inputName].Clone();
        var expected = Inputs[inputName].OrderBy(x => x).ToArray();

        algorithm.Sort(array).ToList(); // enumerate fully; sorts in place

        Assert.Equal(expected, array);
    }

    [Theory]
    [MemberData(nameof(AlgorithmInputPairs))]
    public void ReplayingStepsReproducesTheSort(string algorithmName, string inputName)
    {
        // The visualizer replays steps against its own copy of the data, so the
        // step stream alone must fully describe every mutation the sort makes.
        var algorithm = Resolve(algorithmName);
        var workingCopy = (int[])Inputs[inputName].Clone();
        var replayCopy = (int[])Inputs[inputName].Clone();
        var expected = Inputs[inputName].OrderBy(x => x).ToArray();

        var steps = algorithm.Sort(workingCopy).ToList();
        SortStepApplier.ApplyAll(replayCopy, steps);

        Assert.Equal(expected, replayCopy);
    }

    [Theory]
    [MemberData(nameof(AlgorithmInputPairs))]
    public void AllStepIndicesAreWithinBounds(string algorithmName, string inputName)
    {
        var algorithm = Resolve(algorithmName);
        var array = (int[])Inputs[inputName].Clone();

        foreach (var step in algorithm.Sort(array))
        {
            Assert.InRange(step.IndexA, 0, Math.Max(0, array.Length - 1));
            Assert.InRange(step.IndexB, 0, Math.Max(0, array.Length - 1));
        }
    }

    [Fact]
    public void RegistryContainsDistinctlyNamedAlgorithms()
    {
        var algorithms = AlgorithmRegistry.CreateAll();
        Assert.True(algorithms.Count >= 10);
        Assert.Equal(algorithms.Count, algorithms.Select(a => a.Name).Distinct().Count());
    }

    [Fact]
    public void EveryAlgorithmReportsComplexityMetadata()
    {
        foreach (var algorithm in AlgorithmRegistry.CreateAll())
        {
            Assert.StartsWith("O(", algorithm.TimeBest);
            Assert.StartsWith("O(", algorithm.TimeAverage);
            Assert.StartsWith("O(", algorithm.TimeWorst);
            Assert.StartsWith("O(", algorithm.SpaceComplexity);
            Assert.True(algorithm.AuxiliaryBytes(100) > 0);
            Assert.False(string.IsNullOrWhiteSpace(algorithm.Description));
            Assert.True(algorithm.Description.Length > 300,
                $"{algorithm.Name} description should be detailed, got {algorithm.Description.Length} chars");
        }
    }
}
