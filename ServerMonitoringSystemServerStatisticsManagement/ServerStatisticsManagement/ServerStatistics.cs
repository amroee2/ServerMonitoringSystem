namespace ServerMonitoringSystemServerStatisticsManagement.ServerStatisticsManagement
{
    public class ServerStatistics
    {
        public string ServerIdentifier { get; set; }
        public double MemoryUsage { get; set; }
        public double AvailableMemory { get; set; }
        public double CpuUsage { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"ServerIdentifier: {ServerIdentifier}, Memory Usage: {MemoryUsage}MB, Available Memory: {AvailableMemory}MB, CPU Usage: {CpuUsage}%, Timestamp: {Timestamp}";
        }
    }
}
