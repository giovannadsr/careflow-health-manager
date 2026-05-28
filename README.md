#  CareFlow Health Manager

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-11.0-239120?style=for-the-badge&logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI_3.0-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-orange?style=for-the-badge)

**Sistema de gestão de saúde com API RESTful, autenticação JWT, Clean Architecture e banco de dados PostgreSQL em Docker.**

</div>

---

##  Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Documentação da API (Swagger)](#documentação-da-api-swagger)
- [Endpoints Principais](#endpoints-principais)
- [Autenticação](#autenticação)
- [Testes](#testes)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [Docker](#docker)
- [Regras de Negócio](#regras-de-negócio)
- [Tratamento de Erros](#tratamento-de-erros)

---

##  Sobre o Projeto

O **CareFlow Health Manager** é um sistema de gerenciamento de saúde desenvolvido para clínicas e consultórios médicos. Centraliza o gerenciamento de pacientes, médicos, agendamentos de consultas e prontuários eletrônicos por meio de uma API RESTful segura e bem documentada.

### Principais objetivos:
- Digitalizar e centralizar registros de saúde
- Eliminar conflitos de agendamento com validações automáticas
- Garantir segurança de dados com autenticação JWT e controle de acesso baseado em papéis (RBAC)
- Conformidade com a LGPD — dados sensíveis tratados adequadamente

---

##  Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| **Backend** | C# 11 com .NET 8 |
| **ORM** | Entity Framework Core 8 |
| **Banco de Dados** | PostgreSQL 16 (Docker) |
| **Autenticação** | JWT Bearer + BCrypt |
| **Documentação** | Swagger / OpenAPI 3.0 |
| **Testes** | xUnit + Moq |
| **Containerização** | Docker + Docker Compose |
| **Arquitetura** | Clean Architecture |

---

##  Arquitetura

O projeto segue o padrão **Clean Architecture** com separação clara de responsabilidades:

```
CareFlow/
├── src/
│   ├── CareFlow.Domain/          # Entidades, interfaces, regras de negócio
│   ├── CareFlow.Application/     # Casos de uso, DTOs, serviços, validadores
│   ├── CareFlow.Infrastructure/  # EF Core, repositórios, migrations
│   └── CareFlow.Api/             # Controllers, DI, Swagger, middlewares
└── tests/
    └── CareFlow.Tests/           # Testes unitários e de integração (xUnit)
```

**Regra de dependência:** as dependências sempre apontam para dentro — `Api → Application → Domain ← Infrastructure`.

### Detalhamento das camadas:

**`CareFlow.Domain`**
- Entidades: `Patient`, `Doctor`, `Appointment`, `MedicalRecord`, `User`
- Interfaces: `IPatientRepository`, `IDoctorRepository`, `IAppointmentRepository`, etc.
- Regras de negócio puras (sem dependências externas)
- Enumerações: `AppointmentStatus`, `UserRole`

**`CareFlow.Application`**
- Casos de uso e serviços de aplicação
- DTOs (Request/Response)
- Validadores (FluentValidation)
- Mapeamentos (AutoMapper)

**`CareFlow.Infrastructure`**
- `CareFlowDbContext` (Entity Framework Core)
- Implementações de repositórios
- Migrations
- Configurações de entidades (Fluent API)

**`CareFlow.Api`**
- Controllers REST
- Configuração de injeção de dependência
- Swagger/OpenAPI
- Middleware global de tratamento de erros
- Autenticação/autorização JWT

---

##  Funcionalidades

###  Autenticação
- Login com e-mail e senha
- Token JWT com expiração de 24 horas
- Refresh de token
- Senhas com hash BCrypt (fator 12)

###  Gestão de Pacientes (CRUD completo)
- Cadastro com CPF único
- Busca por ID, CPF ou nome
- Listagem paginada com filtros
- Atualização de dados
- Soft delete (inativação)

###  Gestão de Médicos (CRUD completo)
- Cadastro com CRM único
- Filtro por especialidade
- Gerenciamento de disponibilidade

###  Agendamento de Consultas (CRUD completo)
- Validação automática de conflitos de horário
- Intervalo mínimo de 30 minutos entre consultas
- Agendamento com antecedência mínima de 1 hora
- Cancelamento com justificativa obrigatória
- Fluxo de status: `Scheduled → Confirmed → InProgress → Completed | Cancelled | NoShow`

###  Prontuário Eletrônico (CRUD completo)
- Registro vinculado à consulta
- Imutabilidade garantida (sem delete)
- Correções via novo registro referenciado
- Histórico completo por paciente

###  Controle de Acesso (RBAC)
| Recurso | Admin | Médico | Recepcionista |
|---------|-------|--------|---------------|
| Gerenciar Usuários | ✅ | ❌ | ❌ |
| CRUD Pacientes | ✅ | 🔍 | ✅ |
| CRUD Médicos | ✅ | 🔍 | 🔍 |
| CRUD Consultas | ✅ | ✅ | ✅ |
| Prontuário (escrita) | ❌ | ✅ | ❌ |
| Prontuário (leitura) | ✅ | ✅ | ❌ |

---

##  Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (para o banco de dados)
- [Git](https://git-scm.com/)

---

##  Instalação e Execução

### 1. Clone o repositório

```bash
git clone https://github.com/giovannadsr/careflow-health-manager.git
cd careflow-health-manager
```

### 2. Inicie o banco de dados PostgreSQL via Docker

```bash
docker-compose up -d
```

Isso sobe um container PostgreSQL 16 na porta **5440**.

Verifique que está rodando:

```bash
docker ps
# Deve mostrar: careflow-postgres (running)
```

### 3. Aplique as migrations do banco de dados

```bash
dotnet ef database update --project src/CareFlow.Infrastructure --startup-project src/CareFlow.Api
```

### 4. Execute a aplicação

```bash
dotnet run --project src/CareFlow.Api
```

A API estará disponível em: **http://localhost:5000**

---

##  Documentação da API (Swagger)

Com a aplicação rodando, acesse:

```
http://localhost:5108/swagger
```

O Swagger UI permite:
- Visualizar todos os endpoints documentados
- Testar requisições diretamente no browser
- Autenticar com token JWT (botão **Authorize**)
- Ver modelos de request/response com exemplos

### Autenticando no Swagger:
1. Use `POST /api/v1/auth/login` com as credenciais
2. Copie o `token` da resposta
3. Clique em **Authorize** no Swagger
4. Insira: `Bearer {seu_token}`

---

##  Endpoints Principais

### Autenticação
```
POST   /api/v1/auth/login          # Login — retorna JWT
POST   /api/v1/auth/refresh-token  # Renova o token
```

### Pacientes
```
GET    /api/v1/patients            # Listar (paginado, com filtros)
GET    /api/v1/patients/{id}       # Buscar por ID
GET    /api/v1/patients/cpf/{cpf}  # Buscar por CPF
POST   /api/v1/patients            # Cadastrar paciente
PUT    /api/v1/patients/{id}       # Atualizar dados
DELETE /api/v1/patients/{id}       # Inativar (soft delete)
```

### Médicos
```
GET    /api/v1/doctors             # Listar (filtro por especialidade)
GET    /api/v1/doctors/{id}        # Buscar por ID
POST   /api/v1/doctors             # Cadastrar médico
PUT    /api/v1/doctors/{id}        # Atualizar dados
DELETE /api/v1/doctors/{id}        # Inativar
```

### Consultas
```
GET    /api/v1/appointments              # Listar consultas (filtros: data, médico, status)
GET    /api/v1/appointments/{id}         # Buscar consulta
GET    /api/v1/appointments/doctor/{id}  # Agenda do médico
GET    /api/v1/appointments/patient/{id} # Histórico do paciente
POST   /api/v1/appointments              # Agendar consulta
PUT    /api/v1/appointments/{id}         # Atualizar dados
PATCH  /api/v1/appointments/{id}/cancel  # Cancelar consulta
PATCH  /api/v1/appointments/{id}/confirm # Confirmar consulta
PATCH  /api/v1/appointments/{id}/complete# Concluir consulta
```

### Prontuário
```
GET    /api/v1/medical-records/patient/{id}     # Histórico do paciente
GET    /api/v1/medical-records/{id}             # Buscar registro
POST   /api/v1/medical-records                  # Criar registro
POST   /api/v1/medical-records/{id}/correction  # Criar correção
```

---

##  Autenticação

O sistema usa **JWT Bearer Token**. Todas as rotas exceto `/api/v1/auth/login` requerem o token.

### Exemplo de login:

```json
POST /api/v1/auth/login
{
  "email": "admin@careflow.com",
  "password": "Admin@123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-05-28T14:00:00Z",
  "user": {
    "id": "...",
    "email": "admin@careflow.com",
    "role": "Admin"
  }
}
```

### Usando o token:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

##  Testes

Execute todos os testes:

```bash
dotnet test
```

Execute com relatório de cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Casos de teste implementados (≥5):

| # | Caso de Teste | Camada |
|---|---------------|--------|
| 1 | `LoginWithValidCredentials_ReturnsJwtToken` | Application |
| 2 | `LoginWithInvalidPassword_ThrowsUnauthorizedException` | Application |
| 3 | `CreatePatient_WithDuplicateCPF_ThrowsConflictException` | Domain |
| 4 | `ScheduleAppointment_WithConflict_ThrowsConflictException` | Domain |
| 5 | `ScheduleAppointment_WithLessThan1HourNotice_ThrowsValidationException` | Domain |
| 6 | `CancelAppointment_WithJustification_UpdatesStatus` | Application |
| 7 | `MedicalRecord_CannotBeDeleted_ThrowsException` | Domain |
| 8 | `GetPatientById_WithInvalidId_Returns404` | Api (Integration) |

---

##  Estrutura do Projeto

```
careflow-health-manager/
│
├── src/
│   ├── CareFlow.Api/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── PatientsController.cs
│   │   │   ├── DoctorsController.cs
│   │   │   ├── AppointmentsController.cs
│   │   │   └── MedicalRecordsController.cs
│   │   ├── Middlewares/
│   │   │   └── GlobalExceptionMiddleware.cs
│   │   ├── Extensions/
│   │   │   ├── SwaggerExtensions.cs
│   │   │   └── AuthExtensions.cs
│   │   └── Program.cs
│   │
│   ├── CareFlow.Application/
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── PatientService.cs
│   │   │   ├── DoctorService.cs
│   │   │   ├── AppointmentService.cs
│   │   │   └── MedicalRecordService.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Patients/
│   │   │   ├── Doctors/
│   │   │   ├── Appointments/
│   │   │   └── MedicalRecords/
│   │   └── Validators/
│   │
│   ├── CareFlow.Domain/
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── Patient.cs
│   │   │   ├── Doctor.cs
│   │   │   ├── Appointment.cs
│   │   │   ├── MedicalRecord.cs
│   │   │   └── User.cs
│   │   ├── Enums/
│   │   │   ├── AppointmentStatus.cs
│   │   │   └── UserRole.cs
│   │   ├── Exceptions/
│   │   │   ├── ConflictException.cs
│   │   │   ├── NotFoundException.cs
│   │   │   └── ValidationException.cs
│   │   └── Interfaces/
│   │       ├── IPatientRepository.cs
│   │       ├── IDoctorRepository.cs
│   │       ├── IAppointmentRepository.cs
│   │       └── IMedicalRecordRepository.cs
│   │
│   └── CareFlow.Infrastructure/
│       ├── Data/
│       │   ├── CareFlowDbContext.cs
│       │   └── Configurations/
│       ├── Repositories/
│       │   ├── PatientRepository.cs
│       │   ├── DoctorRepository.cs
│       │   ├── AppointmentRepository.cs
│       │   └── MedicalRecordRepository.cs
│       └── Migrations/
│
├── tests/
│   └── CareFlow.Tests/
│       ├── Application/
│       ├── Domain/
│       └── Integration/
│
├── docker-compose.yml
├── CareFlow.slnx
└── README.md
```

---

##  Variáveis de Ambiente

Configure em `src/CareFlow.Api/appsettings.json` ou via variáveis de ambiente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5440;Database=careflowdb;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-com-no-minimo-32-caracteres",
    "Issuer": "CareFlow",
    "Audience": "CareFlowUsers",
    "ExpirationHours": 24
  }
}
```

>  **Em produção**, nunca commite a chave JWT no repositório. Use variáveis de ambiente ou um serviço de secrets.

---

##  Docker

### Iniciar banco de dados:
```bash
docker-compose up -d
```

### Parar serviços:
```bash
docker-compose down
```

### Parar e remover volumes (cuidado: apaga os dados):
```bash
docker-compose down -v
```

### Ver logs do PostgreSQL:
```bash
docker logs careflow-postgres
```

O `docker-compose.yml` configura:
- **PostgreSQL 16** na porta `5440` (local) → `5432` (container)
- Volume persistente `postgres_data`
- Usuário: `postgres`, Senha: `postgres`, Database: `careflowdb`

---

##  Regras de Negócio

| Regra | Descrição |
|-------|-----------|
| **RN-001** | CPF do paciente deve ser único |
| **RN-002** | CRM do médico deve ser único |
| **RN-003** | Consultas: intervalo mínimo de 30 min entre consultas do mesmo médico |
| **RN-004** | Agendamento com no mínimo 1 hora de antecedência |
| **RN-005** | Cancelamento com no mínimo 2 horas de antecedência |
| **RN-006** | Pacientes/médicos: apenas soft delete (inativação) |
| **RN-007** | Prontuários são imutáveis — correções via novo registro |
| **RN-008** | Senhas: mínimo 8 chars, maiúscula, minúscula, número e especial |
| **RN-009** | RBAC: médico só acessa prontuário em escrita |

---

##  Tratamento de Erros

Todas as exceções são tratadas pelo middleware global e retornam respostas padronizadas no formato **RFC 7807 (Problem Details)**:

```json
{
  "type": "https://httpstatuses.com/409",
  "title": "Conflict",
  "status": 409,
  "detail": "Já existe uma consulta agendada para este médico neste horário.",
  "traceId": "00-abc123-def456-00"
}
```

| Exceção | HTTP Status |
|---------|-------------|
| `NotFoundException` | 404 Not Found |
| `ConflictException` | 409 Conflict |
| `ValidationException` | 400 Bad Request |
| `UnauthorizedException` | 401 Unauthorized |
| `ForbiddenException` | 403 Forbidden |
| Não tratada | 500 Internal Server Error |

---

##  Contribuição

1. Faça um fork do projeto
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit: `git commit -m 'feat: adiciona nova funcionalidade'`
4. Push: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

---

##  Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

<div align="center">
  Desenvolvido com ❤️ para o projeto CareFlow Health Manager
  <br>
  <strong>C# .NET 8 | Clean Architecture | PostgreSQL | Docker</strong>
</div>
