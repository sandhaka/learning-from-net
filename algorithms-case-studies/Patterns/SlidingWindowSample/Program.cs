namespace SlidingWindowSample;

class Program
{
    static void Main()
    {
        var tasks = Data.Generator.SampleWorkTasks(100);
        Report.WorkTasks(tasks);
    }
}