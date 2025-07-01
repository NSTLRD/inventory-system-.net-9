# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solo el proyecto principal (sin tests)
COPY ["ms-products.csproj", "./"]
COPY ["Products.api", "./Products.api/"]
COPY ["Products.Interfaces", "./Products.Interfaces/"]
COPY ["Program.cs", "./"]
COPY ["appsettings.json", "./"]
COPY ["appsettings.Development.json", "./"]
COPY ["Migrations", "./Migrations/"]

# Restaurar dependencias y publicar
RUN dotnet restore "ms-products.csproj" 
RUN dotnet publish "ms-products.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ms-products.dll"]

