
using TaskManager.Core.Enum;
using TaskManager.Core.Interfaces.Repository;
using TaskManager.Core.Interfaces.Service;
using Microsoft.Extensions.Logging;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Services
{
  
    public class HistoryService(
        ITaskHistoryRepository historyRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository,
        ILogger<HistoryService> logger) : IHistoryService
    {
        private readonly ITaskHistoryRepository _historyRepository = historyRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly ILogger<HistoryService> _logger = logger;

        public async Task RecordHistoryAsync(Guid taskId, Guid userId, string changeDescription, HistoryActionType actionType)
        {
            try
            {
                // Verifica se a tarefa existe
                var taskExists = await _taskRepository.ExistsAsync(taskId);
                if (!taskExists)
                {
                    _logger.LogWarning("Tentativa de registrar histórico para tarefa inexistente: {TaskId}", taskId);
                    throw new NotFoundException(nameof(TaskItem), taskId);
                }

                // Verifica se o usuário existe
                var userExists = await _userRepository.ExistsAsync(userId);
                if (!userExists)
                {
                    _logger.LogWarning("Tentativa de registrar histórico com usuário inexistente: {UserId}", userId);
                    throw new NotFoundException(nameof(User), userId);
                }

                var historyEntry = new TaskHistory
                {
                    TaskId = taskId,
                    ChangedByUserId = userId,
                    ChangeDescription = changeDescription,
                    ChangedAt = DateTime.UtcNow,
                    ActionType = actionType
                };

                await _historyRepository.AddAsync(historyEntry);
                _logger.LogInformation("Histórico registrado para tarefa {TaskId}, ação: {ActionType}", taskId, actionType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar histórico para tarefa {TaskId}", taskId);
                throw;
            }
        }

        public async Task<IEnumerable<TaskHistory>> GetTaskHistoryAsync(Guid taskId)
        {
            try
            {
                var taskExists = await _taskRepository.ExistsAsync(taskId);
                if (!taskExists)
                {
                    throw new NotFoundException(nameof(TaskItem), taskId);
                }

                var history = await _historyRepository.GetByTaskIdAsync(taskId);
                return history.OrderByDescending(h => h.ChangedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar histórico da tarefa {TaskId}", taskId);
                throw;
            }
        }

        public async Task RecordStatusChangeAsync(Guid taskId, Guid userId, TaskStatus oldStatus, TaskStatus newStatus)
        {
            var description = $"Status alterado de {oldStatus} para {newStatus}";
            await RecordHistoryAsync(taskId, userId, description, HistoryActionType.StatusChanged);
        }

        public async Task RecordCommentAddedAsync(Guid taskId, Guid userId, string commentContent)
        {
            var truncatedComment = commentContent.Length > 50
                ? $"{commentContent[..47]}..."
                : commentContent;

            var description = $"Comentário adicionado: \"{truncatedComment}\"";
            await RecordHistoryAsync(taskId, userId, description, HistoryActionType.CommentAdded);
        }    
    }
}
