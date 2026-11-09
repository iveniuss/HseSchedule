FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ScheduleAPI/ScheduleAPI.csproj", "ScheduleAPI/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "ScheduleAPI/ScheduleAPI.csproj"
COPY . .
WORKDIR "/src/ScheduleAPI"
RUN dotnet build "./ScheduleAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ScheduleAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScheduleAPI.dll"]
