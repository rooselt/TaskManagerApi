namespace TaskManager.Core.DTOs
{

    public class PerformanceReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTasksCompleted { get; set; }
        public double AverageTasksCompletedPerUser { get; set; }
        public IEnumerable<UserPerformanceDto> UserPerformances { get; set; } = [];
    }
}
