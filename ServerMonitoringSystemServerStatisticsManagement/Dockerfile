# Use the .NET 8.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj and restore dependencies
COPY ServerMonitoringSystemServerStatisticsManagement/*.csproj ./
RUN dotnet restore

# Copy the rest of the application files and build the application
COPY ServerMonitoringSystemServerStatisticsManagement/. ./
RUN dotnet publish -c Release -o /out

# Use the .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV STATISTICS_DB_CONNECTION="mongodb://mongodb:27017"

# Expose the port and run the application
EXPOSE 80
ENTRYPOINT ["dotnet", "ServerMonitoringSystemServerStatisticsManagement.dll"]