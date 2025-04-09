using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs;
using TaskManager.Core.Interfaces.Service;

namespace TaskManagerAPI.Controllers
{

    [ApiController]
    [Route("api/projects/{projectId}/tasks")]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        private readonly ITaskService _taskService = taskService;

        [HttpGet]
        public async Task<IActionResult> GetTasks(Guid projectId)
        {
            var tasks = await _taskService.GetProjectTasks(projectId);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto dto)
        {
            var task = await _taskService.CreateTask(projectId, dto);
            return CreatedAtAction(nameof(GetTasks), new { projectId, id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid projectId, Guid id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _taskService.UpdateTask(projectId, id, dto);
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid projectId, Guid id)
        {
            await _taskService.DeleteTask(projectId, id);
            return NoContent();
        }

        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentDto dto)
        {
            var comment = await _taskService.AddCommentToTask(id, dto);
            return Ok(comment);
        }
    }

}