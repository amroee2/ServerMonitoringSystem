FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ServerMonitoringSystemSignalRManagement/*.csproj ./
RUN dotnet restore

COPY ServerMonitoringSystemSignalRManagement/. ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV SIGNALR_SERVER_URL="http://signalr-management:80"

EXPOSE 80
ENTRYPOINT ["dotnet", "ServerMonitoringSystemSignalRManagement.dll"]