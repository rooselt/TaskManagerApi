using TaskManager.Infrastructure.Services;
using TaskManager.Core.Interfaces.Repository;
using Moq;
using Microsoft.Extensions.Logging;
using TaskManager.Core.Entities;
using TaskManager.Core.DTOs;
using TaskManager.Infrastructure.Repositories;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TaskManager.Test.Services
{
    [TestClass()]
    public class ProjectServiceTests
    {
        private Mock<IProjectRepository>? _mockProjectRepository;
        private Mock<ITaskRepository>? _mockTaskRepository;
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILogger<ProjectService>>? _mockLogger;
        private ProjectService? _projectService;

        [TestInitialize]
        public void Setup()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<ProjectService>>();

            _projectService = new ProjectService(
                _mockProjectRepository.Object,
                _mockTaskRepository.Object,
                _mockUserRepository.Object,
                _mockLogger.Object
            );
        }



        [TestMethod()]
        public async Task GetUserProjectsAsyncTest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var projects = new List<Project> { new Project { Id = projectId, Name = "Test Project" } };
            _mockProjectRepository!.Setup(repo => repo.GetUserProjectsAsync(userId)).ReturnsAsync(projects);
            _mockUserRepository!.Setup(repo => repo.ExistsAsync(userId)).ReturnsAsync(true);
            // Act
            var result = await _projectService!.GetUserProjectsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Test Project", result.ElementAt(0).Name);
        }

        [TestMethod()]
        public async Task GetProjectByIdAsyncTest()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, Name = "Test Project" };
            _mockProjectRepository!.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);

            // Act
            var result = await _projectService!.GetProjectByIdAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result.Id);
            Assert.AreEqual("Test Project", result.Name);
        }

        [TestMethod()]
        public async Task CreateProjectAsyncTest()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Name = "New Project" };
            _mockProjectRepository!.Setup(repo => repo.AddAsync((It.IsAny<Project>())));

            var createProjectDto = new CreateProjectDto
            {
                Name = "New Project",
                OwnerId = Guid.NewGuid(),
                Description = "Project Description"
            };
            
            _mockUserRepository!.Setup(repo => repo.ExistsAsync(createProjectDto.OwnerId)).ReturnsAsync(true);

            // Act
            var result = await _projectService!.CreateProjectAsync(createProjectDto);

            // Assert
            Assert.IsTrue(result != null);
        }

        [TestMethod()]
        public async Task DeleteProjectAsyncTest()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _mockProjectRepository!.Setup(repo => repo.DeleteAsync(It.IsAny<Project>()));

            // Act
            var result = await _projectService!.DeleteProjectAsync(projectId);

            // Assert
            Assert.IsTrue(result != null);
        }

        [TestMethod()]
        public async Task UpdateProjectAsyncTest()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, Name = "Updated Project" };
            var updateProjectDto = new UpdateProjectDto
            {
                Name = "Updated Project"
            };

            _mockProjectRepository!.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(project);   
            _mockProjectRepository!.Setup(repo => repo.UpdateAsync(It.IsAny<Project>()));

            // Act
            var result = await _projectService!.UpdateProjectAsync(projectId, updateProjectDto);
            // Assert
            Assert.IsTrue(result != null);
        }

        [TestMethod()]
        public async Task CountUserProjectsAsyncTest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projectCount = 5;

            _mockUserRepository!.Setup(repo => repo.ExistsAsync(userId)).ReturnsAsync(true); // Adicionado operador de null-forgiving (!)
            _mockProjectRepository!.Setup(repo => repo.CountUserProjectsAsync(userId)).ReturnsAsync(projectCount); // Adicionado operador de null-forgiving (!)

            // Act
            var result = await _projectService!.CountUserProjectsAsync(userId); // Adicionado operador de null-forgiving (!)

            // Assert
            Assert.AreEqual(projectCount, result);
        }
    }
}