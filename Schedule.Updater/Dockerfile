FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER root
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Schedule.Updater/Schedule.Updater.csproj", "Schedule.Updater/"]
COPY ["Schedule.Shared/Schedule.Shared.csproj", "Schedule.Shared/"]
RUN dotnet restore "Schedule.Updater/Schedule.Updater.csproj"
COPY . .
WORKDIR "/src/Schedule.Updater"
RUN dotnet build "./Schedule.Updater.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Schedule.Updater.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Schedule.Updater.dll"]
