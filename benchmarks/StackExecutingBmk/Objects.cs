using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace StackExecutingBmk;

[SimpleJob(RuntimeMoniker.Net60)]
public class ExecutingOnStackBmk
{
    private readonly List<Measurement> _onHeapMeasurements;
    private readonly Measurement[] _measurementsArray;
    private readonly Random _random = new();

    public ExecutingOnStackBmk()
    {
        const int numItems = 1024;
        
        _onHeapMeasurements = new List<Measurement>(); // on heap
        _measurementsArray = new Measurement[numItems]; // completely on stack

        for (var i = 0; i < numItems; i++)
        {
            var m = new Measurement(i + 100, _random.NextDouble(), _random.NextDouble());
            
            // Use a copy of m for each
            _onHeapMeasurements.Add(m);
            _measurementsArray[i] = m;
        }
    }

    [Benchmark]
    public void OnHeapTest()
    {
        var counter = 10000;
        while (counter-- > 0)
            _ = _onHeapMeasurements.Sum(v => v.Value) / _onHeapMeasurements.Count;
    }

    [Benchmark]
    public void OnStackTest()
    {
        var counter = 10000;
        while (counter-- > 0)
            _ = _measurementsArray.Sum(v => v.Value) / _measurementsArray.Length;
    }
}

/// <summary>
/// Example of a measurement object
/// </summary>
public readonly struct Measurement
{
    public Measurement(int waveLength, double horizontalRawValue, double? verticalRawValue)
    {
        HorizontalRawValue = horizontalRawValue;
        VerticalRawValue = verticalRawValue;
        WaveLength = waveLength;
    }
    
    public double HorizontalRawValue { get; }
    public double? VerticalRawValue { get; }
    public int WaveLength { get; }

    private double Average => VerticalRawValue.HasValue
        ? (VerticalRawValue.Value + HorizontalRawValue) / 2
        : HorizontalRawValue;
    
    public double Value => (Math.Abs(Average) + Average) / 2;
}