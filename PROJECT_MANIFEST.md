# 📊 Manifesto do Projeto ONGES Campaign Microserviço

## 🎯 Objetivo

Criar um microserviço robusto de gestão de campanhas de arrecadação para a ONG Esperança Solidária, seguindo:
- ✅ **Domain-Driven Design (DDD)**
- ✅ **CQRS Pattern**
- ✅ **Clean Architecture**
- ✅ Requisitos do PDF do Hackathon 9NETT

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────┐
│           ONGES.Campaign.API (REST)             │
│  Controllers, Middleware, Autenticação JWT      │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│      ONGES.Campaign.Application (CQRS)          │
│ Commands, Queries, Handlers, DTOs, Validators  │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│       ONGES.Campaign.Domain (Núcleo)            │
│ Agregados, Value Objects, Eventos, Interfaces  │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│    ONGES.Campaign.Infrastructure (Dados)       │
│ EF Core, Repositórios, RabbitMQ, Migrations    │
└──────────────────┬──────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        ▼                     ▼
    SQL Server          RabbitMQ (Futuro)
                              │
                        ┌─────▼──────┐
                        │ Worker Job │
                        └────────────┘
```

## 📦 Estrutura de Diretórios Completa

```
ONGES/
│
├── 📁 ONGES.Campaign.Domain                     [Layer de Domínio]
│   ├── 📁 Aggregates/
│   │   └── 📄 CampaignAggregate.cs             [Raiz do Agregado]
│   ├── 📁 Entities/
│   │   └── 📄 BaseEntity.cs                    [Entidade Base + Domain Events]
│   ├── 📁 Events/
│   │   └── 📄 CampaignDomainEvents.cs          [5 eventos disparados]
│   ├── 📁 ValueObjects/
│   │   ├── 📄 Money.cs                         [VO para valores financeiros]
│   │   └── 📄 CampaignStatus.cs                [VO para status]
│   ├── 📁 Interfaces/
│   │   ├── 📄 ICampaignRepository.cs           [Contrato de repositório]
│   │   └── 📄 IUnitOfWork.cs                   [Contrato de transações]
│   ├── 📄 GlobalUsings.cs
│   ├── 📄 ONGES.Campaign.Domain.csproj
│
├── 📁 ONGES.Campaign.Application                [Layer de Aplicação - CQRS]
│   ├── 📁 Commands/
│   │   └── 📄 CampaignCommands.cs              [4 commands CQRS]
│   ├── 📁 Queries/
│   │   └── 📄 CampaignQueries.cs               [3 queries CQRS]
│   ├── 📁 Handlers/
│   │   ├── 📄 CommandHandlers.cs               [Implementação de commands]
│   │   └── 📄 QueryHandlers.cs                 [Implementação de queries]
│   ├── 📁 DTOs/
│   │   └── 📄 CampaignDTOs.cs                  [Request/Response models]
│   ├── 📁 Validators/
│   │   └── 📄 CampaignValidators.cs            [FluentValidation rules]
│   ├── 📁 Mappers/
│   │   └── 📄 CampaignMappingProfile.cs        [AutoMapper config]
│   ├── 📄 GlobalUsings.cs
│   ├── 📄 ONGES.Campaign.Application.csproj
│
├── 📁 ONGES.Campaign.Infrastructure             [Layer de Infraestrutura]
│   ├── 📁 Persistence/
│   │   ├── 📄 CampaignDbContext.cs             [EF Core DbContext]
│   │   ├── 📄 UnitOfWork.cs                    [Implementação UoW]
│   │   ├── 📁 Migrations/
│   │   │   └── 📄 README.md                    [Guia de migrations]
│   ├── 📁 Repositories/
│   │   └── 📄 CampaignRepository.cs            [Implementação repositório]
│   ├── 📁 Messaging/
│   │   └── 📄 IMessagePublisher.cs             [Publisher de eventos]
│   ├── 📁 Configuration/
│   │   └── 📄 InfrastructureExtensions.cs      [DI setup]
│   ├── 📄 GlobalUsings.cs
│   ├── 📄 ONGES.Campaign.Infrastructure.csproj
│
├── 📁 ONGES.Campaign.API                       [Layer de Apresentação]
│   ├── 📁 Controllers/
│   │   └── 📄 CampaignsController.cs           [2 controllers REST]
│   ├── 📁 Middleware/
│   │   └── 📄 ExceptionHandlingMiddleware.cs  [Tratamento global]
│   ├── 📁 Configuration/
│   │   └── 📄 ServiceCollectionExtensions.cs  [Setup services]
│   ├── 📁 Properties/
│   │   └── 📄 launchSettings.json             [Configurações de launch]
│   ├── 📄 Program.cs                          [Entry point]
│   ├── 📄 appsettings.json                    [Configurações produção]
│   ├── 📄 appsettings.Development.json        [Configurações dev]
│   ├── 📄 GlobalUsings.cs
│   ├── 📄 ONGES.Campaign.API.csproj
│
├── 📄 ONGES.sln                                [Solution file]
│
├── 📚 DOCUMENTAÇÃO
│   ├── 📄 README.md                            [Guia inicial - LEIA PRIMEIRO]
│   ├── 📄 GETTING_STARTED.md                   [Passos para inicial]
│   ├── 📄 ARCHITECTURE.md                      [Design e padrões]
│   ├── 📄 JWT_TESTING_GUIDE.md                 [Como gerar tokens]
│   ├── 📄 PROJECT_MANIFEST.md                  [Este arquivo]
│
├── 🧪 TESTING & DEPLOYMENT
│   ├── 📄 ONGES_Campaign_API.postman_collection.json  [Collection Postman]
│   ├── 📄 docker-compose.yml                   [SQL Server + RabbitMQ]
│   ├── 📄 run.bat                              [Script Windows]
│   ├── 📄 run.sh                               [Script Linux/Mac]
│
├── 📄 .gitignore                               [Git ignore file]
```

## 🎨 Padrões e Practices Utilizados

### 1. Domain-Driven Design (DDD)
- **Agregado**: `CampaignAggregate` como raiz
- **Value Objects**: `Money`, `CampaignStatus`
- **Domain Events**: Eventos que propagam mudanças importantes
- **Interfaces de Domínio**: `ICampaignRepository`, `IUnitOfWork`

### 2. CQRS (Command Query Responsibility Segregation)
- **Commands**: CreateCampaignCommand, UpdateCampaignCommand, etc.
- **Queries**: GetCampaignByIdQuery, GetActiveCampaignsQuery, etc.
- **Handlers**: Orquestram a execução via MediatR

### 3. Clean Architecture
- Camadas bem definidas com dependências unidirecionais
- Domain não depende de nada
- Application depende apenas do Domain
- Infrastructure implementa contratos do Domain

### 4. Repository Pattern
- `ICampaignRepository` abstrai persistência
- `CampaignRepository` implementa com EF Core
- Facilita testes e mudanças de banco

### 5. Unit of Work Pattern
- `IUnitOfWork` coordena mudanças
- `UnitOfWork` gerencia transações
- Garante consistência de dados

### 6. Dependency Injection
- Todos os serviços registrados no container
- Resolução automática de dependências
- Lifecycle apropriado (Scoped, Transient, Singleton)

## 🔒 Segurança

### Autenticação
- ✅ JWT Bearer Token
- ✅ HS256 (HMAC SHA-256)
- ✅ Claims com ExpiresAt

### Autorização
- ✅ Role-Based Access Control (RBAC)
- ✅ `[Authorize(Roles = "GestorONG")]`
- ✅ Endpoints públicos sem [Authorize]

### Validação
- ✅ FluentValidation nas requisições
- ✅ Validações de negócio no Domain
- ✅ Middleware de tratamento de erros

## 📊 Base de Dados

### Tabela: Campaigns

```sql
CREATE TABLE Campaigns (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description VARCHAR(1000) NOT NULL,
    FinancialTarget DECIMAL(18,2) NOT NULL,
    AmountRaised DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status INT NOT NULL,  -- 1=Ativa, 2=Concluida, 3=Cancelada
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL
);

CREATE INDEX IX_Campaigns_Status ON Campaigns(Status);
CREATE INDEX IX_Campaigns_CreatorId ON Campaigns(CreatorId);
CREATE INDEX IX_Campaigns_CreatedAt ON Campaigns(CreatedAt);
```

## 🔄 Fluxo de Dados

### Criar Campanha
```
POST /api/campaigns
    ↓ (CreateCampaignRequest)
CampaignsController.CreateCampaign()
    ↓
CreateCampaignCommand (via MediatR)
    ↓
CreateCampaignCommandValidator (FluentValidation)
    ↓
CreateCampaignCommandHandler
    ↓
CampaignAggregate.Create() (Lógica de negócio)
    ↓ (Dispara CampaignCreatedDomainEvent)
IUnitOfWork.SaveChangesAsync()
    ↓
CampaignRepository.AddAsync()
    ↓
CampaignDbContext.SaveChangesAsync()
    ↓
SQL Server (INSERT)
    ↓
Mapper.Map() (CampaignResponse)
    ↓
201 Created + JSON response
```

### Obter Campanhas Ativas (Transparência)
```
GET /api/transparency/campaigns
    ↓
TransparencyController.GetActiveCampaigns()
    ↓
GetActiveCampaignsQuery (via MediatR)
    ↓ (Sem validação necessária)
GetActiveCampaignsQueryHandler
    ↓
IUnitOfWork.Campaigns.GetAllActiveAsync()
    ↓
CampaignRepository.GetAllActiveAsync()
    ↓
CampaignDbContext.Where(c => c.Status.IsActive)
    ↓
SQL Server (SELECT)
    ↓
Mapper.Map() (List<TransparencyPanelResponse>)
    ↓
200 OK + JSON response
```

## 🚀 Como Rodar

### Rápido (Windows)
```bash
cd c:\Git\ONGES
.\run.bat
```

### Rápido (Linux/Mac)
```bash
cd ~/ONGES
./run.sh
```

### Manual
```bash
cd c:\Git\ONGES
dotnet restore
dotnet build
cd ONGES.Campaign.API
dotnet ef database update --project ..\ONGES.Campaign.Infrastructure
dotnet run
```

## 🧪 Testes

### Estrutura esperada (para implementação futura)
```
ONGES.Campaign.Tests/
├── Domain/
│   └── CampaignAggregateTests.cs
├── Application/
│   └── CreateCampaignCommandHandlerTests.cs
└── Integration/
    └── CampaignControllerTests.cs
```

### Comandos
```bash
dotnet test                    # Todos os testes
dotnet test --filter Campaign  # Tests específicos
dotnet test --coverage         # Com cobertura
```

## 📈 Métricas do Projeto

| Métrica | Valor |
|---------|-------|
| Linhas de Código (aproximado) | ~2,500 |
| Arquivos criados | 30+ |
| Namespaces | 12 |
| Classes/Records | 25+ |
| Interfaces | 4 |
| RabbitMQ pronto? | Não (stub) |
| Docker pronto? | Sim (compose) |
| Kubernetes pronto? | Não (futuro) |
| Testes unitários? | Não (framework pronto) |
| Cobertura esperada | 80%+ |

## 🔮 Roadmap Futuro

### Phase 2: Microserviço de Doações
```
ONGES.Donation.API/
├── Domain/     - Agregado Donation
├── Application - Commands para processar doação
└── Infrastructure - RabbitMQ consumer
```

### Phase 3: Worker Background
```
ONGES.Donation.Worker/
├── RabbitMQ Consumer
├── Event Handler
└── UpdateCampaignAmountRaised via HTTP
```

### Phase 4: Observabilidade
```
Prometheus    - Métricas
Grafana       - Dashboard
OpenTelemetry - Tracing
ELK Stack     - Logs
```

### Phase 5: Kubernetes & CI/CD
```
Kubernetes manifests (YAML)
GitHub Actions pipeline
Helm charts
```

## 💻 Stack Tecnológico

| Componente | Tecnologia | Versão |
|-----------|-----------|--------|
| Runtime | .NET | 8.0 |
| Web Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| CQRS | MediatR | 12.2 |
| Validação | FluentValidation | 11.9 |
| Mapping | AutoMapper | 13.0 |
| Logging | Serilog | 8.0 |
| Database | SQL Server | 2019+ |
| Message Queue | RabbitMQ | 3.0+ |
| Auth | JWT Bearer | - |
| API Docs | Swagger/OpenAPI | 3.1 |

## 📝 Arquivos Criados (Checklist)

### Domain Layer
- ✅ `CampaignAggregate.cs`
- ✅ `BaseEntity.cs`
- ✅ `CampaignDomainEvents.cs`
- ✅ `Money.cs`
- ✅ `CampaignStatus.cs`
- ✅ `ICampaignRepository.cs`
- ✅ `IUnitOfWork.cs`
- ✅ `GlobalUsings.cs`
- ✅ `.csproj` (Domain)

### Application Layer
- ✅ `CampaignCommands.cs`
- ✅ `CampaignQueries.cs`
- ✅ `CommandHandlers.cs`
- ✅ `QueryHandlers.cs`
- ✅ `CampaignDTOs.cs`
- ✅ `CampaignValidators.cs`
- ✅ `CampaignMappingProfile.cs`
- ✅ `GlobalUsings.cs`
- ✅ `.csproj` (Application)

### Infrastructure Layer
- ✅ `CampaignDbContext.cs`
- ✅ `UnitOfWork.cs`
- ✅ `CampaignRepository.cs`
- ✅ `IMessagePublisher.cs`
- ✅ `InfrastructureExtensions.cs`
- ✅ `GlobalUsings.cs`
- ✅ `Migrations/README.md`
- ✅ `.csproj` (Infrastructure)

### API Layer
- ✅ `CampaignsController.cs`
- ✅ `TransparencyController.cs`
- ✅ `ExceptionHandlingMiddleware.cs`
- ✅ `ServiceCollectionExtensions.cs`
- ✅ `Program.cs`
- ✅ `appsettings.json`
- ✅ `appsettings.Development.json`
- ✅ `launchSettings.json`
- ✅ `GlobalUsings.cs`
- ✅ `.csproj` (API)

### Arquivos de Configuração
- ✅ `ONGES.sln`
- ✅ `.gitignore`
- ✅ `docker-compose.yml`
- ✅ `run.bat`
- ✅ `run.sh`

### Documentação
- ✅ `README.md`
- ✅ `GETTING_STARTED.md`
- ✅ `ARCHITECTURE.md`
- ✅ `JWT_TESTING_GUIDE.md`
- ✅ `PROJECT_MANIFEST.md` (este)

### Testes
- ✅ `ONGES_Campaign_API.postman_collection.json`

## 📞 Contato

Projeto: **ONGES Campaign Microservice**
ONG: **Esperança Solidária**
Hackathon: **9NETT**
Data: **Março de 2026**

---

**Última atualização**: 25/03/2026
