using TaskManager.Infrastructure.Services;
using Moq;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;

namespace TaskManager.Test.Services
{
    [TestClass()]
    public class ReportServiceTests
    {

        private Mock<ITaskRepository>? _taskRepositoryMock;
        private Mock<IProjectRepository>? _projectRepositoryMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ILogger<ReportService>>? _loggerMock;
        private ReportService? _reportService;

        [TestInitialize]
        public void Setup()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<ReportService>>();

            _reportService = new ReportService(
                _taskRepositoryMock.Object,
                _projectRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task GetPerformanceReportAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotManager()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            _userRepositoryMock!.Setup(repo => repo.IsManagerAsync(managerId)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _reportService!.GetPerformanceReportAsync(managerId));
        }

        [TestMethod]
        public async Task GetProjectProgressReportAsync_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _projectRepositoryMock!.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                _reportService!.GetProjectProgressReportAsync(projectId));
        }

        [TestMethod]
        public async Task GetUserTasksReportAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock!.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                _reportService!.GetUserTasksReportAsync(userId));
        }

        [TestMethod]
        public async Task GetTasksStatusReportAsync_ShouldReturnCorrectReport_WhenProjectsExist()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var projects = new List<Project> { new Project { Id = projectId, Name = "Test Project" } };
            var tasks = new List<TaskItem>
        {
            new() { ProjectId = projectId, Status = TaskStatus.Completed, DueDate = DateTime.UtcNow.AddDays(-1), Description="Test Description", Title="Test" }
        };

            _projectRepositoryMock!.Setup(repo => repo.GetAllAsync()).ReturnsAsync(projects);
            _taskRepositoryMock!.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _reportService!.GetTasksStatusReportAsync();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().CompletedTasks);
        }
      
    }

}
