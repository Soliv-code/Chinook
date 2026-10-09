# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем файлы проектов для восстановления пакетов (кэш слоев Docker)
COPY ["Chinook.Domain/Chinook.Domain.csproj", "Chinook.Domain/"]
COPY ["Chinook.Application/Chinook.Application.csproj", "Chinook.Application/"]
COPY ["Chinook.Infrastructure/Chinook.Infrastructure.csproj", "Chinook.Infrastructure/"]
COPY ["Chinook.API/Chinook.API.csproj", "Chinook.API/"]

RUN dotnet restore "Chinook.API/Chinook.API.csproj"

# Копируем весь исходный код
COPY . .

WORKDIR "/src/Chinook.API"
RUN dotnet build "Chinook.API.csproj" -c Release -o /app/build

# Этап 2: Публикация
FROM build AS publish
RUN dotnet publish "Chinook.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап 3: Финальный образ (легкий и безопасный)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Открываем порты (8080 для HTTP, 8081 для HTTPS по умолчанию в .NET)
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Chinook.API.dll"]