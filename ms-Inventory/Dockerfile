# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ms-Inventory.csproj", "./"]
RUN dotnet restore "ms-Inventory.csproj"
COPY . .
RUN dotnet publish "ms-Inventory.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ms-Inventory.dll"]
