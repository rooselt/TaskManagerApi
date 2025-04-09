using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Core;
using TaskManager.Core.DTOs;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Interfaces.Repository;
using TaskManager.Infrastructure.Services;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;

namespace TaskManager.Test
{
    [TestClass]
    public class TaskServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepo = null!;
        private Mock<IProjectRepository> _mockProjectRepo = null!;
        private Mock<IUserRepository> _mockUserRepo = null!;
        private Mock<IHistoryService> _mockHistoryService = null!;
        private Mock<ICommentRepository> _mockCommentService = null!;
        private TaskService _taskService = null!;
        private Mock<ILogger<TaskService>> _logger = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockTaskRepo = new Mock<ITaskRepository>();
            _mockProjectRepo = new Mock<IProjectRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockHistoryService = new Mock<IHistoryService>();
            _mockCommentService = new Mock<ICommentRepository>();
            _logger = new Mock<ILogger<TaskService>>();

            _taskService = new TaskService(
                _mockTaskRepo.Object,
                _mockProjectRepo.Object,
                _mockUserRepo.Object,
                _mockHistoryService.Object,
                _mockCommentService.Object,
                _logger.Object
            );
        }

        [TestMethod]
        public async Task CreateTask_WhenProjectExistsAndHasSpace_ShouldCreateTask()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockProjectRepo.Setup(x => x.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId, Tasks = [] });

            _mockUserRepo.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });

            var dto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(7),
                Priority = TaskPriority.Medium,
                UserId = userId
            };

            // Act
            var result = await _taskService.CreateTask(projectId, dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Task", result.Title);
            _mockTaskRepo.Verify(x => x.AddAsync(It.IsAny<TaskItem>()));
            _mockHistoryService.Verify(x => x.RecordHistoryAsync(
                It.IsAny<Guid>(), userId, "Task created", HistoryActionType.Created));
        }

        [TestMethod]
        public async Task CreateTask_WhenProjectHas20Tasks_ShouldThrowException()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            var tasks = Enumerable.Range(0, 20).Select(_ => new TaskItem
            {
                Title = "Default Title", // Required property
                Description = "Default Description", // Required property
                CreatedByUserId = Guid.NewGuid(), // Example required property
                ProjectId = Guid.NewGuid() // Example required property
            }).ToList();
   

            _mockProjectRepo.Setup(x => x.GetByIdAsync(projectId))
                .ReturnsAsync(new Project { Id = projectId, Tasks = tasks });

            var dto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Test Description",
                UserId = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsExceptionAsync<BusinessException>(() => _taskService.CreateTask(projectId, dto));
        }

        [TestMethod]
        public async Task UpdateTask_WhenValidData_ShouldUpdateTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Old Title",
                Description = "Old Description",
                Status = TaskStatus.Pending,
                ProjectId = projectId
            };

            _mockTaskRepo.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            var dto = new UpdateTaskDto
            {
                Title = "New Title",
                Status = TaskStatus.InProgress,
                UserId = userId
            };

            // Act
            var result = await _taskService.UpdateTask(projectId, taskId, dto);

            // Assert
            Assert.AreEqual("New Title", result.Title);
            Assert.AreEqual(TaskStatus.InProgress, result.Status);
            _mockTaskRepo.Verify(x => x.UpdateAsync(It.IsAny<TaskItem>()));
            _mockHistoryService.Verify(x => x.RecordHistoryAsync(
                taskId, userId, It.IsAny<string>(), HistoryActionType.StatusChanged));
        }
    }
}