# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта из папки ManagementSystem
COPY ["ManagementSystem/ManagementSystem.csproj", "ManagementSystem/"]
RUN dotnet restore "ManagementSystem/ManagementSystem.csproj"

# Копируем всё остальное
COPY . .
WORKDIR "/src/ManagementSystem"
RUN dotnet build "ManagementSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ManagementSystem.csproj" -c Release -o /app/publish

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ManagementSystem.dll"]