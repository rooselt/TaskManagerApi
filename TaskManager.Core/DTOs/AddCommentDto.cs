using System.ComponentModel.DataAnnotations;


namespace TaskManager.Core.DTOs
{
    public class AddCommentDto
    {
        [Required]
        [StringLength(1000)]
        public required string Content { get; set; }

        [Required]
        public Guid AuthorId { get; set; }
    }


    public class CommentDto
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public Guid AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
