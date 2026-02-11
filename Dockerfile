FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Теперь мы копируем из подпапки, так как Dockerfile снаружи
COPY ["ManagementSystem/ManagementSystem.csproj", "ManagementSystem/"]
RUN dotnet restore "ManagementSystem/ManagementSystem.csproj"

COPY . .
WORKDIR "/src/ManagementSystem"
RUN dotnet publish "ManagementSystem.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ManagementSystem.dll"]