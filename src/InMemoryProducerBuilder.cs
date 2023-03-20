using PassChallenge.Core.Handlers;

namespace PassChallenge.Connectors.InMemory;

public class InMemoryProducerBuilder : IBuilderWithChallengeHandlers
{
    public ChallengeHandlerDescriptorStorageBuilder DescriptorStorageBuilder { get; } = new();
}