using BenchmarkDotNet.Running;
using FrozenCollectionsBmk;

BenchmarkRunner.Run<ConstructionBmk>();
BenchmarkRunner.Run<ReadBmk>();
BenchmarkRunner.Run<WriteBmk>();