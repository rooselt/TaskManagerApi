﻿namespace TaskManager.Core.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
        public ICollection<TaskItem> Tasks { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }      
   
}
