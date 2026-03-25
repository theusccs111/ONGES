# Guia de Testes com JWT

## Como Gerar e Usar JWT Token

### Opção 1: Usando JWT.io (Recomendado para Desenvolvimento)

1. Acesse [jwt.io](https://jwt.io)
2. No lado esquerdo, selecione o algoritmo **HS256**
3. Cole o seguinte no campo **PAYLOAD**:

```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "gestor@onges.com.br",
  "role": "GestorONG",
  "iat": 1700000000,
  "exp": 1799999999
}
```

4. No campo **VERIFY SIGNATURE**, cole a secret key:
```
your-super-secret-key-change-this-in-production-must-be-at-least-32-characters
```

5. Copie o token gerado de `eyJ...` (HEADER.PAYLOAD.SIGNATURE)

### Opção 2: Usando C# Localmente

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public static string GenerateJwtToken()
{
    var secretKey = "your-super-secret-key-change-this-in-production-must-be-at-least-32-characters";
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "ONGES.Campaign.API",
        audience: "ONGES.Campaign.Client",
        claims: new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "gestor@onges.com.br"),
            new Claim(ClaimTypes.Role, "GestorONG")
        },
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials);

    var tokenHandler = new JwtSecurityTokenHandler();
    return tokenHandler.WriteToken(token);
}
```

## Usando no Postman

### Opção 1: Variável de Ambiente

1. Abra a Collection **ONGES Campaign API**
2. Clique na aba **Variables**
3. Encontre a variável `jwt_token`
4. Cole o token no campo **Current Value**
5. Clique em **Save**

Agora todos os requests que usam `{{jwt_token}}` funcionarão.

### Opção 2: Header Manual

Em qualquer requisição que precise de autenticação, adicione um header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Testando Endpoints

### ✅ Criar Campanha (Requer JWT + Role: GestorONG)

```bash
POST https://localhost:7000/api/campaigns
Authorization: Bearer {{jwt_token}}
Content-Type: application/json

{
  "title": "Campanha de Emergência",
  "description": "Arrecadação para crianças",
  "financialTarget": 50000.00,
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T23:59:59Z"
}
```

**Resposta esperada (201 Created)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Campanha de Emergência",
  "description": "Arrecadação para crianças",
  "financialTarget": 50000.00,
  "amountRaised": 0.0,
  "goalAchieved": false,
  "status": "Ativa",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T23:59:59Z",
  "creatorId": "550e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2024-03-25T10:30:00Z",
  "updatedAt": null
}
```

### ✅ Obter Campanhas Ativas (Público - Sem JWT)

```bash
GET https://localhost:7000/api/transparency/campaigns
Content-Type: application/json
```

**Resposta esperada (200 OK)**:
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Campanha de Emergência",
    "financialTarget": 50000.00,
    "amountRaised": 2500.00
  }
]
```

## Erros Comuns

### 401 Unauthorized
- **Causa**: Token expirado ou inválido
- **Solução**: Gere um novo token

### 403 Forbidden
- **Causa**: Role do token não é "GestorONG"
- **Solução**: Verifique se o token tem `"role": "GestorONG"` no payload

### 400 Bad Request
- **Causa**: Validação falhou (ex: data no passado)
- **Solução**: Verifique as validações no README.md

## Variáveis do Token JWT

Quando criar um token, inclua:

```json
{
  "sub": "USER_ID",                    // ID do usuário (required)
  "email": "user@example.com",         // Email (optional)
  "role": "GestorONG",                 // Role (GestorONG ou Doador)
  "iat": 1700000000,                   // Issued At (timestamp)
  "exp": 1799999999                    // Expiration (timestamp)
}
```

## Exemplo Completo com curl

```bash
# Gerar token (usando jwt.io ou o script C# acima)
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

# Criar campanha
curl -X POST \
  https://localhost:7000/api/campaigns \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Minha Campanha",
    "description": "Descrição",
    "financialTarget": 1000,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T23:59:59Z"
  }' \
  -k  # -k ignora SSL self-signed

# Obter campanhas públicas
curl -X GET \
  https://localhost:7000/api/transparency/campaigns \
  -k
```

## Debugando Tokens

### Verificar conteúdo do token

```csharp
var tokenHandler = new JwtSecurityTokenHandler();
var token = tokenHandler.ReadToken("seu_token_aqui") as JwtSecurityToken;

foreach (var claim in token.Claims)
{
    Console.WriteLine($"{claim.Type}: {claim.Value}");
}
```

### Verificar expiração

```csharp
var exp = token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
var expirationTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
    .AddSeconds(long.Parse(exp));
Console.WriteLine($"Token expires at: {expirationTime}");
```
