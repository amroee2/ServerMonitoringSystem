FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ServerMonitoringSystemMessageQueueServices/*.csproj ./ServerMonitoringSystemMessageQueueServices/
COPY Models/*.csproj ./Models/
COPY ServerMonitoringSystemSignalRManagement/*.csproj ./ServerMonitoringSystemSignalRManagement/
COPY ServerMonitoringSystemServerStatisticsManagement/*.csproj ./ServerMonitoringSystemServerStatisticsManagement/
RUN dotnet restore ServerMonitoringSystemMessageQueueServices/ServerMonitoringSystemMessageQueueServices.csproj

COPY . .
RUN dotnet publish ServerMonitoringSystemMessageQueueServices/ServerMonitoringSystemMessageQueueServices.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV MESSAGE_QUEUE_HOST="rabbitmq"
ENV MESSAGE_QUEUE_PORT=5672

ENV ASPNETCORE_ENVIRONMENT=Production
ENV CONNECTION_STRING="mongodb://mongodb:27017"

ENV CPU_USAGE_ANOMALY_THRESHOLD_PERCENTAGE=0.4
ENV MEMORY_USAGE_ANOMALY_THRESHOLD_PERCENTAGE=0.5
ENV MEMORY_USAGE_THRESHOLD_PERCENTAGE=0.8
ENV CPU_USAGE_THRESHOLD_PERCENTAGE=0.9

ENV SIGNALR_CLIENT_HUB_URL="http://signalr-management:80/chatHub"
EXPOSE 80
ENTRYPOINT ["dotnet", "ServerMonitoringSystemMessageQueueServices.dll"]