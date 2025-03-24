using SlidingWindowSample.Data;

namespace SlidingWindowSample
{
    public enum Mode { Console, Csv }

    public static class Report
    {
        public static void WorkTasks(IEnumerable<WorkTask> tasks, Mode mode = Mode.Console)
        {
            switch (mode)
            {
                case Mode.Console:
                {
                    Console.WriteLine("Assessment\tDeviation%\tTask Value");
                    foreach (var workTask in tasks)
                        Console.WriteLine($"{workTask.Effort}\t\t{workTask.Deviation:F2}\t\t{workTask.Value:F2}");
                    break;
                }
                case Mode.Csv:
                {
                    Console.WriteLine("Assessment,Deviation%,Task Value") ;
                    foreach (var workTask in tasks) Console.WriteLine($"{workTask.Effort},{workTask.Deviation:F2},{workTask.Value:F2}");
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
