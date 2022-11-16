namespace Vayosoft.Threading.Channels.Diagnostics
{
    public class HandlerMetricsSnapshot : IMetricsSnapshot
    {
        public int Length { get; set; }
        public long MeasurementTimeMs { get; set; }
        public long MaxTimeMs { get; set; }
        public long MinTimeMs { get; set; }
        public long OperationCount { get; set; }
    }
}
