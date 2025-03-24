namespace SlidingWindowSample.Data
{
    /// <summary>
    /// Work Assessment data
    /// </summary>
    /// <param name="Effort">Estimation, assessment value</param>
    /// <param name="Deviation">Assessment deviation percentage</param>
    /// <param name="Value">Impact, value of the work. Higher has more value when accomplished</param>
    public record struct WorkTask(int Effort, double Deviation, double Value);
}
