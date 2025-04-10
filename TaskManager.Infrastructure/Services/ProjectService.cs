using Microsoft.Extensions.Logging;
using TaskManager.Core.DTOs;
using TaskManager.Core.Models;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Service;
using TaskManager.Core.Interfaces.Repository;

namespace TaskManager.Infrastructure.Services
{

    public class ProjectService(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        ILogger<ProjectService> logger) : IProjectService
    {
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ILogger<ProjectService> _logger = logger;

        public async Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Buscando projetos para o usuário {UserId}", userId);

                var userExists = await _userRepository.ExistsAsync(userId);
                if (!userExists)
                {
                    _logger.LogWarning("Usuário não encontrado: {UserId}", userId);
                    throw new NotFoundException(nameof(User), userId);
                }

                var projects = await _projectRepository.GetUserProjectsAsync(userId);
                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar projetos do usuário {UserId}", userId);
                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(Guid projectId)
        {
            try
            {
                _logger.LogInformation("Buscando projeto {ProjectId}", projectId);

                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    _logger.LogWarning("Projeto não encontrado: {ProjectId}", projectId);
                    throw new NotFoundException(nameof(Project), projectId);
                }

                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar projeto {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<Project> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            try
            {
                _logger.LogInformation("Criando novo projeto para o usuário {UserId}", createProjectDto.OwnerId);

                var userExists = await _userRepository.ExistsAsync(createProjectDto.OwnerId);
                if (!userExists)
                {
                    _logger.LogWarning("Usuário não encontrado: {UserId}", createProjectDto.OwnerId);
                    throw new NotFoundException(nameof(User), createProjectDto.OwnerId);
                }

                var project = new Project
                {
                    Name = createProjectDto.Name,
                    OwnerId = createProjectDto.OwnerId,
                    Description = createProjectDto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                await _projectRepository.AddAsync(project);
                _logger.LogInformation("Projeto {ProjectId} criado com sucesso", project.Id);

                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar projeto");
                throw;
            }
        }

        public async Task<OperationResult> DeleteProjectAsync(Guid projectId)
        {
            try
            {
                _logger.LogInformation("Iniciando exclusão do projeto {ProjectId}", projectId);

                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    _logger.LogWarning("Projeto não encontrado: {ProjectId}", projectId);
                    return OperationResult.Fail("Projeto não encontrado", projectId); // Substituindo 'null' por 'new object()'
                }

                // Verifica se há tarefas pendentes
                var hasPendingTasks = await _taskRepository.HasPendingTasksAsync(projectId);
                if (hasPendingTasks)
                {
                    _logger.LogWarning("Tentativa de excluir projeto {ProjectId} com tarefas pendentes", projectId);
                    return OperationResult.Fail("Não é possível excluir um projeto com tarefas pendentes", new object()); // Substituindo 'null' por 'new object()'
                }

                await _projectRepository.DeleteAsync(project);
                _logger.LogInformation("Projeto {ProjectId} excluído com sucesso", projectId);

                return OperationResult.Ok(new object(), "Projeto excluído com sucesso"); // Substituindo 'null' por 'new object()'
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir projeto {ProjectId}", projectId);
                return OperationResult.Fail("Ocorreu um erro ao excluir o projeto", new object()); // Substituindo 'null' por 'new object()'
            }
        }

        public async Task<Project> UpdateProjectAsync(Guid projectId, UpdateProjectDto updateProjectDto)
        {
            try
            {
                _logger.LogInformation("Atualizando projeto {ProjectId}", projectId);

                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    _logger.LogWarning("Projeto não encontrado: {ProjectId}", projectId);
                    throw new NotFoundException(nameof(Project), projectId);
                }

                project.Name = updateProjectDto.Name ?? project.Name;

                await _projectRepository.UpdateAsync(project);
                _logger.LogInformation("Projeto {ProjectId} atualizado com sucesso", projectId);

                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar projeto {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<int> CountUserProjectsAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Contando projetos do usuário {UserId}", userId);

                var userExists = await _userRepository.ExistsAsync(userId);
                if (!userExists)
                {
                    _logger.LogWarning("Usuário não encontrado: {UserId}", userId);
                    throw new NotFoundException(nameof(User), userId);
                }

                return await _projectRepository.CountUserProjectsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar projetos do usuário {UserId}", userId);
                throw;
            }
        }
    }
}
