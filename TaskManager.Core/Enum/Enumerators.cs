
namespace TaskManager.Core.Enum
{
    
    public enum TaskStatus
    {
        Pending,      // Tarefa pendente
        InProgress,  // Tarefa em andamento
        Completed    // Tarefa concluída
    }

    // TaskManager.Core/Enums/TaskPriority.cs
    public enum TaskPriority
    {
        Low,     // Prioridade baixa
        Medium,  // Prioridade média
        High     // Prioridade alta
    }

    
    public enum UserRole
    {
        Member,  // Usuário comum
        Manager  // Gerente (com acesso a relatórios)
    }

    
    public enum HistoryActionType
    {
        Created,     // Tarefa criada
        Updated,     // Tarefa atualizada
        StatusChanged, // Status alterado
        CommentAdded,  // Comentário adicionado
        Deleted      // Tarefa removida
    }
}
