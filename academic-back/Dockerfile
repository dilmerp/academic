# 1. Etapa base para ejecutar la aplicación (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# 2. Etapa de construcción (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamos los archivos de proyecto para restaurar dependencias
COPY ["Academic.Api/Academic.Api.csproj", "Academic.Api/"]
COPY ["Academic.Domain/Academic.Domain.csproj", "Academic.Domain/"]
COPY ["Academic.Persistence/Academic.Persistence.csproj", "Academic.Persistence/"]

# Restauramos los paquetes NuGet
RUN dotnet restore "./Academic.Api/Academic.Api.csproj"

# Copiamos el resto del código fuente de toda la solución
COPY . .

# Construimos el proyecto
WORKDIR "/src/Academic.Api"
RUN dotnet build "./Academic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Etapa de publicación (Publish)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Academic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Etapa final (Producción)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Comando de arranque de la API
ENTRYPOINT ["dotnet", "Academic.Api.dll"]