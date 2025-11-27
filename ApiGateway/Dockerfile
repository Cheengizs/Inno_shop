# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем все файлы проекта
COPY . .

# Восстанавливаем зависимости
RUN dotnet restore ApiGateway.csproj

# Сборка проекта в Release
RUN dotnet build ApiGateway.csproj -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish ApiGateway.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=publish /app/publish .

# Открываем порт 80
EXPOSE 80

# Точка входа
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
