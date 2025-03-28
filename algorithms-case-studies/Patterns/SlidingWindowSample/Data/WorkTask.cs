namespace SlidingWindowSample.Data
{
    /// <summary>
    /// Work Assessment data
    /// </summary>
    /// <param name="Effort">Estimation, assessment value</param>
    /// <param name="Deviation">Assessment deviation percentage</param>
    /// <param name="Value">Impact, value of the work. Higher has more value when accomplished</param>
    public record struct WorkTask(int Effort, double Deviation, double Value)
    {
        public static WorkTask Empty => new WorkTask(0, 0, 0);

        public static WorkTask operator +(WorkTask workTaskA, WorkTask workTaskB)
        {
            var worstDeviation = workTaskA.Deviation > workTaskB.Deviation ? workTaskA.Deviation : workTaskB.Deviation;
            return new WorkTask(workTaskA.Effort + workTaskB.Effort, worstDeviation, workTaskA.Value + workTaskB.Value);
        }
    }
}
