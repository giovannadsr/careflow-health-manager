const fs = require('fs');

const readme = `# CareFlow Health Manager

<div align="center">

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI_3.0-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-orange?style=for-the-badge)
![xUnit](https://img.shields.io/badge/Tests-xUnit_+_Moq-512BD4?style=for-the-badge)

**Sistema de gestão de saúde com API RESTful, autenticação JWT, Clean Architecture e banco de dados PostgreSQL em Docker.**

</div>

---

## Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Entidades do Domínio](#entidades-do-domínio)
- [Funcionalidades e Endpoints](#funcionalidades-e-endpoints)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Documentação da API (Swagger)](#documentação-da-api-swagger)
- [Autenticação](#autenticação)
- [Testes](#testes)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [Docker](#docker)
- [Regras de Negócio](#regras-de-negócio)
- [Tratamento de Erros](#tratamento-de-erros)

---

## Sobre o Projeto

O **CareFlow Health Manager** é um sistema de gerenciamento de saúde desenvolvido para clínicas e consultórios médicos. Centraliza o gerenciamento de pacientes, médicos e agendamentos de consultas por meio de uma API RESTful segura e bem documentada.

### Principais objetivos
- Digitalizar e centralizar registros de pacientes e médicos
- Validar conflitos de agendamento automaticamente (janela de 30 minutos)
- Garantir segurança de dados com autenticação JWT e controle de acesso baseado em papéis (RBAC)
- Fornecer rastreabilidade de consultas por fluxo de status

---

## Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| **Backend** | C# com .NET 10 |
| **ORM** | Entity Framework Core 10 (Npgsql) |
| **Banco de Dados** | PostgreSQL 16 (Docker) |
| **Autenticação** | JWT Bearer + BCrypt.Net-Next v4.2 |
| **Documentação** | Swagger / OpenAPI 3.0 (Swashbuckle 6.6) |
| **Testes** | xUnit 2.9 + Moq 4.20 |
| **Containerização** | Docker + Docker Compose |
| **Arquitetura** | Clean Architecture |

---

## Arquitetura

O projeto segue o padrão **Clean Architecture** com separação clara de responsabilidades:

\`\`\`
CareFlow/
├── src/
│   ├── CareFlow.Domain/          # Entidades, enums, exceções de domínio
│   ├── CareFlow.Application/     # Interfaces, DTOs
│   ├── CareFlow.Infrastructure/  # EF Core, serviços, migrations
│   └── CareFlow.Api/             # Controllers, Swagger, middleware, DI
└── tests/
    └── CareFlow.Tests/           # Testes unitários (xUnit + Moq)
\`\`\`

**Regra de dependência:** \`Api → Application ← Infrastructure → Domain\`

### Camadas

**\`CareFlow.Domain\`**
- Entidades: \`Patient\`, \`Doctor\`, \`Appointment\`, \`User\`, \`TaskItem\`
- Base: \`BaseEntity\` (Id + CreatedAt)
- Enums: \`AppointmentStatus\`, \`UserRole\`, \`TaskItemStatus\`, \`TaskPriority\`
- Exceções de domínio: \`NotFoundException\`, \`ConflictException\`, \`ValidationException\`

**\`CareFlow.Application\`**
- Interfaces de serviço: \`IAuthService\`, \`IPatientService\`, \`IDoctorService\`, \`IAppointmentService\`
- DTOs de request/response para todas as entidades principais

**\`CareFlow.Infrastructure\`**
- \`AppDbContext\` (Entity Framework Core com Npgsql)
- Implementações: \`AuthService\`, \`PatientService\`, \`DoctorService\`, \`AppointmentService\`
- Migrations EF Core (5 migrations: InitialCreate → UpdatePatientFields → AddDoctors → AppointmentStatusEnum → AddUserRole)

**\`CareFlow.Api\`**
- Controllers: \`AuthController\`, \`PatientsController\`, \`DoctorsController\`, \`AppointmentsController\`
- Middleware global de exceções: \`ExceptionMiddleware\`
- Configuração JWT + Swagger com autenticação Bearer

---

## Entidades do Domínio

### Patient
| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK |
| FullName | string | Nome completo |
| BirthDate | DateTime | Data de nascimento |
| Gender | string | Gênero |
| CPF | string | Único (índice único no banco) |
| MedicalRecordNumber | string | Número do prontuário |
| Diagnosis | string | Diagnóstico |
| PhoneNumber | string | Telefone |
| EmergencyContact | string | Contato de emergência |
| CreatedAt | DateTime | Data de criação (UTC) |

### Doctor
| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK (gerado no construtor) |
| FullName | string | Nome completo |
| CRM | string | Único (índice único no banco) |
| Specialty | string | Especialidade |
| IsActive | bool | Ativo (default: true) |

### Appointment
| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK |
| PatientId | Guid | FK → Patient |
| DoctorId | Guid | FK → Doctor |
| AppointmentDate | DateTime | Data/hora da consulta |
| Status | AppointmentStatus | Scheduled (default) |
| Notes | string? | Observações (nullable) |

### User
| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK (BaseEntity) |
| Name | string | Nome |
| Email | string | Único (índice único no banco) |
| PasswordHash | string | Hash BCrypt |
| Role | UserRole | User (default) |
| Tasks | ICollection\\<TaskItem\\> | Tarefas atribuídas |
| CreatedAt | DateTime | BaseEntity |

### AppointmentStatus (enum)
\`Scheduled → Confirmed → InProgress → Completed | Cancelled | NoShow\`

### UserRole (enum)
\`User (0) | Admin (1) | Doctor (2) | Recepcionist (3)\`

---

## Funcionalidades e Endpoints

### Autenticação — \`/api/auth\`

| Método | Rota | Acesso | Descrição |
|--------|------|--------|-----------|
| POST | \`/api/auth/register\` | Público | Registra novo usuário (role padrão: User) |
| POST | \`/api/auth/login\` | Público | Login — retorna JWT |

### Pacientes — \`/api/patients\`

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| GET | \`/api/patients\` | Admin, Doctor, Recepcionist | Listar todos |
| GET | \`/api/patients/{id}\` | Admin, Doctor, Recepcionist | Buscar por ID |
| POST | \`/api/patients\` | Admin, Recepcionist | Cadastrar |
| PUT | \`/api/patients/{id}\` | Admin, Recepcionist | Atualizar (FullName, Diagnosis) |
| DELETE | \`/api/patients/{id}\` | Admin | Remover |

### Médicos — \`/api/doctors\`

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| GET | \`/api/doctors\` | Admin, Recepcionist | Listar todos |
| GET | \`/api/doctors/{id}\` | Admin, Recepcionist | Buscar por ID |
| POST | \`/api/doctors\` | Admin | Cadastrar (CRM único) |
| PUT | \`/api/doctors/{id}\` | Admin | Atualizar (FullName, Specialty, IsActive) |
| DELETE | \`/api/doctors/{id}\` | Admin | Remover |

### Consultas — \`/api/appointments\`

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| GET | \`/api/appointments\` | Admin, Doctor, Recepcionist | Listar todas |
| GET | \`/api/appointments/{id}\` | Admin, Doctor, Recepcionist | Buscar por ID |
| POST | \`/api/appointments\` | Admin, Recepcionist | Agendar (valida conflito e antecedência) |
| PUT | \`/api/appointments/{id}\` | Admin, Recepcionist | Atualizar data/notas |
| PATCH | \`/api/appointments/{id}/confirm\` | Admin, Recepcionist | Confirmar (Scheduled → Confirmed) |
| PATCH | \`/api/appointments/{id}/start\` | Admin, Doctor | Iniciar (Confirmed → InProgress) |
| PATCH | \`/api/appointments/{id}/complete\` | Admin, Doctor | Concluir (InProgress → Completed) |
| PATCH | \`/api/appointments/{id}/cancel\` | Admin, Recepcionist | Cancelar (min. 2h de antecedência) |
| DELETE | \`/api/appointments/{id}\` | Admin | Remover permanentemente |

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

---

## Instalação e Execução

### 1. Clone o repositório

\`\`\`bash
git clone https://github.com/giovannadsr/careflow-health-manager.git
cd careflow-health-manager
\`\`\`

### 2. Inicie o banco de dados PostgreSQL via Docker

\`\`\`bash
docker-compose up -d
\`\`\`

Sobe um container **PostgreSQL 16** na porta **5440** (mapeado para 5432 interno).

Verifique:
\`\`\`bash
docker ps
# careflow-postgres   Up
\`\`\`

### 3. Aplique as migrations

\`\`\`bash
dotnet ef database update --project src/CareFlow.Infrastructure --startup-project src/CareFlow.Api
\`\`\`

### 4. Execute a aplicação

\`\`\`bash
dotnet run --project src/CareFlow.Api
\`\`\`

A API estará em: **http://localhost:5108** (configuração padrão do launchSettings).

---

## Documentação da API (Swagger)

Com a aplicação rodando, acesse:

\`\`\`
http://localhost:5108/swagger
\`\`\`

### Autenticando no Swagger

1. \`POST /api/auth/register\` — crie um usuário
2. \`POST /api/auth/login\` — faça login e copie o \`token\`
3. Clique em **Authorize** e insira: \`Bearer {seu_token}\`

---

## Autenticação

O sistema usa **JWT Bearer Token**. Todas as rotas exceto \`/api/auth/register\` e \`/api/auth/login\` requerem autenticação.

### Exemplo de registro

\`\`\`json
POST /api/auth/register
{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "senha123"
}
\`\`\`

### Exemplo de login

\`\`\`json
POST /api/auth/login
{
  "email": "joao@example.com",
  "password": "senha123"
}
\`\`\`

**Resposta:**
\`\`\`json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-06-07T20:00:00Z"
}
\`\`\`

O token expira em **60 minutos** (configurável em \`Jwt:ExpiresInMinutes\`).

### Usando o token nas requisições

\`\`\`http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
\`\`\`

---

## Testes

\`\`\`bash
dotnet test
\`\`\`

Com cobertura:
\`\`\`bash
dotnet test --collect:"XPlat Code Coverage"
\`\`\`

### Casos de teste implementados

| # | Arquivo | Teste | Cenário |
|---|---------|-------|---------|
| 1 | \`Auth/AuthServiceTests.cs\` | \`Login_WithValidCredentials_ShouldReturnToken\` | Token não pode ser nulo/vazio |
| 2 | \`Patients/PatientServiceTests.cs\` | \`CreatePatient_WithDuplicateCpf_ShouldThrowException\` | CPF duplicado lança exceção |
| 3 | \`Appointments/AppointmentServiceTests.cs\` | \`ScheduleAppointment_WithLessThanOneHour_ShouldThrowException\` | Agendamento < 1h lança exceção |
| 4 | \`Appointments/AppointmentServiceTests.cs\` | \`ScheduleAppointment_WithConflict_ShouldThrowException\` | Conflito de horário lança exceção |
| 5 | \`Appointments/AppointmentServiceTests.cs\` | \`ConfirmAppointment_ShouldChangeStatus\` | Confirmação altera status para Confirmed |

---

## Estrutura do Projeto

\`\`\`
careflow-health-manager/
│
├── src/
│   ├── CareFlow.Api/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── PatientsController.cs
│   │   │   ├── DoctorsController.cs
│   │   │   └── AppointmentsController.cs
│   │   ├── Middleware/
│   │   │   └── ExceptionMiddleware.cs
│   │   ├── appsettings.json
│   │   └── Program.cs
│   │
│   ├── CareFlow.Application/
│   │   ├── DTOs/
│   │   │   ├── Auth/         (LoginRequestDto, RegisterRequestDto)
│   │   │   ├── Appointments/ (CreateAppointmentDto, UpdateAppointmentDto, AppointmentResponseDto)
│   │   │   ├── Doctors/      (CreateDoctorDto, UpdateDoctorDto, DoctorResponseDto)
│   │   │   └── (Patients)    (CreatePatientDto, UpdatePatientDto, PatientResponseDto)
│   │   └── Interfaces/
│   │       ├── IAuthService.cs
│   │       ├── IPatientService.cs
│   │       ├── IDoctorService.cs
│   │       └── IAppointmentService.cs
│   │
│   ├── CareFlow.Domain/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs
│   │   ├── Entities/
│   │   │   ├── Patient.cs
│   │   │   ├── Doctor.cs
│   │   │   ├── Appointment.cs
│   │   │   ├── User.cs
│   │   │   └── TaskItem.cs
│   │   ├── Enums/
│   │   │   ├── AppointmentStatus.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── TaskItemStatus.cs
│   │   │   └── TaskPriority.cs
│   │   └── Exceptions/
│   │       ├── ConflictException.cs
│   │       ├── NotFoundException.cs
│   │       └── ValidationException.cs
│   │
│   └── CareFlow.Infrastructure/
│       ├── Persistence/
│       │   └── AppDbContext.cs
│       ├── Services/
│       │   ├── AuthService.cs
│       │   ├── PatientService.cs
│       │   ├── DoctorService.cs
│       │   └── AppointmentService.cs
│       └── Migrations/
│           ├── 20260526124908_InitialCreate
│           ├── 20260527022009_UpdatePatientFields
│           ├── 20260530185659_AddDoctors
│           ├── 20260531025522_AppointmentStatusEnum
│           └── 20260607033522_AddUserRole
│
├── tests/
│   └── CareFlow.Tests/
│       ├── Auth/
│       │   └── AuthServiceTests.cs
│       ├── Appointments/
│       │   └── AppointmentServiceTests.cs
│       └── Patients/
│           └── PatientServiceTests.cs
│
├── docker-compose.yml
└── README.md
\`\`\`

---

## Variáveis de Ambiente

Configure em \`src/CareFlow.Api/appsettings.json\`:

\`\`\`json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5440;Database=careflowdb;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "careflow-super-secret-key-123456789",
    "Issuer": "CareFlow.Api",
    "Audience": "CareFlow.Client",
    "ExpiresInMinutes": 60
  }
}
\`\`\`

> **Em produção**, nunca commite a chave JWT no repositório. Use variáveis de ambiente ou um serviço de secrets.

---

## Docker

\`\`\`bash
# Iniciar banco de dados
docker-compose up -d

# Parar
docker-compose down

# Parar e remover volumes (apaga dados)
docker-compose down -v

# Ver logs
docker logs careflow-postgres
\`\`\`

O \`docker-compose.yml\` configura:
- **PostgreSQL 16** na porta \`5440\` → \`5432\` (interno)
- Volume persistente \`postgres_data\`
- Credenciais: usuário \`postgres\`, senha \`postgres\`, database \`careflowdb\`

---

## Regras de Negócio

| Código | Regra | Implementação |
|--------|-------|---------------|
| RN-001 | CPF do paciente deve ser único | Índice único em \`AppDbContext\` + exceção de banco |
| RN-002 | CRM do médico deve ser único | Índice único em \`AppDbContext\` |
| RN-003 | E-mail do usuário deve ser único | Índice único em \`AppDbContext\` + verificação em \`AuthService\` |
| RN-004 | Consultas: janela de 30 min entre consultas do mesmo médico | \`AppointmentService.CreateAsync\` → \`ConflictException\` |
| RN-005 | Agendamento com no mínimo 1 hora de antecedência | \`AppointmentService.CreateAsync\` → \`ValidationException\` |
| RN-006 | Cancelamento com no mínimo 2 horas de antecedência | \`AppointmentService.CancelAsync\` → \`ValidationException\` |
| RN-007 | Fluxo de status: Scheduled → Confirmed → InProgress → Completed/Cancelled | \`ConfirmAsync\`, \`StartAsync\`, \`CompleteAsync\`, \`CancelAsync\` |
| RN-008 | Confirmação exige status Scheduled | \`ConfirmAsync\` → \`ValidationException\` |
| RN-009 | Início exige status Confirmed | \`StartAsync\` → \`ValidationException\` |
| RN-010 | Conclusão exige status InProgress | \`CompleteAsync\` → \`ValidationException\` |
| RN-011 | Senhas armazenadas com hash BCrypt | \`AuthService.RegisterAsync\` |
| RN-012 | RBAC por roles: Admin / Doctor / Recepcionist / User | \`[Authorize(Roles = "...")]\` nos controllers |

---

## Tratamento de Erros

Todas as exceções são capturadas pelo \`ExceptionMiddleware\` e retornam JSON padronizado:

\`\`\`json
{
  "status": 409,
  "message": "Já existe uma consulta agendada para este médico neste horário."
}
\`\`\`

| Exceção | HTTP Status |
|---------|-------------|
| \`NotFoundException\` | 404 Not Found |
| \`ValidationException\` | 400 Bad Request |
| \`ConflictException\` | 409 Conflict |
| Não tratada | 500 Internal Server Error |

---

## Contribuição

1. Fork o projeto
2. \`git checkout -b feature/nova-funcionalidade\`
3. \`git commit -m 'feat: descrição'\`
4. \`git push origin feature/nova-funcionalidade\`
5. Abra um Pull Request

---

## Licença

MIT. Veja [LICENSE](LICENSE) para detalhes.

---

<div align="center">
  Desenvolvido com ❤️ — CareFlow Health Manager<br>
  <strong>C# .NET 10 | Clean Architecture | PostgreSQL | Docker</strong>
</div>
`;

fs.writeFileSync('/mnt/user-data/outputs/README.md', readme);
console.log('README written');
