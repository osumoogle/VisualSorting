# Visual Sorting

A WPF (.NET 8) sorting-algorithm visualizer. Pick an algorithm, choose how long the
animation should take, and watch every comparison, swap, and write as the sort runs.

## Run

```
dotnet run --project src/VisualSorting.App
```

## Test

```
dotnet test
```

## Features

- 10 algorithms: Bubble, Cocktail Shaker, Comb, Gnome, Heap, Insertion, Merge, Quick, Selection, Shell
- Duration slider (1–60 s) — the animation is paced so the whole sort fits the chosen time
- Array size slider (10–400 elements) and shuffle button
- Info panel showing each algorithm's best/average/worst Big-O time complexity,
  auxiliary space complexity, and an estimated auxiliary memory figure for the current array size
- Live comparison and swap/write counters with step progress
- Light / dark theme toggle (Theme dropdown in the toolbar; dark is the default)
- "How it works" button opening a detailed, non-modal explanation of the selected
  algorithm: its mechanism, what to watch for in the visualization, and its
  performance characteristics, alongside the complexity and memory summary

Colors: blue = untouched, amber = comparing, red = swapping, purple = write (merge sort), green = in final position.

## Architecture

- **`src/VisualSorting.Core`** — UI-free algorithm library. Each algorithm implements
  `ISortAlgorithm` and yields a stream of `SortStep` records (`Compare`, `Swap`,
  `Overwrite`, `MarkSorted`) while sorting. Replaying the steps against a copy of the
  input reproduces the sort exactly — the invariant the tests enforce.
- **`src/VisualSorting.App`** — WPF front end. Pre-generates the step stream, computes the
  per-step delay from the chosen duration (batching steps per frame when the delay would
  drop below ~10 ms), and animates bars on a `Canvas`.
- **`tests/VisualSorting.Core.Tests`** — xUnit suite: every algorithm × edge-case inputs
  (empty, single, sorted, reversed, duplicates, all-equal, random), verifying in-place
  correctness, step-replay fidelity, index bounds, and complexity metadata.
