# 🎉 Projeto ONGES Campaign API - Criado com Sucesso!

## 📋 Resumo do que foi criado

Você agora tem uma **solução completa .NET Core 8 com DDD** para gerenciar campanhas de arrecadação. Segue rigorosamente os requisitos do PDF do Hackathon.

## 📦 Estrutura de Pastas Criada

```
c:\Git\ONGES\
│
├── ONGES.Campaign.Domain/              # Layer de Domínio (Entidades, Agregados, Value Objects)
│   ├── Aggregates/
│   │   └── CampaignAggregate.cs        # Raiz do agregado com lógica de negócio
│   ├── Entities/
│   │   └── BaseEntity.cs               # Entidade base com suporte a Domain Events
│   ├── Events/
│   │   └── CampaignDomainEvents.cs     # Eventos disparados
│   ├── ValueObjects/
│   │   ├── Money.cs                    # Representa valor financeiro
│   │   └── CampaignStatus.cs           # Enum-like para status
│   └── Interfaces/
│       ├── ICampaignRepository.cs      # Contrato de persistência
│       └── IUnitOfWork.cs              # Contrato de transações
│
├── ONGES.Campaign.Application/         # Layer de Aplicação (CQRS)
│   ├── Commands/
│   │   └── CampaignCommands.cs         # CreateCampaign, UpdateCampaign, etc.
│   ├── Queries/
│   │   └── CampaignQueries.cs          # GetCampaignById, GetActiveCampaigns, etc.
│   ├── Handlers/
│   │   ├── CommandHandlers.cs          # Processa commands
│   │   └── QueryHandlers.cs            # Processa queries
│   ├── DTOs/
│   │   └── CampaignDTOs.cs             # Request/Response models
│   ├── Validators/
│   │   └── CampaignValidators.cs       # FluentValidation rules
│   └── Mappers/
│       └── CampaignMappingProfile.cs   # AutoMapper configuration
│
├── ONGES.Campaign.Infrastructure/      # Layer de Infraestrutura (Persistência)
│   ├── Persistence/
│   │   ├── CampaignDbContext.cs        # EF Core DbContext
│   │   ├── UnitOfWork.cs               # Implementação do Unit of Work
│   │   └── Migrations/                 # Migrações do EF Core
│   ├── Repositories/
│   │   └── CampaignRepository.cs       # Implementação do repositório
│   ├── Messaging/
│   │   └── IMessagePublisher.cs        # Interface para publicar eventos
│   └── Configuration/
│       └── InfrastructureExtensions.cs # Registro de serviços DI
│
├── ONGES.Campaign.API/                 # Layer de Apresentação (API REST)
│   ├── Controllers/
│   │   └── CampaignsController.cs      # Endpoints de campanhas
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs  # Tratamento global de erros
│   ├── Configuration/
│   │   └── ServiceCollectionExtensions.cs  # Configuração de serviços
│   ├── Properties/
│   │   └── launchSettings.json         # Configuração de launch
│   ├── Program.cs                      # Entry point
│   ├── appsettings.json                # Configurações
│   └── appsettings.Development.json    # Configurações de desenvolvimento
│
├── ONGES.sln                           # Solution file
├── README.md                           # Documentação principal
├── ARCHITECTURE.md                     # Documentação de arquitetura
├── JWT_TESTING_GUIDE.md                # Guia de testes com JWT
├── ONGES_Campaign_API.postman_collection.json  # Collection do Postman
├── docker-compose.yml                  # Docker compose para SQL Server + RabbitMQ
├── run.sh                              # Script para rodar (Linux/Mac)
├── run.bat                             # Script para rodar (Windows)
└── .gitignore                          # Arquivo Git ignore
```

## 🚀 Próximos Passos

### 1️⃣ Restaurar Dependências
```bash
cd c:\Git\ONGES
dotnet restore
```

### 2️⃣ Configurar Banco de Dados (Escolha uma opção)

**Opção A: LocalDB (Recomendado para Dev)**
- Já está configurado em `appsettings.Development.json`
- Nenhuma ação necessária

**Opção B: SQL Server + RabbitMQ via Docker**
```bash
docker-compose up -d
```

**Opção C: Seu SQL Server Local**
- Edite a connection string em `appsettings.json`

### 3️⃣ Aplicar Migrações
```bash
cd ONGES.Campaign.API
dotnet ef database update --project ..\ONGES.Campaign.Infrastructure
```

### 4️⃣ Executar a Aplicação
```bash
# Opção A: Via script
./run.bat  # Windows
./run.sh   # Linux/Mac

# Opção B: Direto
dotnet run
```

A API estará disponível em:
- 🌐 **API**: https://localhost:7000
- 📚 **Swagger**: https://localhost:7000/swagger

## ✨ Recursos Implementados

### ✅ Domain-Driven Design (DDD)
- Agregado `CampaignAggregate` com lógica de negócio encapsulada
- Value Objects `Money` e `CampaignStatus`
- Eventos de domínio para cada mudança importante

### ✅ CQRS (Command Query Responsibility Segregation)
- Commands para operações de escrita
- Queries para operações de leitura
- Separação clara de responsabilidades

### ✅ Validações de Negócio
- Data de término não pode ser no passado
- Meta financeira deve ser > 0
- Apenas campanhas "Ativa" podem ser editadas

### ✅ Autenticação e Autorização
- JWT Bearer Token
- Roles: GestorONG (gerencia campanhas), Doador (faz doações)
- Endpoints protegidos com `[Authorize]`

### ✅ Painel de Transparência
- Endpoint público `/api/transparency/campaigns`
- Retorna apenas campanhas com status "Ativa"
- Sem autenticação necessária

### ✅ Tratamento de Erros Global
- Middleware centralizado catching exceções
- Respostas HTTP apropriadas (400, 404, 500)

### ✅ Documentação automática
- Swagger/OpenAPI integrado
- Todos os endpoints documentados

### ✅ Logging
- Serilog configurado
- Logs em console e arquivo

### ✅ Dependency Injection
- AutoMapper para mapping de DTOs
- FluentValidation para validações
- MediatR para CQRS

## 🔐 Endpoints Implementados

### Autenticandos (Requer JWT + Role: GestorONG)
- `POST /api/campaigns` - Criar campanha
- `PUT /api/campaigns/{id}` - Atualizar campanha
- `DELETE /api/campaigns/{id}` - Cancelar campanha
- `GET /api/campaigns` - Listar todas as campanhas

### Públicos
- `GET /api/campaigns/{id}` - Obter campanha por ID
- `GET /api/transparency/campaigns` - Painel de transparência (campanhas ativas)

## 📊 Propriedades da Campanha

| Campo | Tipo | Validação |
|-------|------|-----------|
| `id` | Guid | Gerado automaticamente |
| `title` | String | Obrigatório, max 255 chars |
| `description` | String | Obrigatório, max 1000 chars |
| `financialTarget` | Decimal | Obrigatório, > 0 |
| `amountRaised` | Decimal | Calculado via eventos |
| `goalAchieved` | Boolean | amountRaised >= financialTarget |
| `status` | Enum | Ativa / Concluída / Cancelada |
| `startDate` | DateTime | Obrigatório |
| `endDate` | DateTime | Obrigatório, no future |
| `creatorId` | Guid | ID do usuário que criou |
| `createdAt` | DateTime | Automático |
| `updatedAt` | DateTime | Nulo até atualizado |

## 🧪 Testando a API

### Via Postman
1. Importe o arquivo `ONGES_Campaign_API.postman_collection.json`
2. Siga as instruções em `JWT_TESTING_GUIDE.md` para gerar um token
3. Execute os requests

### Via curl
```bash
# Obter campanhas ativas (público)
curl -X GET https://localhost:7000/api/transparency/campaigns -k

# Criar campanha (requer JWT)
curl -X POST https://localhost:7000/api/campaigns \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Nova","description":"Desc","financialTarget":5000,"startDate":"2024-01-01T00:00:00Z","endDate":"2024-12-31T23:59:59Z"}' \
  -k
```

## 🔄 Arquitetura em Alto Nível

```
┌─────────────┐
│   Cliente   │
└──────┬──────┘
       │ HTTP Request
       ▼
┌─────────────────────────┐
│  API Controller Layer   │ ◄─ Validação HTTP, Autenticação
├─────────────────────────┤
│  Command/Query Envelope │ ◄─ CQRS Pattern
├─────────────────────────┤
│  Validation Layer       │ ◄─ FluentValidation
├─────────────────────────┤
│  Handler (Orchestration)│ ◄─ Coordena lógica
├─────────────────────────┤
│  Domain Layer (Logic)   │ ◄─ CampaignAggregate, Value Objects
├─────────────────────────┤
│  Repository Layer       │ ◄─ Persistência abstrata
├─────────────────────────┤
│ Entity Framework Core   │ ◄─ ORM mapping
├─────────────────────────┤
│   SQL Server Database   │ ◄─ Armazenamento
└─────────────────────────┘

       + Eventos de Domínio
       ▼
┌─────────────────────────┐
│  Message Broker         │ ◄─ RabbitMQ/Kafka (Futuro)
├─────────────────────────┤
│  Worker Service         │ ◄─ Processa doações (Futuro)
└─────────────────────────┘
```

## 🎯 Alinhamento com Requisitos do PDF

✅ **Autenticação e Autorização (RBAC)**
- JWT tokens implementados
- Roles: GestorONG, Doador

✅ **Gestão de Campanhas (Acesso: GestorONG)**
- ✅ Criar: Campos obrigatórios, validações
- ✅ Editar: Apenas se Ativa
- ✅ Cancelar: Implementado
- ✅ RN: Data no futuro, Meta > 0

✅ **Painel de Transparência (Acesso: Público)**
- ✅ Retorna apenas campanhas Ativa
- ✅ Sem autenticação necessária
- ✅ Retorna Título, Meta, Valor Arrecadado

✅ **Sem Minimal API**
- Controllers tradicionais implementados

✅ **DDD Completo**
- Domain: Agregados, Value Objects, Eventos
- Application: CQRS, DTOs, Validadores
- Infrastructure: Repositórios, EF Core
- API: Controllers, Middleware

## 💡 Próximas Melhorias

- [ ] Implementar Worker para processar doações via RabbitMQ
- [ ] Testes unitários com xUnit
- [ ] Testes de integração
- [ ] API Gateway com Ocelot
- [ ] Observabilidade com Prometheus/Grafana
- [ ] Docker + Kubernetes manifests
- [ ] CI/CD com GitHub Actions
- [ ] Implementar Event Sourcing para histórico
- [ ] Read models separados (CQRS avançado)

## 📚 Documentação

Leia com atenção:
1. **[README.md](./README.md)** - Como rodar e usar a API
2. **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Explicação detalhada da arquitetura
3. **[JWT_TESTING_GUIDE.md](./JWT_TESTING_GUIDE.md)** - Como gerar e usar tokens JWT
4. **[Postman Collection](./ONGES_Campaign_API.postman_collection.json)** - Para testar via Postman

## 🤝 Suporte

Qualquer dúvida sobre o código, consulte:
- Comentários XML nas classes
- Documentação dos métodos públicos
- Exemplos nos controladores

## 📝 Licença

MIT License - Use livremente para o projeto da ONG Esperança Solidária

---

**Projeto criado com** ❤️ **para ONGES - Hackathon 9NETT**
