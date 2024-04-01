using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace StackAlloc;

internal class AgeClass
{
    public int Age { get; set; }
}

internal struct AgeStruct
{
    public int Age { get; set; }
}

[ShortRunJob]
[MemoryDiagnoser(displayGenColumns: true)]
public class DataProcessor
{
    private readonly Random _random = new();
    
    [Benchmark(Description = "Use only classes")]
    public void Process1()
    {
        var counter = 10000;
        while (counter-- > 0)
            _ = HeapAllocation(1024);
    }
    
    [Benchmark(Description = "Use only structs")]
    public void Process2()
    {
        var counter = 10000;
        while (counter-- > 0)
            _ = StackAllocation(1024);
    }
    
    [Benchmark(Description = "Use stackalloc")]
    public void Process3()
    {
        var counter = 10000;
        while (counter-- > 0)
            _ = UseStackAlloc(1024);
    }

    [Benchmark(Description = "Use ArrayPool<>")]
    public void Process4()
    {
        var counter = 10000;
        while (counter-- > 0)
            _ = UseArrayPool(1024);
    }

    private double UseStackAlloc(int size)
    {
        Span<AgeStruct> ageStructs = stackalloc AgeStruct[size];
        for (var i = 0; i < 1024; i++)
        {
            ageStructs[i].Age = _random.Next();
        }

        return ageStructs[^1].Age;
    }
    
    private double StackAllocation(int size)
    {
        AgeStruct[] ageStructs = new AgeStruct[size];
        for (var i = 0; i < 1024; i++)
        {
            ageStructs[i].Age = _random.Next();
        }

        return ageStructs[^1].Age;
    }

    private int HeapAllocation(int size)
    {
        var ageClasses = new AgeClass[size];
        for (var i = 0; i < 1024; i++)
        {
            ageClasses[i] = new AgeClass{ Age = _random.Next() };
        }

        return ageClasses.Last().Age;
    }

    private int UseArrayPool(int size)
    {
        AgeStruct[] ageStructs = ArrayPool<AgeStruct>.Shared.Rent(size);
        for (var i = 0; i < 1024; i++)
        {
            ageStructs[i].Age = _random.Next();
        }
        
        return ageStructs[^1].Age;
    }
}