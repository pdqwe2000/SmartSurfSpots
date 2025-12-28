# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solution e todos os projetos
COPY *.sln .
COPY SmartSurfSpots.API/*.csproj ./SmartSurfSpots.API/
COPY SmartSurfSpots.Services/*.csproj ./SmartSurfSpots.Services/
COPY SmartSurfSpots.Data/*.csproj ./SmartSurfSpots.Data/
COPY SmartSurfSpots.Domain/*.csproj ./SmartSurfSpots.Domain/

# Restore
RUN dotnet restore

# Copiar todo o código
COPY . .

# Build
WORKDIR /src/SmartSurfSpots.API
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar aplicação publicada
COPY --from=build /app/publish .

# Expor porta
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Executar
ENTRYPOINT ["dotnet", "SmartSurfSpots.API.dll"]