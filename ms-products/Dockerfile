# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy the project file and restore
COPY ["ms-products.csproj", "./"]
RUN dotnet restore "ms-products.csproj"

# copy everything else and build
COPY . .
RUN dotnet publish "ms-products.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ms-products.dll"]

