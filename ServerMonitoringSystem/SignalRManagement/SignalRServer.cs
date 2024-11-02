using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServerMonitoringSystem.SignalRManagement
{
    public class SignalRServer
    {
        private readonly IHost _host;

        public SignalRServer(string url = "http://localhost:5000")
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSignalR();
                    services.AddCors(options =>
                    {
                        options.AddPolicy("AllowSpecificOrigins",
                            builder =>
                            {
                                builder.WithOrigins("http://localhost:5000")
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()
                                       .AllowCredentials();
                            });
                    });

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(url);
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseCors("AllowSpecificOrigins");
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<ChatHub>("/chatHub");
                        });

                    });
                })
                .Build();
        }

        public async Task StartAsync()
        {
            _ = _host.RunAsync();
            Console.WriteLine("SignalR Server started.");

        }

        public async Task StopAsync()
        {
            await _host.StopAsync();
        }
    }
}