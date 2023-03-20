using System;
using PassChallenge.Core.Extensions;
using PassChallenge.Core.Handlers;
using PassChallenge.Core.Solver;

namespace PassChallenge.Connectors.InMemory.Extensions;

public static class ChallengeSolverBuilderExtensions
{
    public static ChallengeSolverBuilder<InMemoryProducer> SetupInMemoryProducer(
        this ChallengeSolverBuilder<InMemoryProducer> challengeSolverBuilder,
        Action<InMemoryProducerBuilder> configure)
    {
        InMemoryProducerBuilder builder = new();
        configure.Invoke(builder);

        IChallengeHandlerDescriptorAvailableStorage descriptorStorage = builder.DescriptorStorageBuilder.Build();

        challengeSolverBuilder.AvailableChallengeAndSolutionStorageBuilder.SetStorage(descriptorStorage);
        challengeSolverBuilder.SetChallengeHandlerFactory(new ChallengeHandlerFactory(descriptorStorage));

        return challengeSolverBuilder;
    }
}