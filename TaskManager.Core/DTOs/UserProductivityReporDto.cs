using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.DTOs
{    
    public class UserProductivityReportDto
    {
        public Guid UserId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TasksCreated { get; set; }
        public int TasksCompleted { get; set; }
        public double CompletionRate { get; set; }
        public TimeSpan AverageCompletionTime { get; set; }
    }
}
