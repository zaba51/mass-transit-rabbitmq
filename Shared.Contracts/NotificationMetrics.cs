using System.Diagnostics.Metrics;

namespace Shared.Contracts
{
    public class NotificationMetrics
    {
        private readonly Counter<long> _successCounter;
        private readonly Counter<long> _failureCounter;

        public NotificationMetrics(Meter meter, string name)
        {
            _successCounter = meter.CreateCounter<long>($"{name}_success_count");
            _failureCounter = meter.CreateCounter<long>($"{name}_failure_count");
        }

        public void RecordSuccess() => _successCounter.Add(1);
        public void RecordFailure() => _failureCounter.Add(1);
    }
}
