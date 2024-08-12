using App.Metrics.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMall.Framework.PGAccessor.WebExample
{
    public class SampleHealthCheck : HealthCheck
    {
        /// <inheritdoc />
        public SampleHealthCheck() : base("Random Health Check")
        {
        }

        /// <inheritdoc />
        protected override Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DateTime.UtcNow.Second <= 20)
            {
                return Task.FromResult(HealthCheckResult.Degraded());
            }

            if (DateTime.UtcNow.Second >= 40)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
