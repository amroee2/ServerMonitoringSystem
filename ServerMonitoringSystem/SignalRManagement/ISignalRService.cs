namespace ServerMonitoringSystem.SignalRManagement
{
    public interface ISignalRService
    {
        Task ConnectAsync();
        Task SendMessageAsync(string user, string message);
        Task DisconnectAsync();
    }
}
