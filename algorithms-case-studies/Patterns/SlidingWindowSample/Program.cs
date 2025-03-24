using System.Globalization;

namespace SlidingWindowSample;

class Program
{
    static void Main()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        
        var tasks = Data.Generator.SampleWorkTasks(100);
        Report.WorkTasks(tasks);
    }
}