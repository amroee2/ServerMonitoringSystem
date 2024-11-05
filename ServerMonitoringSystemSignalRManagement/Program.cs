using ServerMonitoringSystemSignalRManagement.SignalRManagement;

string url = Environment.GetEnvironmentVariable("SIGNALR_SERVER_URL");
Console.WriteLine(url);
var server = new SignalRServer(url);
await server.StartAsync();