using System.Diagnostics;

namespace Models.StatisticsCollectors
{
    public class WindowsServerStatisticsRepository
    {
        public ServerStatistics ServerStatistics { get; set; }
        private static PerformanceCounter CpuCounter;
        private static PerformanceCounter RamCounter;
        private static PerformanceCounter TotalMemoryCounter;

        public WindowsServerStatisticsRepository()
        {
            CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            RamCounter = new PerformanceCounter("Memory", "Available MBytes");
            TotalMemoryCounter = new PerformanceCounter("Memory", "Committed Bytes");
            ServerStatistics = new ServerStatistics();
            UpdateStatistics();

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
    }
}
