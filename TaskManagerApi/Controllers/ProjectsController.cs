using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs;
using TaskManager.Core.Interfaces.Service;

namespace TaskManagerApi.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        private readonly IProjectService _projectService = projectService;

        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] Guid userId)
        {
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            var project = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);

            return NoContent();
        }
    }
}