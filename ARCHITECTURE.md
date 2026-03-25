# ONGES Campaign API - Documentação de Arquitetura

## Visão Geral da Arquitetura

A aplicação segue o padrão **Domain-Driven Design (DDD)** combinado com **CQRS (Command Query Responsibility Segregation)** para oferecer:

- ✅ Separação clara de responsabilidades
- ✅ Lógica de negócio centralizada no Domain
- ✅ Escalabilidade e manutenibilidade
- ✅ Testabilidade mais fácil
- ✅ Comunicação via eventos de domínio

## Camadas da Aplicação

### 1. Domain Layer (ONGES.Campaign.Domain)

**Responsabilidade**: Representar as regras de negócio

**Componentes principais**:

#### Agregados
- **CampaignAggregate**: Raiz do agregado que encapsula toda a lógica de uma campanha

#### Entities
- **BaseEntity**: Classe base para todas as entidades com suporte a eventos de domínio

#### Value Objects
- **Money**: Encapsula a lógica de valor financeiro (validação de positivo)
- **CampaignStatus**: Encapsula os estados possíveis de uma campanha

#### Domain Events
Eventos que representam algo que aconteceu no domínio:
- `CampaignCreatedDomainEvent`
- `CampaignUpdatedDomainEvent`
- `CampaignCancelledDomainEvent`
- `CampaignAmountRaisedUpdatedDomainEvent`
- `CampaignCompletedDomainEvent`

#### Interfaces
- **ICampaignRepository**: Define como persistir campanhas
- **IUnitOfWork**: Define como gerenciar transações

**Vantagem**: O domínio é completamente independente de frameworks. Nenhuma dependência externa.

### 2. Application Layer (ONGES.Campaign.Application)

**Responsabilidade**: Orquestrar casos de uso (Use Cases)

**Componentes principais**:

#### Commands (Ações que modificam estado)
- `CreateCampaignCommand`
- `UpdateCampaignCommand`
- `CancelCampaignCommand`
- `UpdateCampaignAmountRaisedCommand`

#### Queries (Ações que consultam dados)
- `GetCampaignByIdQuery`
- `GetActiveCampaignsQuery`
- `GetAllCampaignsQuery`

#### Handlers
- **CommandHandlers**: Processam commands e publicam eventos
- **QueryHandlers**: Retornam dados do banco (read-only)

#### DTOs (Data Transfer Objects)
- `CreateCampaignRequest`
- `UpdateCampaignRequest`
- `CampaignResponse`
- `TransparencyPanelResponse`
- `ApiResponse<T>`

#### Validadores
- Regras de validação usando FluentValidation
- Validação de requests antes de chegar ao Domain

#### Mappers
- AutoMapper para converter entre entities e DTOs

**Vantagem**: Reutilizável por diferentes interfaces (Web, gRPC, CLI, etc.)

### 3. Infrastructure Layer (ONGES.Campaign.Infrastructure)

**Responsabilidade**: Implementar interfaces definidas no Domain

**Componentes principais**:

#### Persistence
- **CampaignDbContext**: DbContext do EF Core
- **UnitOfWork**: Implementação de `IUnitOfWork`
- **Migrations**: Controle de versão do banco de dados

#### Repositories
- **CampaignRepository**: Implementação de `ICampaignRepository`

#### Messaging
- **RabbitMqPublisher**: Publicar eventos para filas (stub por enquanto)

#### Configuration
- **InfrastructureExtensions**: Registra todos os serviços de infraestrutura

**Vantagem**: Pode ser facilmente substituída por outra implementação sem afetar o restante da aplicação.

### 4. API Layer (ONGES.Campaign.API)

**Responsabilidade**: Expor endpoints HTTP e gerenciar requisições

**Componentes principais**:

#### Controllers
- **CampaignsController**: Gerencia operações CRUD de campanhas
- **TransparencyController**: Painel público com campanhas ativas

#### Middleware
- **ExceptionHandlingMiddleware**: Captura exceções e retorna respostas HTTP apropriadas

#### Configuration
- **ServiceCollectionExtensions**: Configura DI, autenticação, Swagger, etc.
- **appsettings.json**: Configurações da aplicação

**Vantagem**: Separa lógica de apresentação da lógica de negócio.

## Fluxo de Requisição

```
Client Request
    ↓
API Controller (Validação HTTP)
    ↓
Command/Query (Envelope de dados)
    ↓
Validator (FluentValidation)
    ↓
Handler (Orquestração)
    ↓
Domain (Lógica de Negócio - Criar Agregado)
    ↓
Repository (Persistir no banco)
    ↓
Events (Publicar eventos de domínio - RabbitMQ)
    ↓
Response (Mapper + DTO)
    ↓
Client Response
```

## Padrões de Design Utilizados

### 1. Repository Pattern
Abstrai a persistência de dados através da interface `ICampaignRepository`.

```csharp
// Domain
public interface ICampaignRepository {
    Task<CampaignAggregate?> GetByIdAsync(Guid id);
}

// Infrastructure
public class CampaignRepository : ICampaignRepository {
    public async Task<CampaignAggregate?> GetByIdAsync(Guid id) {
        return await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id);
    }
}
```

### 2. Unit of Work Pattern
Coordena mudanças em múltiplos agregados em uma transação.

```csharp
// Domain
public interface IUnitOfWork {
    ICampaignRepository Campaigns { get; }
    Task<int> SaveChangesAsync();
}

// Infrastructure
public class UnitOfWork : IUnitOfWork {
    public async Task<int> SaveChangesAsync() {
        return await _context.SaveChangesAsync();
    }
}
```

### 3. CQRS (Command Query Responsibility Segregation)
Separa operações de escrita (Commands) de leitura (Queries).

```csharp
// Command (Escrita)
public record CreateCampaignCommand(...) : IRequest<CampaignResponse>;

// Query (Leitura)
public record GetCampaignByIdQuery(Guid Id) : IRequest<CampaignResponse?>;
```

### 4. Domain Events
Eventos que representam mudanças importantes no domínio.

```csharp
// Domínio publica um evento
campaign.AddDomainEvent(new CampaignCreatedDomainEvent(...));

// Infrastructure publica para RabbitMQ
await _messagePublisher.PublishAsync("campaigns.created", domainEvent);

// Worker consome o evento
consumer.Listen("campaigns.created", async (event) => {
    // Atualizar valor arrecadado
});
```

### 5. Value Objects
Objetos que representam valores imutáveis do domínio.

```csharp
// Money é um Value Object
public sealed class Money : IEquatable<Money> {
    public decimal Amount { get; }
    
    public Money(decimal amount) {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");
        Amount = amount;
    }
}

// Uso
var meta = new Money(10000.00); // Validação automática
```

### 6. Agregados
Agrupamentos de entidades com uma raiz que controla acesso e invariantes.

```csharp
// CampaignAggregate é a raiz
public sealed class CampaignAggregate : BaseEntity {
    // Contém Campaign info
    // Factory método para criar
    public static CampaignAggregate Create(...) { }
    
    // Métodos de domínio
    public void UpdateAmountRaised(decimal amount) { }
    public void Cancel() { }
}
```

## Fluxo de Autenticação e Autorização

```
User Login → Gera JWT
    ↓
JWT contém Claims (userId, role)
    ↓
API recebe requisição com JWT no header Authorization
    ↓
JWT Bearer Middleware valida signature
    ↓
Claims extraídos (userId viene de ClaimTypes.NameIdentifier)
    ↓
[Authorize(Roles = "GestorONG")] verifica role
    ↓
Controller executa operação com contexto autenticado
```

## Estrutura de Banco de Dados

### Tabela: Campaigns

| Coluna | Tipo | Constraints | Descrição |
|--------|------|-----------|-----------|
| Id | UNIQUEIDENTIFIER | PK | ID único da campanha |
| Title | VARCHAR(255) | NOT NULL | Título da campanha |
| Description | VARCHAR(1000) | NOT NULL | Descrição detalhada |
| FinancialTarget | DECIMAL(18,2) | NOT NULL | Meta de arrecadação |
| AmountRaised | DECIMAL(18,2) | NOT NULL, DEFAULT 0 | Total arrecadado |
| Status | INT | NOT NULL | 1=Ativa, 2=Concluída, 3=Cancelada |
| StartDate | DATETIME2 | NOT NULL | Quando começa |
| EndDate | DATETIME2 | NOT NULL | Quando termina |
| CreatorId | UNIQUEIDENTIFIER | NOT NULL | Quem criou |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Timestamp de criação |
| UpdatedAt | DATETIME2 | NULL | Timestamp da última atualização |

**Índices**:
- PK: `Id`
- Index: `Status` (para queries "Ativa")
- Index: `CreatorId` (para listar por creator)
- Index: `CreatedAt` (para ordenação)

## Considerações de Escalabilidade

### Atual (MVP)
- ✅ Single database (SQL Server)
- ✅ Síncrono por enquanto
- ✅ Sem distribuição

### Futuro (Microserviços Completos)
- ⏳ Worker separate para processar doações
- ⏳ RabbitMQ/Kafka para mensageria
- ⏳ Event Sourcing para histórico completo
- ⏳ CQRS com read models separados
- ⏳ Database per service
- ⏳ API Gateway (Ocelot)

## Como Adicionar uma Nova Feature

### 1. Criar Value Object/Entity no Domain
```csharp
// ONGES.Campaign.Domain/ValueObjects
public sealed class Discount : IEquatable<Discount> { }
```

### 2. Adicionar ao Agregado
```csharp
public sealed class CampaignAggregate : BaseEntity {
    public Discount? Discount { get; private set; }
    
    public void ApplyDiscount(Discount discount) { 
        // Lógica
    }
}
```

### 3. Criar Command/Query
```csharp
// ONGES.Campaign.Application/Commands
public record ApplyDiscountCommand(Guid CampaignId, Discount Discount) 
    : IRequest<Unit>;
```

### 4. Criar Handler
```csharp
public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand> {
    public async Task Handle(ApplyDiscountCommand request, 
        CancellationToken cancellationToken) {
        // Implementar lógica
    }
}
```

### 5. Adicionar Controller Endpoint
```csharp
[HttpPost("{id}/discount")]
public async Task<IActionResult> ApplyDiscount(Guid id, Discount discount) {
    // Chamar handler via MediatR
}
```

## Testes

**Estrutura esperada** (não implementado ainda):
```
ONGES.Campaign.Tests/
├── Domain/
│   └── CampaignAggregateTests.cs
├── Application/
│   └── CreateCampaignCommandHandlerTests.cs
└── Integration/
    └── CampaignControllerTests.cs
```

## Observabilidade Futura

- **Logging**: Serilog (já configurado)
- **Tracing**: OpenTelemetry
- **Metrics**: Prometheus
- **Dashboard**: Grafana

## Referências

- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [MediatR](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://fluentvalidation.net/)
