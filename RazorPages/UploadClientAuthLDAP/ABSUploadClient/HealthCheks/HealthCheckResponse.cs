using System;
using System.Collections.Generic;

namespace ABSUploadClient.HealthCheks
{
    internal class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
        public TimeSpan Duration { get; set; }
        public string Version { get; set; }
        public string LogLevel { get; internal set; }
    }
}
