using Microsoft.Extensions.Logging;
using TaskManager.Core.DTOs;
using TaskManager.Core.Enum;
using TaskManager.Core.Interfaces.Repository;
using TaskStatus = TaskManager.Core.Enum.TaskStatus;
using System.Collections.Immutable;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Services
{ 
    public class ReportService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ILogger<ReportService> logger) : IReportService
    {
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ILogger<ReportService> _logger = logger;

        public async Task<PerformanceReportDto> GetPerformanceReportAsync(Guid managerId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Gerando relatório de desempenho solicitado pelo gerente {ManagerId}", managerId);

                // Verifica se o usuário é gerente
                var isManager = await _userRepository.IsManagerAsync(managerId);
                if (!isManager)
                {
                    _logger.LogWarning("Acesso não autorizado: usuário {ManagerId} não é gerente", managerId);
                    throw new UnauthorizedAccessException("Apenas gerentes podem acessar este relatório");
                }

                // Define o período padrão (últimos 30 dias)
                var defaultEndDate = DateTime.UtcNow;
                var defaultStartDate = defaultEndDate.AddDays(-30);

                var reportStartDate = startDate ?? defaultStartDate;
                var reportEndDate = endDate ?? defaultEndDate;

                // Obtém todos os usuários (exceto gerentes)
                var users = await _userRepository.GetAllAsync();
                var regularUsers = users.Where(u => u.Role == UserRole.Member).ToList();

                // Obtém todas as tarefas concluídas no período
                var completedTasks = await _taskRepository.GetCompletedTasksByPeriodAsync(reportStartDate, reportEndDate);

                // Calcula métricas
                var userPerformances = regularUsers.Select(async user => new UserPerformanceDto
                {
                    UserId = user.Id,
                    UserName = user.Name ?? "Usuário Desconhecido",
                    TasksCompleted = completedTasks.Count(t => t.AssignedUserId == user.Id),
                    TasksCreated = await _taskRepository.CountTasksCreatedByUserAsync(user.Id, reportStartDate, reportEndDate)
                }).ToImmutableList();

                var totalTasksCompleted = userPerformances.Sum(up => up.Result.TasksCompleted);

                var averageTasksCompleted = regularUsers.Count != 0
                    ? (double)totalTasksCompleted / regularUsers.Count
                    : 0;

                return new PerformanceReportDto
                {
                    StartDate = reportStartDate,
                    EndDate = reportEndDate,
                    TotalTasksCompleted = totalTasksCompleted,
                    AverageTasksCompletedPerUser = averageTasksCompleted,
                    UserPerformances = [.. userPerformances.Select(up => up.Result)]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de desempenho");
                throw;
            }
        }

        public async Task<ProjectProgressReportDto> GetProjectProgressReportAsync(Guid projectId)
        {
            try
            {
                _logger.LogInformation("Gerando relatório de progresso do projeto {ProjectId}", projectId);

                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    _logger.LogWarning("Projeto não encontrado: {ProjectId}", projectId);
                    throw new NotFoundException(nameof(Project), projectId);
                }

                var tasks = await _taskRepository.GetTasksByProjectAsync(projectId);
                var totalTasks = tasks.Count();
                var completedTasks = tasks.Count(t => t.Status == TaskStatus.Completed);

                var progressPercentage = totalTasks > 0
                    ? (double)completedTasks / totalTasks * 100
                    : 0;

                var tasksByStatus = tasks
                    .GroupBy(t => t.Status)
                    .ToDictionary(
                        g => g.Key.ToString(),
                        g => g.Count());

                var tasksByPriority = tasks
                    .GroupBy(t => t.Priority)
                    .ToDictionary(
                        g => g.Key.ToString(),
                        g => g.Count());

                return new ProjectProgressReportDto
                {
                    ProjectId = projectId,
                    ProjectName = project.Name,
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    ProgressPercentage = progressPercentage,
                    TasksByStatus = tasksByStatus,
                    TasksByPriority = tasksByPriority,
                    OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de progresso do projeto {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<UserTasksReportDto> GetUserTasksReportAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Gerando relatório de tarefas do usuário {UserId}", userId);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado: {UserId}", userId);
                    throw new NotFoundException(nameof(User), userId);
                }

                var assignedTasks = await _taskRepository.GetTasksByUserAsync(userId);
                var createdTasks = await _taskRepository.GetTasksCreatedByUserAsync(userId);

                return new UserTasksReportDto
                {
                    UserId = userId,
                    UserName = user.Name,
                    TotalAssignedTasks = assignedTasks.Count(),
                    CompletedAssignedTasks = assignedTasks.Count(t => t.Status == TaskStatus.Completed),
                    PendingAssignedTasks = assignedTasks.Count(t => t.Status == TaskStatus.Pending),
                    OverdueAssignedTasks = assignedTasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
                    TotalCreatedTasks = createdTasks.Count(),
                    TasksByProject = [.. assignedTasks
                        .GroupBy(t => t.ProjectId)
                        .Select(g => new UserTasksByProjectDto
                        {
                            ProjectId = g.Key,
                            ProjectName = g.First().Project?.Name ?? "Unknown",
                            TaskCount = g.Count()
                        })]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de tarefas do usuário {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<TaskStatusReportDto>> GetTasksStatusReportAsync()
        {
            try
            {
                _logger.LogInformation("Gerando relatório de status das tarefas");

                var allTasks = await _taskRepository.GetAllAsync();
                var projects = await _projectRepository.GetAllAsync();

                return projects.Select(project => new TaskStatusReportDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalTasks = allTasks.Count(t => t.ProjectId == project.Id),
                    PendingTasks = allTasks.Count(t => t.ProjectId == project.Id && t.Status == TaskStatus.Pending),
                    InProgressTasks = allTasks.Count(t => t.ProjectId == project.Id && t.Status == TaskStatus.InProgress),
                    CompletedTasks = allTasks.Count(t => t.ProjectId == project.Id && t.Status == TaskStatus.Completed),
                    OverdueTasks = allTasks.Count(t => t.ProjectId == project.Id &&
                                                     t.DueDate < DateTime.UtcNow &&
                                                     t.Status != TaskStatus.Completed)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de status das tarefas");
                throw;
            }
        }
    }
}
