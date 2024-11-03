using System.Diagnostics;

namespace ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement
{
    public class ServerStatisticsRepository
    {
        private ServerStatistics ServerStatistics { get; set; }
        private static PerformanceCounter CpuCounter = new("Processor", "% Processor Time", "_Total");
        private static PerformanceCounter RamCounter = new("Memory", "Available MBytes");
        private static PerformanceCounter TotalMemoryCounter = new("Memory", "Committed Bytes");

        public ServerStatisticsRepository()
        {
            ServerStatistics = new ServerStatistics();
        }
        public ServerStatistics UpdateStatistics()
        {
            ServerStatistics.MemoryUsage = GetMemoryUsage();
            ServerStatistics.AvailableMemory = GetAvailableMemory();
            ServerStatistics.CpuUsage = GetCpuUsage();
            ServerStatistics.Timestamp = DateTime.Now;
            return ServerStatistics;
        }

        public double GetMemoryUsage()
        {
            double totalMemory = TotalMemoryCounter.NextValue() / (1024 * 1024);
            double availableMemory = GetAvailableMemory();
            double memoryUsage = totalMemory - availableMemory;
            return memoryUsage;
        }

        public double GetAvailableMemory()
        {
            return RamCounter.NextValue();
        }

        public double GetCpuUsage()
        {
            return CpuCounter.NextValue();
        }

        public void ShowServerStatistics()
        {
            Console.WriteLine(ServerStatistics);
        }
    }
}
