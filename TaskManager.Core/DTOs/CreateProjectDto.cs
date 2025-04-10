using System.ComponentModel.DataAnnotations;


namespace TaskManager.Core.DTOs
{
    
    public class CreateProjectDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }

                
        [StringLength(500, MinimumLength = 3)]
        public string? Description { get; set; }

        [Required]
        public Guid OwnerId { get; set; }
    }

}
