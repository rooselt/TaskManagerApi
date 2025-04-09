using System.ComponentModel.DataAnnotations;


namespace TaskManager.Core.DTOs
{
    public class UpdateProjectDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }
    }
}
