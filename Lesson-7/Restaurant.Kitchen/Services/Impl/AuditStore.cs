using MassTransit.Audit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Restaurant.Kitchen.Services.Impl;

public class AuditStore : IMessageAuditStore
{
    private readonly ILogger _logger;
    public AuditStore(ILogger<AuditStore> logger)
    {
        _logger = logger;
    }

    public Task StoreMessage<T>(T message, MessageAuditMetadata metadata) where T : class
    {
        _logger.LogInformation(JsonSerializer.Serialize(metadata) + "\n" + JsonSerializer.Serialize(message));

        return Task.CompletedTask;
    }
}
