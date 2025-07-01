# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solo el proyecto principal (sin tests)
COPY ["ms-Inventory.csproj", "./"]
COPY ["Inventory.Api", "./Inventory.Api/"]
COPY ["Inventory.Interfaces", "./Inventory.Interfaces/"]
COPY ["Program.cs", "./"]
COPY ["appsettings.json", "./"]
COPY ["appsettings.Development.json", "./"]

# Restaurar dependencias y publicar
RUN dotnet restore "ms-Inventory.csproj"
RUN dotnet publish "ms-Inventory.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ms-Inventory.dll"]
