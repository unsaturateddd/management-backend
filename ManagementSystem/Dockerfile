# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["ManagementSystem.csproj", "./"]
RUN dotnet restore "./ManagementSystem.csproj"

# Копируем остальные файлы и собираем
COPY . .
RUN dotnet publish "ManagementSystem.csproj" -c Release -o /app/publish

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render передает порт через переменную окружения PORT
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ManagementSystem.dll"]