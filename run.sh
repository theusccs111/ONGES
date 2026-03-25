#!/bin/bash

# Script para compilar e rodar a aplicação ONGES Campaign API

set -e

echo "🏗️  Restaurando dependências..."
dotnet restore

echo "🔨 Compilando solução..."
dotnet build --configuration Release

echo "📦 Aplicando migrações do banco de dados..."
cd ONGES.Campaign.API
dotnet ef database update --project ../ONGES.Campaign.Infrastructure

echo "🚀 Iniciando aplicação..."
dotnet run

echo "✅ Pronto! Acesse: https://localhost:7000/swagger"
