using SlidingWindowSample.Data;

namespace SlidingWindowSample
{
    public static class Report
    {
        public static void WorkTasks(IEnumerable<WorkTask> tasks)
        {
            Console.WriteLine("Assessment\tDeviation%\tTask Value");
            foreach (var workTask in tasks) Console.WriteLine($"{workTask.Effort}\t\t{workTask.Deviation:F2}\t\t{workTask.Value:F2}");
        }
    }
}
