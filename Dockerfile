# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY InventoryManager.sln .

COPY InventoryManager.API/*.csproj InventoryManager.API/
COPY InventoryManager.Application/*.csproj InventoryManager.Application/
COPY InventoryManager.Infrastructure/*.csproj InventoryManager.Infrastructure/
COPY InventoryManager.Domain/*.csproj InventoryManager.Domain/
COPY InventoryManager.Worker/*.csproj InventoryManager.Worker/

RUN dotnet restore InventoryManager.sln

COPY . .

WORKDIR /src/InventoryManager.API
RUN dotnet publish InventoryManager.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:${PORT}
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InventoryManager.API.dll"]