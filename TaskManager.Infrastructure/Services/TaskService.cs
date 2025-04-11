
namespace TaskManager.Infrastructure.Services
{
    using Microsoft.Extensions.Logging;   
    using TaskManager.Core.DTOs;
    using TaskManager.Core.Entities;
    using TaskManager.Core.Enum;
    using TaskManager.Core.Interfaces.Repository;
    using TaskManager.Core.Interfaces.Service;
    using TaskStatus = Core.Enum.TaskStatus;


    public class TaskService(ITaskRepository taskRepository,
                      IProjectRepository projectRepository,
                      IUserRepository userRepository,
                      IHistoryService historyService,
                      ICommentRepository commentRepository,
                      ILogger<TaskService> logger) : ITaskService
    {
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IHistoryService _historyService = historyService;
        private readonly ILogger<TaskService> _logger = logger;
        private readonly ICommentRepository _commentRepository = commentRepository;

        public async Task<Comment> AddCommentToTask(Guid taskId, AddCommentDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(taskId) ?? throw new NotFoundException(nameof(TaskItem), taskId);
            var comment = new Comment
            {
                Content = dto.Content,
                AuthorId = dto.AuthorId,
                TaskId = taskId,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            await _historyService.RecordCommentAddedAsync(taskId, dto.AuthorId, dto.Content);

            return comment;
        }

        public async Task<TaskItem> CreateTask(Guid projectId, CreateTaskDto dto)
        {
            var projectTaskCount = await _projectRepository.CountTasksInProjectAsync(projectId);
            if (projectTaskCount >= 20)
                throw new BusinessException("Project has reached the maximum number of tasks (20)");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title cannot be null or empty", nameof(dto));

            var task = new TaskItem
            {
                Title = dto.Title!,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = TaskStatus.Pending,
                Priority = dto.Priority,
                ProjectId = projectId,
                CreatedByUserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddAsync(task);
            await _historyService.RecordHistoryAsync(task.Id, dto.UserId, "Task created", HistoryActionType.Created);

            return task;
        }
        public async Task<IEnumerable<TaskItem>> GetProjectTasks(Guid projectId)
        {
            _logger.LogInformation("Buscando tarefas do projeto {ProjectId}", projectId);

            // Verifica se o projeto existe
            var projectExists = await _projectRepository.ExistsAsync(projectId);
            if (!projectExists)
            {
                _logger.LogWarning("Projeto não encontrado: {ProjectId}", projectId);
                throw new NotFoundException(nameof(Project), projectId);
            }

            // Obtém as tarefas do projeto
            var tasks = await _taskRepository.GetTasksByProjectAsync(projectId);

            _logger.LogInformation("Encontradas {TaskCount} tarefas para o projeto {ProjectId}", tasks.Count(), projectId);

            return tasks;
        }

        public async Task<TaskItem> UpdateTask(Guid projectId, Guid taskId, UpdateTaskDto dto)
        {
            _logger.LogInformation("Atualizando tarefa {TaskId} no projeto {ProjectId}", taskId, projectId);

            // Validações iniciais
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.ProjectId != projectId)
            {
                _logger.LogWarning("Tarefa não encontrada: {TaskId} no projeto {ProjectId}", taskId, projectId);
                throw new NotFoundException(nameof(TaskItem), taskId);
            }

            var userExists = await _userRepository.ExistsAsync(dto.UserId);
            if (!userExists)
            {
                _logger.LogWarning("Usuário não encontrado: {UserId}", dto.UserId);
                throw new NotFoundException(nameof(User), dto.UserId);
            }

            // Registra o estado anterior para histórico
            var oldStatus = task.Status;
            var oldTitle = task.Title;
            var oldDescription = task.Description;
            var oldDueDate = task.DueDate;

            // Aplica as atualizações
            if (!string.IsNullOrEmpty(dto.Title) && dto.Title != task.Title)
            {
                task.Title = dto.Title;
                await _historyService.RecordHistoryAsync(taskId, dto.UserId,
                    $"Título alterado de '{oldTitle}' para '{dto.Title}'",
                    HistoryActionType.Updated);
            }

            if (!string.IsNullOrEmpty(dto.Description) && dto.Description != task.Description)
            {
                task.Description = dto.Description;
                await _historyService.RecordHistoryAsync(taskId, dto.UserId,
                    "Descrição da tarefa atualizada",
                    HistoryActionType.Updated);
            }

            if (dto.DueDate.HasValue && dto.DueDate.Value != task.DueDate)
            {
                var oldDueDateStr = oldDueDate.ToString("dd/MM/yyyy");
                var newDueDateStr = dto.DueDate.Value.ToString("dd/MM/yyyy");

                task.DueDate = dto.DueDate.Value;
                await _historyService.RecordHistoryAsync(taskId, dto.UserId,
                    $"Data de vencimento alterada de {oldDueDateStr} para {newDueDateStr}",
                    HistoryActionType.Updated);
            }

            if (dto.Status.HasValue && dto.Status.Value != task.Status)
            {
                task.Status = dto.Status.Value;
                await _historyService.RecordStatusChangeAsync(taskId, dto.UserId, oldStatus, dto.Status.Value);

                // Se a tarefa foi concluída, registra a data de conclusão
                if (dto.Status.Value == TaskStatus.Completed)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
            }

            // Salva as alterações
            await _taskRepository.UpdateAsync(task);
            _logger.LogInformation("Tarefa {TaskId} atualizada com sucesso", taskId);

            return task;
        }

        public async Task DeleteTask(Guid projectId, Guid taskId)
        {
            _logger.LogInformation("Iniciando exclusão da tarefa {TaskId} do projeto {ProjectId}", taskId, projectId);

            // Verifica se a tarefa existe e pertence ao projeto
            var task = await _taskRepository.GetTaskAsync(taskId);
            if (task == null || task.ProjectId != projectId)
            {
                _logger.LogWarning("Tarefa não encontrada: {TaskId} no projeto {ProjectId}", taskId, projectId);
                throw new NotFoundException(nameof(TaskItem), taskId);
            }

            // Verifica se o projeto associado à tarefa não é nulo antes de acessar o OwnerId
            if (task.Project == null)
            {
                _logger.LogWarning("Projeto associado à tarefa {TaskId} é nulo", taskId);
                throw new InvalidOperationException($"Projeto associado à tarefa {taskId} é nulo.");
            }          

            await _taskRepository.DeleteTaskAsync(taskId);

            // Registra no histórico
            await _historyService.RecordHistoryAsync(taskId, task.Project.OwnerId,
                "Tarefa excluída",
                HistoryActionType.Deleted);

            _logger.LogInformation("Tarefa {TaskId} excluída com sucesso", taskId);
        }



        public async Task<UserProductivityReportDto> GetUserProductivityReportAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var tasksCreated = await _taskRepository.CountTasksCreatedByUserAsync(userId, startDate, endDate);
            var tasksCompleted = await _taskRepository.CountTasksByUserAndStatusAsync(userId, TaskStatus.Completed, startDate, endDate);

            return new UserProductivityReportDto
            {
                UserId = userId,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TasksCreated = tasksCreated,
                TasksCompleted = tasksCompleted,
                CompletionRate = tasksCreated > 0 ? (double)tasksCompleted / tasksCreated * 100 : 0
            };
        }

    }
}