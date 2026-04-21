# ONGES

Microserviço responsável pelo gerenciamento de campanhas da plataforma ONGES.

O projeto segue o mesmo padrão arquitetural dos outros repositórios do ecossistema, com separação por camadas, API HTTP para operações de gestão e um Consumer dedicado ao processamento assíncrono de doações.

## Visão Geral

Fluxo principal:

1. um usuário autenticado com perfil Gestor cria, atualiza ou cancela campanhas pela API
2. a API valida os dados de entrada
3. as campanhas são persistidas no SQL Server
4. eventos de domínio podem ser publicados no broker
5. o módulo de doações publica mensagens de doação no broker
6. o `ONGES.Campaign.Consumer` consome mensagens de doação
7. o Consumer atualiza o valor arrecadado da campanha
8. quando a meta é atingida, a campanha pode ser marcada como concluída

Mensagem consumida para atualizar arrecadação:

```json
{
  "CampaignId": "550e8400-e29b-41d4-a716-446655440000",
  "Amount": 500.00,
  "DonatedAt": "2026-04-03T10:30:00Z"
}
```

## Estrutura da Solução

A solução `ONGES.sln` contém os seguintes projetos:

- `ONGES.Campaign.API`
- `ONGES.Campaign.Application`
- `ONGES.Campaign.Domain`
- `ONGES.Campaign.Infrastructure`
- `ONGES.Campaign.Consumer`
- `ONGES.Contracts` (referência compartilhada)

Responsabilidade de cada camada:

- `API`: controllers, autenticação/autorização JWT, Swagger e métricas
- `Application`: DTOs e interfaces de serviços
- `Domain`: entidades, agregados, value objects e regras de negócio
- `Infrastructure`: persistência, repositórios, validadores, mensageria e configurações técnicas
- `Consumer`: processamento assíncrono das mensagens de doação
- `Contracts`: contrato compartilhado de mensagens entre microsserviços

## Tecnologias

- .NET 10
- ASP.NET Core Web API (Controllers)
- Entity Framework Core
- SQL Server
- RabbitMQ
- MassTransit
- FluentValidation
- JWT Bearer Authentication
- Swagger (Swashbuckle)
- Prometheus (`prometheus-net.AspNetCore`)

## Endpoints Principais

Base route:

```text
/api/v1
```

Campanhas (requer autenticação JWT):

- `GET /api/v1/campaigns`
- `GET /api/v1/campaigns/{id}`
- `GET /api/v1/campaigns/internal/{id}` (uso interno, anônimo)
- `POST /api/v1/campaigns` (requer role `Gestor`)
- `PUT /api/v1/campaigns/{id}` (requer role `Gestor`)
- `DELETE /api/v1/campaigns/{id}` (requer role `Gestor`)

Transparência (rota pública):

- `GET /api/v1/transparency/campaigns`

Observabilidade:

- `GET /metrics` (Prometheus)

## Configuração

### Banco de dados

Configuração esperada:

- `ConnectionStrings:DefaultConnection`

Exemplo local:

```text
Server=localhost,1433;Database=db-onges-campaign-dev;User Id=sa;Password=<SUA_SENHA>;TrustServerCertificate=True;
```

### JWT

A API espera os seguintes itens:

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key` (base64)
- `Jwt:ExpiresMinutes`

### RabbitMQ

Configurações esperadas:

- `RabbitMq:Host`
- `RabbitMq:Port`
- `RabbitMq:VirtualHost`
- `RabbitMq:Username`
- `RabbitMq:Password`
- `RabbitMq:CampaignUpdatesQueue`

Valor padrão de fila no projeto:

- `campaigns-queue`

## Executando Localmente

### 1. Restaurar dependências

```bash
dotnet restore ./ONGES.sln
```

### 2. Executar a API

```bash
dotnet run --project ./ONGES.Campaign.API/ONGES.Campaign.API.csproj
```

Com a API em desenvolvimento, o Swagger fica disponível em:

```text
https://localhost:7000/swagger
```

### 3. Executar o consumer

```bash
dotnet run --project ./ONGES.Campaign.Consumer/ONGES.Campaign.Consumer.csproj
```

## Docker

A solução possui Dockerfiles para API e Consumer:

- `ONGES.Campaign.API/Dockerfile`
- `ONGES.Campaign.Consumer/Dockerfile`

Build de exemplo:

```bash
docker build -f ONGES.Campaign.API/Dockerfile -t onges-campaign-api .
docker build -f ONGES.Campaign.Consumer/Dockerfile -t onges-campaign-consumer .
```

Execução de exemplo:

```bash
docker run --rm -p 8080:80 --name onges-campaign-api onges-campaign-api
docker run --rm --name onges-campaign-consumer onges-campaign-consumer
```

## Migrations

As migrations do EF Core já existem no projeto de Infrastructure.

Para aplicar manualmente:

```bash
dotnet ef database update \
  --project ./ONGES.Campaign.Infrastructure/ONGES.Campaign.Infrastructure.csproj \
  --startup-project ./ONGES.Campaign.API/ONGES.Campaign.API.csproj
```

## Observações Importantes

- somente usuários com role `Gestor` podem criar, editar e cancelar campanhas
- a rota de transparência é pública e retorna apenas dados resumidos
- a atualização de valor arrecadado é assíncrona via mensagens consumidas pelo `ONGES.Campaign.Consumer`
- a API executa `Database.Migrate()` automaticamente em ambiente de desenvolvimento
