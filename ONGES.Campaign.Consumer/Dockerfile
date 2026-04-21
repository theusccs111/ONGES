FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ONGES.Campaign.Consumer/ONGES.Campaign.Consumer.csproj ONGES.Campaign.Consumer/
COPY ONGES.Campaign.Application/ONGES.Campaign.Application.csproj ONGES.Campaign.Application/
COPY ONGES.Campaign.Infrastructure/ONGES.Campaign.Infrastructure.csproj ONGES.Campaign.Infrastructure/
COPY ONGES.Campaign.Domain/ONGES.Campaign.Domain.csproj ONGES.Campaign.Domain/

RUN dotnet restore "ONGES.Campaign.Consumer/ONGES.Campaign.Consumer.csproj"

COPY . .
WORKDIR /src/ONGES.Campaign.Consumer
RUN dotnet publish "ONGES.Campaign.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ONGES.Campaign.Consumer.dll"]
