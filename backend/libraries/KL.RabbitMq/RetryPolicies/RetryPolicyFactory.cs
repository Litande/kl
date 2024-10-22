using KL.RabbitMq.Messaging;
using Microsoft.Extensions.Logging;

namespace KL.RabbitMq.RetryPolicies;

internal class RetryPolicyFactory(ILogger<IRabbitRetryPolicy> logger)
{
    public IRabbitRetryPolicy Create(RetryPolicyConfiguration? policyConfiguration)
    {
        return policyConfiguration?.Type switch
        {
            RetryPolicyType.ExponentialBackoff => new RabbitRetryExponentialBackoffPolicy(logger, policyConfiguration.MaxRetry),
            _ => new DefaultRabbitRetryPolicy(logger)
        };
    }
}
