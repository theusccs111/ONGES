// Script para criar a primeira migração do projeto
// Execute na pasta ONGES.Campaign.API:
// 
// dotnet ef migrations add InitialCreate --project ../ONGES.Campaign.Infrastructure --output-dir Persistence/Migrations
// dotnet ef database update --project ../ONGES.Campaign.Infrastructure

// Comandos úteis:
// 
// Criar migration:
// dotnet ef migrations add NomeDaMigracao --project ../ONGES.Campaign.Infrastructure --output-dir Persistence/Migrations
//
// Aplicar migration:
// dotnet ef database update --project ../ONGES.Campaign.Infrastructure
//
// Remover última migration:
// dotnet ef migrations remove --project ../ONGES.Campaign.Infrastructure
//
// Ver migrations aplicadas:
// dotnet ef migrations list --project ../ONGES.Campaign.Infrastructure
