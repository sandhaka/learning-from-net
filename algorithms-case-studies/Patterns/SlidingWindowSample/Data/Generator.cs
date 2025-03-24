namespace SlidingWindowSample.Data
{
    public static class Generator
    {
        public static IEnumerable<WorkTask> SampleWorkTasks(int n)
        {
            var rand = new Random();

            for (var i = 0; i < n; i++)
            {
                var effort = rand.Next(4, 40);
                var value = rand.NextDouble();

                var u1 = 1.0 - rand.NextDouble();
                var u2 = 1.0 - rand.NextDouble();
                var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                var stdDev = Math.Abs(effort * randStdNormal);

                yield return new WorkTask(effort, stdDev, value);
            }
        }
    }
}
