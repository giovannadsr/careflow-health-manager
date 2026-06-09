# CareFlow Health Manager

Sistema de gerenciamento de pacientes, médicos e consultas desenvolvido em **.NET 10**, seguindo princípios de arquitetura em camadas (**API, Application, Domain e Infrastructure**) e aplicando conceitos de **Clean Architecture**, **DDD**, autenticação **JWT**, controle de acesso por perfis (**Roles**) e persistência com **PostgreSQL**.

---

# Objetivo do Projeto

O CareFlow foi desenvolvido para apoiar o gerenciamento de atendimentos em ambientes de saúde, permitindo:

- Cadastro e gerenciamento de pacientes
- Cadastro e gerenciamento de médicos
- Agendamento de consultas
- Controle do ciclo de vida das consultas
- Autenticação e autorização de usuários
- Controle de acesso baseado em papéis (Roles)

---

# Tecnologias Utilizadas

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- BCrypt
- Swagger / OpenAPI
- xUnit
- Docker

---

# Arquitetura do Projeto

O sistema foi organizado em camadas para garantir separação de responsabilidades e facilidade de manutenção.

```text
CareFlow.HealthManager
│
├── src
│   │
│   ├── CareFlow.Api
│   │   ├── Controllers
│   │   ├── Middleware
│   │   └── Program.cs
│   │
│   ├── CareFlow.Application
│   │   ├── DTOs
│   │   └── Interfaces
│   │
│   ├── CareFlow.Domain
│   │   ├── Entities
│   │   ├── Enums
│   │   ├── Interfaces
│   │   └── Exceptions
│   │
│   └── CareFlow.Infrastructure
│       ├── Persistence
│       ├── Repositories
│       ├── Services
│       └── Migrations
│
└── tests
    └── CareFlow.Tests
```

---

# Funcionalidades Implementadas

## Autenticação

- Registro de usuários
- Login
- Geração de Token JWT
- Senhas criptografadas com BCrypt

### Endpoints

```http
POST /api/Auth/register
POST /api/Auth/login
```

---

## Controle de Acesso (Roles)

O sistema possui autorização baseada em perfis.

### Roles disponíveis

| Role | Permissão |
|--------|------------|
| Admin | Acesso total |
| Doctor | Operações médicas |
| Receptionist | Agendamentos |

### Exemplo

```csharp
[Authorize(Roles = "Admin")]
```

```csharp
[Authorize(Roles = "Admin,Doctor")]
```

---

# Módulo de Pacientes

Permite o gerenciamento completo dos pacientes.

## Campos

| Campo | Tipo |
|---------|--------|
| FullName | string |
| BirthDate | DateTime |
| Gender | string |
| CPF | string |
| MedicalRecordNumber | string |
| Diagnosis | string |
| PhoneNumber | string |
| EmergencyContact | string |

## Endpoints

```http
GET    /api/Patients
GET    /api/Patients/{id}
POST   /api/Patients
PUT    /api/Patients/{id}
DELETE /api/Patients/{id}
```

---

# Módulo de Médicos

Permite o gerenciamento dos profissionais da saúde.

## Campos

| Campo | Tipo |
|---------|--------|
| FullName | string |
| CRM | string |
| Specialty | string |
| IsActive | bool |

## Endpoints

```http
GET    /api/Doctors
GET    /api/Doctors/{id}
POST   /api/Doctors
PUT    /api/Doctors/{id}
DELETE /api/Doctors/{id}
```

---

# Módulo de Consultas

Permite o gerenciamento dos atendimentos entre pacientes e médicos.

## Campos

| Campo | Tipo |
|---------|--------|
| PatientId | Guid |
| DoctorId | Guid |
| AppointmentDate | DateTime |
| Status | AppointmentStatus |
| Notes | string |

---

## Status da Consulta

```csharp
public enum AppointmentStatus
{
    Scheduled = 0,
    Confirmed = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}
```

---

## Fluxo de Estados

```text
Scheduled
    ↓
Confirmed
    ↓
InProgress
    ↓
Completed

Scheduled
    ↓
Cancelled

Confirmed
    ↓
Cancelled
```

---

## Regras de Negócio Implementadas

### RN01 - Paciente deve existir

Não é permitido criar consulta para paciente inexistente.

---

### RN02 - Médico deve existir

Não é permitido criar consulta para médico inexistente.

---

### RN03 - Conflito de Horário

Um médico não pode possuir duas consultas no mesmo intervalo.

---

### RN04 - Antecedência mínima

Consultas devem ser agendadas com pelo menos 1 hora de antecedência.

---

### RN05 - Controle de Status

Somente transições válidas são permitidas.

Exemplo:

```text
Scheduled -> Confirmed
Confirmed -> InProgress
InProgress -> Completed
```

---

## Endpoints

```http
GET    /api/Appointments
GET    /api/Appointments/{id}

POST   /api/Appointments

PUT    /api/Appointments/{id}

DELETE /api/Appointments/{id}

PATCH  /api/Appointments/{id}/confirm
PATCH  /api/Appointments/{id}/start
PATCH  /api/Appointments/{id}/complete
PATCH  /api/Appointments/{id}/cancel
```

---

# Tratamento Global de Exceções

Foi implementado um middleware centralizado para tratamento de erros.

## Exceções tratadas

### NotFoundException

```http
404 Not Found
```

Exemplo:

```json
{
  "status": 404,
  "message": "Paciente não encontrado."
}
```

---

### ValidationException

```http
400 Bad Request
```

Exemplo:

```json
{
  "status": 400,
  "message": "Consultas devem ser agendadas com pelo menos 1 hora de antecedência."
}
```

---

### ConflictException

```http
409 Conflict
```

Exemplo:

```json
{
  "status": 409,
  "message": "Já existe uma consulta agendada para este médico neste horário."
}
```

---

### Erros inesperados

```http
500 Internal Server Error
```

Exemplo:

```json
{
  "status": 500,
  "message": "Ocorreu um erro interno."
}
```

---

# Testes Automatizados

O projeto utiliza **xUnit**.

## Cenários cobertos

### Pacientes

- Buscar paciente inexistente
- Exclusão de paciente inexistente

### Consultas

- Impedir criação com horário conflitante
- Impedir transição inválida de status

### Execução

```bash
dotnet test
```

---

# Banco de Dados

## PostgreSQL via Docker

Criar container:

```bash
docker run --name careflow-postgres \
-e POSTGRES_USER=postgres \
-e POSTGRES_PASSWORD=postgres \
-e POSTGRES_DB=careflowdb \
-p 5440:5432 \
-d postgres
```

Iniciar container:

```bash
docker start careflow-postgres
```

Verificar status:

```bash
docker ps
```

---

# Migrations

Criar migration:

```bash
dotnet ef migrations add NomeDaMigration \
--project src/CareFlow.Infrastructure \
--startup-project src/CareFlow.Api
```

Aplicar migration:

```bash
dotnet ef database update \
--project src/CareFlow.Infrastructure \
--startup-project src/CareFlow.Api
```

---

# Executando o Projeto

## Restaurar dependências

```bash
dotnet restore
```

## Compilar

```bash
dotnet build
```

## Executar API

```bash
dotnet run --project src/CareFlow.Api
```

---

# Swagger

Após iniciar a API:

```text
http://localhost:5108/swagger
```

---

# Repositório

(https://github.com/giovannadsr/careflow-health-manager)

---

# Melhorias Futuras

- Soft Delete para pacientes
- Policies baseadas em Claims
- Refresh Token
- Logs estruturados
- Integração com FluentValidation
- Testes com InMemoryDatabase
- Cobertura de testes ampliada
- CI/CD com GitHub Actions
- Deploy em nuvem
- Documentação OpenAPI mais completa

---

# Autor

**Giovanna Dias Rodrigues**

Projeto desenvolvido para estudo de arquitetura de software, APIs REST, autenticação JWT, DDD e boas práticas com .NET.
