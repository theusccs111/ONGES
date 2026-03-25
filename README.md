# ONGES Campaign API

**Microserviço de Gestão de Campanhas - ONG Esperança Solidária**

## Visão Geral

Este é um microserviço .NET Core 8 responsável pela gestão completa de campanhas de arrecadação para a plataforma digital "Conexão Solidária" da ONG Esperança Solidária.

### Arquitetura

O projeto segue os princípios de **Domain-Driven Design (DDD)** com a seguinte estrutura camada:

```
ONGES.Campaign.Domain
├── Aggregates         (CampaignAggregate - Raiz do Agregado)
├── Entities           (BaseEntity - Classes base)
├── Events             (CampaignDomainEvents - Eventos de Domínio)
├── ValueObjects       (Money, CampaignStatus - Objetos de Valor)
└── Interfaces         (ICampaignRepository, IUnitOfWork)

ONGES.Campaign.Application
├── Commands           (CreateCampaignCommand, UpdateCampaignCommand, etc.)
├── Queries            (GetCampaignByIdQuery, GetActiveCampaignsQuery, etc.)
├── Handlers           (CommandHandlers, QueryHandlers)
├── DTOs               (CreateCampaignRequest, CampaignResponse, etc.)
├── Validators         (FluentValidation)
└── Mappers            (AutoMapper Profiles)

ONGES.Campaign.Infrastructure
├── Persistence        (CampaignDbContext, UnitOfWork)
├── Repositories       (CampaignRepository)
├── Messaging          (RabbitMQ Publisher - Para Eventos)
└── Configuration      (DI Extensions)

ONGES.Campaign.API
├── Controllers        (CampaignsController, TransparencyController)
├── Middleware         (ExceptionHandlingMiddleware)
├── Configuration      (ServiceCollectionExtensions)
└── Program.cs         (Entry Point)
```

## Requisitos Técnicos

- **.NET 8.0** ou superior
- **SQL Server** 2019 ou superior (ou LocalDB para desenvolvimento)
- **Entity Framework Core 8.0**
- **MediatR** para CQRS
- **FluentValidation** para validações
- **AutoMapper** para mapeamento de objetos
- **JWT** para autenticação
- **Serilog** para logging

## Configurar e Executar

### 1. Clonar o Repositório

```bash
git clone <repo-url>
cd ONGES
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Configurar Banco de Dados

#### Opção A: SQL Server Local

Atualize `appsettings.json` com sua string de conexão:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=seu-servidor;Database=ONGES_Campaign;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

#### Opção B: LocalDB (Desenvolvimento)

A configuração padrão em `appsettings.Development.json` já usa LocalDB.

### 4. Aplicar Migrações

```bash
cd ONGES.Campaign.API
dotnet ef database update --project ../ONGES.Campaign.Infrastructure
```

### 5. Executar a Aplicação

```bash
cd ONGES.Campaign.API
dotnet run
```

A API estará disponível em: `https://localhost:7000`

Swagger UI: `https://localhost:7000/swagger`

## Estrutura de Dados

### Entidade Campaign

```sql
CREATE TABLE Campaigns (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description VARCHAR(1000) NOT NULL,
    FinancialTarget DECIMAL(18,2) NOT NULL,
    AmountRaised DECIMAL(18,2) DEFAULT 0,
    Status INT NOT NULL, -- 1=Ativa, 2=Concluída, 3=Cancelada
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL
)
```

## Endpoints

### Autenticação

**Obter Token JWT** (Future Implementation)
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@example.com",
  "password": "senha"
}
```

### Gestão de Campanhas (Requer JWT + Role: GestorONG)

#### Criar Campanha
```
POST /api/campaigns
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Campanha de Emergência",
  "description": "Arrecadação para crianças em situação de vulnerabilidade",
  "financialTarget": 10000.00,
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T23:59:59Z"
}
```

#### Atualizar Campanha
```
PUT /api/campaigns/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Novo Título",
  "description": "Nova descrição",
  "financialTarget": 15000.00,
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T23:59:59Z"
}
```

#### Cancelar Campanha
```
DELETE /api/campaigns/{id}
Authorization: Bearer <token>
```

#### Obter Todas as Campanhas (GestorONG only)
```
GET /api/campaigns
Authorization: Bearer <token>
```

#### Obter Campanha por ID
```
GET /api/campaigns/{id}
```

### Painel de Transparência (Público)

#### Obter Campanhas Ativas
```
GET /api/transparency/campaigns
```

Resposta:
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Campanha de Emergência",
    "financialTarget": 10000.00,
    "amountRaised": 2500.00
  }
]
```

## Regras de Negócio

### Criação de Campanha
- ✅ Título e descrição são obrigatórios
- ✅ Meta financeira deve ser > 0
- ✅ Data de término não pode ser no passado
- ✅ Data de início deve ser anterior à data de término
- ✅ Apenas usuários com role GestorONG podem criar

### Atualização de Campanha
- ✅ Apenas campanhas em status "Ativa" podem ser editadas
- ✅ As mesmas validações de criação se aplicam
- ✅ Apenas GestorONG pode editar

### Status de Campanha
- **Ativa**: Campanha aceita doações
- **Concluída**: Meta financeira foi atingida (automático)
- **Cancelada**: Campanha foi cancelada manualmente

### Processo de Doação (Futura Implementação)
- ❌ Doações não podem ser feitas para campanhas encerradas ou canceladas
- ❌ Cada doação publica um evento `DoacaoRecebidaEvent`
- ❌ Um Worker consome a fila e atualiza o valor arrecadado
- ❌ Quando `valor_arrecadado >= meta`, a campanha muda para "Concluída"

## Eventos de Domínio

Os seguintes eventos são disparados durante a execução:

- **CampaignCreatedDomainEvent**: Quando uma campanha é criada
- **CampaignUpdatedDomainEvent**: Quando uma campanha é atualizada
- **CampaignCancelledDomainEvent**: Quando uma campanha é cancelada
- **CampaignAmountRaisedUpdatedDomainEvent**: Quando o valor arrecadado muda
- **CampaignCompletedDomainEvent**: Quando a meta é atingida

## Testing

```bash
# Restaurar pacotes de teste
dotnet restore

# Executar testes (quando implementados)
dotnet test
```

## Documentação da API

- **Swagger/OpenAPI**: `https://localhost:7000/swagger`
- Todos os endpoints estão documentados no Swagger

## Melhorias Futuras

- [ ] Implementar Worker para processar eventos de doação via RabbitMQ
- [ ] Adicionar testes unitários com xUnit
- [ ] Implementar API Gateway (Ocelot)
- [ ] Adicionar observabilidade com Grafana/Prometheus
- [ ] Containerizar com Docker
- [ ] Configurar Kubernetes manifests (YAML)
- [ ] Implementar CI/CD com GitHub Actions
- [ ] Adicionar autenticação de usuários (GestorONG e Doador)

## Contribuindo

1. Faça um Fork do projeto
2. Crie uma branch para seu feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas alterações (`git commit -m 'Add MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## Licença

Este projeto está sob a licença MIT.

## Contato

- **Organização**: ONGES
- **Projeto**: Conexão Solidária
- **ONG**: Esperança Solidária
