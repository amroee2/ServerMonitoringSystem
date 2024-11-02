using Microsoft.AspNetCore.SignalR.Client;

namespace ServerMonitoringSystem.SignalRManagement
{
    public class SignalRClient : ISignalRService
    {
        private readonly HubConnection _connection;

        public SignalRClient(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();
        }

        public async Task ConnectAsync()
        {
            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"{user}: {message}");
            });

            try
            {
                await _connection.StartAsync();
                Console.WriteLine("Connected to the SignalR hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string user, string message)
        {
            try
            {
                await _connection.InvokeAsync("SendMessage", user, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                await _connection.StopAsync();
                Console.WriteLine("Disconnected from the SignalR hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disconnecting: {ex.Message}");
            }
        }
    }
}
