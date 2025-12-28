# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar ficheiros de projeto
COPY ["SmartSurfSpots.API/SmartSurfSpots.API.csproj", "SmartSurfSpots.API/"]
COPY ["SmartSurfSpots.Services/SmartSurfSpots.Services.csproj", "SmartSurfSpots.Services/"]
COPY ["SmartSurfSpots.Data/SmartSurfSpots.Data.csproj", "SmartSurfSpots.Data/"]
COPY ["SmartSurfSpots.Domain/SmartSurfSpots.Domain.csproj", "SmartSurfSpots.Domain/"]

# Restore
RUN dotnet restore "SmartSurfSpots.API/SmartSurfSpots.API.csproj"

# Copiar todo o código
COPY . .

# Build
WORKDIR "/src/SmartSurfSpots.API"
RUN dotnet build "SmartSurfSpots.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "SmartSurfSpots.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartSurfSpots.API.dll"]