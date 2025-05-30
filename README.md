# Task Manager API

API para gerenciamento de projetos e tarefas.

## Requisitos

- .NET 8.0
- Docker 
- SQL Server 2017
- Visual Studio 2022 ou VSCode

## Execução com Docker

1. Clone o repositório
2. Execute: `docker-compose -f "docker-compose.yml" -f "docker-compose.override.yml" up`
3. A API estará disponível em `http://localhost:8080/swagger`
4. Crie um projeto com o Swagger
5. Inicie o processo de criar Tarefas, listar, deletar e ver o Relatorio em Reports

![image](https://github.com/user-attachments/assets/828a8bb6-dff6-4c2e-b016-c32acb655c64)

![image](https://github.com/user-attachments/assets/ccac2be0-ff70-46d4-a695-9ddf87f583b6)

## Usuário de Test

- `UserID: 3FA85F64-5717-4562-B3FC-2C963F66AFA6`

![image](https://github.com/user-attachments/assets/ab9d2e4b-c8df-45f9-98e6-23dd4a2001ea)

## Endpoints

- `GET /api/projects?userId={userId}` - Listar projetos do usuário
- `POST /api/projects` - Criar novo projeto
- `GET /api/projects/{projectId}/tasks` - Listar tarefas do projeto
- `POST /api/projects/{projectId}/tasks` - Criar nova tarefa
- `PUT /api/projects/{projectId}/tasks/{taskId}` - Atualizar tarefa
- `DELETE /api/projects/{projectId}/tasks/{taskId}` - Remover tarefa
- `GET /api/reports/performance?managerId={managerId}` - Relatório de desempenho
- `GET /api/reports/project-progress/{projectId}` - Relatório de desempenho
- `GET /api/reports/tasks-status` - Relatório de desempenho
- `GET /api/reports/user-tasks/{userId}` - Relatório de desempenho


## Fase 2: Perguntas para o PO

1. Qual é o volume esperado de usuários e tarefas?
2. Há necessidade de notificações quando tarefas são atualizadas?
3. Deveríamos permitir a atribuição de tarefas a múltiplos usuários?
4. Será necessário suporte a anexos em tarefas/comentários?
5. Há requisitos específicos para ordenação/filtragem de tarefas?
6. Precisamos de integração com outros sistemas (email, calendário, etc.)?

## Fase 3: Melhorias Futuras

1. Implementar autenticação/autorização robusta
2. Adicionar suporte a cache para melhor performance
3. Implementar padrão CQRS para separação de leitura/escrita
4. Adicionar suporte a eventos assíncronos com RabbitMQ/Kafka
5. Migrar para arquitetura de microsserviços quando escalar
6. Implementar CI/CD automatizado
7. Adicionar monitoramento com Application Insights
8. Suporte a múltiplos bancos de dados (poliglota)
9. Implementar testes de integração e e2e
10. Adicionar documentação com Swagger/OpenAPI
