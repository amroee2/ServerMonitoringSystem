FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ServerMonitoringSystemServerStatisticsManagement/*.csproj ./ServerMonitoringSystemServerStatisticsManagement/
COPY Models/*.csproj ./Models/
RUN dotnet restore ServerMonitoringSystemServerStatisticsManagement/ServerMonitoringSystemServerStatisticsManagement.csproj

COPY . .
RUN dotnet publish ServerMonitoringSystemServerStatisticsManagement/ServerMonitoringSystemServerStatisticsManagement.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV MESSAGE_QUEUE_HOST="rabbitmq"
ENV MESSAGE_QUEUE_PORT=5672

ENV SAMPLING_INTERVAL_SECONDS=60
ENV SERVER_IDENTIFIER="linux1"
EXPOSE 80
ENTRYPOINT ["dotnet", "ServerMonitoringSystemServerStatisticsManagement.dll"]