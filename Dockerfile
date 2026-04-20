# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos archivos de proyecto primero para aprovechar cache
COPY ["FutbolLeague.API/FutbolLeague.API.csproj", "FutbolLeague.API/"]
COPY ["FutbolLeague.Application/FutbolLeague.Application.csproj", "FutbolLeague.Application/"]
COPY ["FutbolLeague.Domain/FutbolLeague.Domain.csproj", "FutbolLeague.Domain/"]
COPY ["FutbolLeague.Infrastructure/FutbolLeague.Infrastructure.csproj", "FutbolLeague.Infrastructure/"]

RUN dotnet restore "FutbolLeague.API/FutbolLeague.API.csproj"

# Copiamos todo el código
COPY . .

WORKDIR "/src/FutbolLeague.API"
RUN dotnet publish "FutbolLeague.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# En contenedores .NET modernos conviene declarar el puerto explícitamente
ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FutbolLeague.API.dll"]