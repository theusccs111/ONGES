FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ONGES.Campaign.API/ONGES.Campaign.API.csproj", "ONGES.Campaign.API/"]
COPY ["ONGES.Campaign.Application/ONGES.Campaign.Application.csproj", "ONGES.Campaign.Application/"]
COPY ["ONGES.Campaign.Infrastructure/ONGES.Campaign.Infrastructure.csproj", "ONGES.Campaign.Infrastructure/"]
COPY ["ONGES.Campaign.Domain/ONGES.Campaign.Domain.csproj", "ONGES.Campaign.Domain/"]

RUN dotnet restore "ONGES.Campaign.API/ONGES.Campaign.API.csproj"

COPY . .
WORKDIR /src/ONGES.Campaign.API
RUN dotnet publish "ONGES.Campaign.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ONGES.Campaign.API.dll"]