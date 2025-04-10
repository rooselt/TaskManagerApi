using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Interfaces.Repository;
using TaskManager.Infrastructure.Services;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;

namespace TaskManager.Test.Services
{
    [TestClass]
    public class HistoryServiceTests
    {
        private Mock<ITaskHistoryRepository>? _historyRepositoryMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ITaskRepository>? _taskRepositoryMock;
        private Mock<ILogger<HistoryService>>? _loggerMock;
        private HistoryService? _historyService;

        [TestInitialize]
        public void Setup()
        {
            _historyRepositoryMock = new Mock<ITaskHistoryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _loggerMock = new Mock<ILogger<HistoryService>>();

            _historyService = new HistoryService(
                _historyRepositoryMock.Object,
                _userRepositoryMock.Object,
                _taskRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public async Task RecordHistoryAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _taskRepositoryMock!.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                _historyService!.RecordHistoryAsync(taskId, userId, "Change description", HistoryActionType.StatusChanged));
        }

        [TestMethod]
        public async Task GetTaskHistoryAsync_ShouldReturnOrderedHistory_WhenTaskExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var history = new List<TaskHistory>        {
            new() { ChangedAt = DateTime.UtcNow.AddMinutes(-10), ChangeDescription="Test"   },
            new() { ChangedAt = DateTime.UtcNow, ChangeDescription="Test"  }
        };
            _taskRepositoryMock?.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
            _historyRepositoryMock?.Setup(repo => repo.GetByTaskIdAsync(taskId)).ReturnsAsync(history);

            // Act
            var result = await _historyService!.GetTaskHistoryAsync(taskId);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.First().ChangedAt > result.Last().ChangedAt);
        }


        [TestMethod]
        public async Task RecordStatusChangeAsync_ShouldRecordHistory_WhenTaskAndUserExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oldStatus = TaskStatus.Pending;
            var newStatus = TaskStatus.Completed;

            _taskRepositoryMock!.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
            _userRepositoryMock!.Setup(repo => repo.ExistsAsync(userId)).ReturnsAsync(true);

            // Act
            await _historyService!.RecordStatusChangeAsync(taskId, userId, oldStatus, newStatus);

            // Assert
            _historyRepositoryMock!.Verify(repo =>
                repo.AddAsync(It.Is<TaskHistory>(h =>
                    h.TaskId == taskId &&
                    h.ChangedByUserId == userId &&
                    h.ChangeDescription == $"Status alterado de {oldStatus} para {newStatus}" &&
                    h.ActionType == HistoryActionType.StatusChanged
                )), Times.Once);
        }

        [TestMethod]
        public async Task RecordCommentAddedAsync_ShouldRecordHistory_WhenTaskAndUserExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var commentContent = "Este é um comentário de teste.";

            _taskRepositoryMock!.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
            _userRepositoryMock!.Setup(repo => repo.ExistsAsync(userId)).ReturnsAsync(true);

            // Act
            await _historyService!.RecordCommentAddedAsync(taskId, userId, commentContent);

            // Assert
            _historyRepositoryMock!.Verify(repo =>
                repo.AddAsync(It.Is<TaskHistory>(h =>
                    h.TaskId == taskId &&
                    h.ChangedByUserId == userId &&
                    h.ChangeDescription == $"Comentário adicionado: \"{commentContent}\"" &&
                    h.ActionType == HistoryActionType.CommentAdded
                )), Times.Once);
        }

        [TestMethod]
        public async Task RecordCommentAddedAsync_ShouldTruncateComment_WhenCommentIsTooLong()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var longComment = new string('A', 100); // Comentário com 100 caracteres
            var truncatedComment = $"{longComment[..47]}...";

            _taskRepositoryMock!.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
            _userRepositoryMock!.Setup(repo => repo.ExistsAsync(userId)).ReturnsAsync(true);

            // Act
            await _historyService!.RecordCommentAddedAsync(taskId, userId, longComment);

            // Assert
            _historyRepositoryMock!.Verify(repo =>
                repo.AddAsync(It.Is<TaskHistory>(h =>
                    h.TaskId == taskId &&
                    h.ChangedByUserId == userId &&
                    h.ChangeDescription == $"Comentário adicionado: \"{truncatedComment}\"" &&
                    h.ActionType == HistoryActionType.CommentAdded
                )), Times.Once);
        }

        [TestMethod]
        public async Task RecordStatusChangeAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oldStatus = TaskStatus.Pending;
            var newStatus = TaskStatus.Completed;

            _taskRepositoryMock!.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                _historyService!.RecordStatusChangeAsync(taskId, userId, oldStatus, newStatus));
        }

        [TestMethod]
        public async Task RecordCommentAddedAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var commentContent = "Comentário de teste.";

            _taskRepositoryMock!.Setup(repo => repo.ExistsAsync(taskId)).ReturnsAsync(true);
            _userRepositoryMock!.Setup(repo => repo.ExistsAsync(userId)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                _historyService!.RecordCommentAddedAsync(taskId, userId, commentContent));
        }
    }

}