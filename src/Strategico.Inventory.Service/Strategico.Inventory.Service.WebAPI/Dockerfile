# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj(s) first and restore
COPY ["Strategico.Inventory.Service.WebAPI/Strategico.Inventory.Service.WebAPI.csproj", "Strategico.Inventory.Service.WebAPI/"]
RUN dotnet restore "./Strategico.Inventory.Service.WebAPI/Strategico.Inventory.Service.WebAPI.csproj" \
    --packages /root/.nuget/packages

# Copy everything else
COPY . .
WORKDIR "/src/Strategico.Inventory.Service.WebAPI"

# Build
RUN dotnet build "./Strategico.Inventory.Service.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build \
    --no-restore

# Publish
FROM build AS publish
RUN dotnet publish "./Strategico.Inventory.Service.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false \
    --no-restore

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Strategico.Inventory.Service.WebAPI.dll"]
