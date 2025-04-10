using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Interfaces.Repository;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController(IReportService reportService) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;

        [HttpGet("performance")]
        public async Task<IActionResult> GetPerformanceReport(
            [FromQuery] Guid managerId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var report = await _reportService.GetPerformanceReportAsync(managerId, startDate, endDate);
            return Ok(report);
        }

        [HttpGet("project-progress/{projectId}")]
        public async Task<IActionResult> GetProjectProgressReport(Guid projectId)
        {
            var report = await _reportService.GetProjectProgressReportAsync(projectId);
            return Ok(report);
        }

        [HttpGet("user-tasks/{userId}")]
        public async Task<IActionResult> GetUserTasksReport(Guid userId)
        {
            var report = await _reportService.GetUserTasksReportAsync(userId);
            return Ok(report);
        }

        [HttpGet("tasks-status")]
        public async Task<IActionResult> GetTasksStatusReport()
        {
            var report = await _reportService.GetTasksStatusReportAsync();
            return Ok(report);
        }
    }
}