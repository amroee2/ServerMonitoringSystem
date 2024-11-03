# Use the .NET 8.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy .csproj files and restore dependencies
COPY ServerMonitoringSystemMessageQueueServices/*.csproj ./ServerMonitoringSystemMessageQueueServices/
COPY ServerMonitoringSystemServerStatisticsManagement/*.csproj ./ServerMonitoringSystemServerStatisticsManagement/
COPY ServerMonitoringSystemSignalRManagement/*.csproj ./ServerMonitoringSystemSignalRManagement/
RUN dotnet restore ServerMonitoringSystemMessageQueueServices/ServerMonitoringSystemMessageQueueServices.csproj

# Copy the remaining files and publish the project
COPY . .
RUN dotnet publish ServerMonitoringSystemMessageQueueServices/ServerMonitoringSystemMessageQueueServices.csproj -c Release -o /out

# Use the .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV MESSAGE_QUEUE_HOST="rabbitmq"
ENV MESSAGE_QUEUE_PORT=5672

# Expose the port and run the application
EXPOSE 80
ENTRYPOINT ["dotnet", "ServerMonitoringSystemMessageQueueServices.dll"]