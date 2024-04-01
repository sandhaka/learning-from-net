using BenchmarkDotNet.Running;
using StackAlloc;

BenchmarkRunner.Run<DataProcessor>();

/*

// * Summary *

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.4 (21F79) [Darwin 21.5.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.202
  [Host]   : .NET 6.0.4 (6.0.422.16404), Arm64 RyuJIT
  .NET 6.0 : .NET 6.0.4 (6.0.422.16404), Arm64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

|             Method |      Mean |    Error |   StdDev |
|------------------- |----------:|---------:|---------:|
| 'Use only classes' | 247.69 ms | 2.604 ms | 2.436 ms |
|   'Use stackalloc' |  31.84 ms | 0.379 ms | 0.355 ms |

*/