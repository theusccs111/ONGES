# ONGES Campaign API - Guia de Uso

## Visão Geral

A ONGES Campaign API foi transformada de Minimal API para Controllers MVC e agora inclui:

- ✅ **Autenticação JWT**: Todas as rotas protegidas requerem um token JWT válido
- ✅ **Autorização por Role**: Apenas usuários com role "Gestor" podem acessar funcionalidades de gerenciamento
- ✅ **Rota Pública**: Painel de Transparência acessível sem autenticação
- ✅ **Background Worker**: Escuta o tópico `donates-topic` do Azure Service Bus e atualiza automaticamente o valor arrecadado das campanhas

## Configuração

### Appsettings

As configurações foram adicionadas ao `appsettings.json` e `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:server-db-onges.database.windows.net,1433;Initial Catalog=db-onges-campaign;..."
  },
  "Jwt": {
    "Key": "9y4XJg0aTphzFJw3TvksRvqHXd+Q4VB8f7ZvU08N+9Q=",
    "Issuer": "ONGES.Users",
    "Audience": "API",
    "ExpiresMinutes": 120
  },
  "AzureServiceBus": {
    "ConnectionString": "Endpoint=sb://sb-onges.servicebus.windows.net/;...",
    "TopicName": "donates-topic",
    "SubscriptionName": "campaigns-donations-subscription"
  }
}
```

## Endpoints

### 1. Campanhas (Protegido - Requer Role "Gestor")

#### GET /api/v1/campaigns
Obtém todas as campanhas
```http
GET /api/v1/campaigns
Authorization: Bearer {token}
```

#### GET /api/v1/campaigns/{id}
Obtém uma campanha por ID
```http
GET /api/v1/campaigns/{id}
Authorization: Bearer {token}
```

#### POST /api/v1/campaigns
Cria uma nova campanha
```http
POST /api/v1/campaigns
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Projeto Educação",
  "description": "Bolsas de estudo para crianças",
  "financialTarget": 50000.00,
  "startDate": "2026-05-01T00:00:00Z",
  "endDate": "2026-12-31T23:59:59Z"
}
```

#### PUT /api/v1/campaigns/{id}
Atualiza uma campanha
```http
PUT /api/v1/campaigns/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Projeto Educação 2026",
  "description": "Bolsas de estudo para crianças e famílias",
  "financialTarget": 60000.00,
  "startDate": "2026-05-01T00:00:00Z",
  "endDate": "2026-12-31T23:59:59Z"
}
```

#### DELETE /api/v1/campaigns/{id}
Cancela uma campanha
```http
DELETE /api/v1/campaigns/{id}
Authorization: Bearer {token}
```

### 2. Painel de Transparência (Público - Sem Autenticação)

#### GET /api/v1/transparency/campaigns
Obtém campanhas ativas para o painel de transparência
```http
GET /api/v1/transparency/campaigns
```

**Resposta:**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Projeto Educação",
    "financialTarget": 50000.00,
    "amountRaised": 15000.00
  }
]
```

## Background Worker (DonationWorker)

O `DonationWorker` é iniciado automaticamente quando a aplicação inicia e:

1. Se conecta ao Azure Service Bus
2. Escuta mensagens no tópico `donates-topic`
3. Processa mensagens de doação com o formato:
   ```json
   {
     "CampaignId": "550e8400-e29b-41d4-a716-446655440000",
     "Amount": 500.00,
     "DonatedAt": "2026-04-03T10:30:00Z"
   }
   ```
4. Atualiza o valor arrecadado da campanha
5. Se o valor arrecadado atingir a meta, a campanha é marcada como Concluída

## Autenticação JWT

### Estrutura do Token

O token JWT deve conter os seguintes claims:

```json
{
  "sub": "usuario-id",
  "iss": "ONGES.Users",
  "aud": "API",
  "role": "Gestor",
  "nameidentifier": "550e8400-e29b-41d4-a716-446655440000",
  "exp": 1234567890
}
```

### Fluxo de Autenticação

1. O usuário faz login na API ONGES.Users
2. Recebe um token JWT válido
3. Inclui o token no header `Authorization: Bearer {token}` de todas as requisições protegidas

## Arquitetura

### Estrutura de Pastas

```
ONGES.Campaign.API/
├── Controllers/
│   ├── CampaignsController.cs
│   └── TransparencyController.cs
├── appsettings.json
├── Program.cs
└── ...

ONGES.Campaign.Infrastructure/
├── Workers/
│   └── DonationWorker.cs
├── Configuration/
│   └── InfrastructureExtensions.cs
├── Services/
│   └── CampaignService.cs
└── ...
```

### Fluxo de Doações

```
Azure Service Bus (donates-topic)
         ↓
    DonationWorker
         ↓
  CampaignService.UpdateAmountRaisedAsync
         ↓
  CampaignRepository.UpdateAsync
         ↓
  Database Update
         ↓
  Domain Events Published (opcional)
```

## Executar Localmente

```bash
# Build
dotnet build

# Run
dotnet run --project ONGES.Campaign.API

# Swagger UI estará disponível em:
# https://localhost:7000/swagger
```

## Dependências

- .NET 8+
- Entity Framework Core
- FluentValidation
- Azure Service Bus SDK
- JWT Bearer Authentication

## Status Lifecycle da Campanha

- **Active**: Campanha está ativa e recebendo doações
- **Completed**: Meta financeira foi atingida
- **Cancelled**: Campanha foi cancelada manualmente

