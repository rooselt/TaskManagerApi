using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.DTOs
{
    
    public class CreateProjectDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }

        [Required]
        public Guid OwnerId { get; set; }
    }

}
