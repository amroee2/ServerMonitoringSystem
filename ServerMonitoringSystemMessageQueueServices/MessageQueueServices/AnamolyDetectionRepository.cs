﻿using Models.StatisticsCollectors;
using ServerMonitoringSystemSignalRManagement.SignalRManagement;

namespace ServerMonitoringSystemMessageQueueServices.MessageQueueServices
{
    public class AnamolyDetectionRepository
    {

        private readonly AnamolyThresholdConfig _anamolyThresholdConfig;
        private readonly ISignalRService _signalRService;
        public AnamolyDetectionRepository(AnamolyThresholdConfig anamolyThresholdConfig, ISignalRService signalRService)
        {
            _anamolyThresholdConfig = anamolyThresholdConfig;
            _signalRService = signalRService;
        }
        public void DetectAnamoly(ServerStatistics currentServerStatistics, ServerStatistics previousServerStatistics)
        {
            if (currentServerStatistics.CpuUsage > previousServerStatistics.CpuUsage * (1 + _anamolyThresholdConfig.CpuUsageAnomalyThresholdPercentage))
            {
                _signalRService.SendMessageAsync("amro", $"Anamoly Alert: Cpu Usage Anamoly detected with Identifier {currentServerStatistics.ServerIdentifier}");
            }
            if (currentServerStatistics.MemoryUsage > previousServerStatistics.MemoryUsage * (1 + _anamolyThresholdConfig.MemoryUsageAnomalyThresholdPercentage))
            {
                _signalRService.SendMessageAsync("amro", $"Anamoly Alert: Memory Usage Anamoly detected with Identifier {currentServerStatistics.ServerIdentifier}");
            }
        }

        public void DetectHighUsage(ServerStatistics currentServerStatistics)
        {
            if (currentServerStatistics.MemoryUsage / (currentServerStatistics.MemoryUsage + currentServerStatistics.AvailableMemory) > _anamolyThresholdConfig.MemoryUsageThresholdPercentage)
            {
 
                _signalRService.SendMessageAsync("amro", $"High Usage Alert: Memory Usage High Usage detected with Identifier {currentServerStatistics.ServerIdentifier}");
            }
            if (currentServerStatistics.CpuUsage > _anamolyThresholdConfig.CpuUsageThresholdPercentage)
            {
                _signalRService.SendMessageAsync("amro", $"High Usage Alert: Cpu Usage High Usage detected with Identifier {currentServerStatistics.ServerIdentifier}");
            }
        }
    }
}
