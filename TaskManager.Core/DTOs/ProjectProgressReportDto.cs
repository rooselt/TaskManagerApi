using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.DTOs
{ 

public class ProjectProgressReportDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double ProgressPercentage { get; set; }
        public Dictionary<string, int> TasksByStatus { get; set; } = [];
        public Dictionary<string, int> TasksByPriority { get; set; } = [];
        public int OverdueTasks { get; set; }
    }
}
