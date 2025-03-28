﻿namespace Models.StatisticsCollectors
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
            return $"ServerIdentifier: {ServerIdentifier}, MemoryUsage: {MemoryUsage}, AvailableMemory: {AvailableMemory}, CpuUsage: {CpuUsage}, Timestamp: {Timestamp}";
        }
    }
}
