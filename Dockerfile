FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только файлы проектов для восстановления зависимостей
COPY ["AutoPlanner.sln", "./"]
COPY ["AutoPlannerApi/AutoPlannerApi.csproj", "AutoPlannerApi/"]
COPY ["AutoPlannerCore/AutoPlannerCore.csproj", "AutoPlannerCore/"]
COPY ["AutoPlannerCore.Test/AutoPlannerCore.Test.csproj", "AutoPlannerCore.Test/"]

# Восстанавливаем с очисткой кэша
RUN dotnet restore "AutoPlanner.sln" --force

# Копируем остальной код
COPY . .

# Собираем и публикуем за один шаг (минуя отдельный build)
WORKDIR "/src/AutoPlannerApi"
RUN dotnet publish "AutoPlannerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS="http://*:8080"
EXPOSE 8080
ENTRYPOINT ["dotnet", "AutoPlannerApi.dll"]