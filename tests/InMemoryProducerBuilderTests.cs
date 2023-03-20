using PassChallenge.Core.Challenges;
using PassChallenge.Core.Solutions;
using Moq;
using NUnit.Framework;

namespace PassChallenge.Connectors.InMemory.Tests;

public class InMemoryProducerBuilderTests
{
    [Test]
    public async Task AddChallengeHandler_With_HandlerFunc_Is_Correct()
    {
        InMemoryProducerBuilder builder = new();
        Mock<Func<Task<ISolution>>> mock = new();

        builder.DescriptorStorageBuilder.AddChallengeHandler<IChallenge, ISolution>((_, _) => mock.Object.Invoke());

        var descriptorStorage = builder.DescriptorStorageBuilder.Build();

        Assert.That(descriptorStorage.Descriptors.Count, Is.EqualTo(1));

        var namedDescriptors = descriptorStorage.GetDescriptors<IChallenge, ISolution>().ToList();
        Assert.That(namedDescriptors.Count, Is.EqualTo(1));

        var descriptor = namedDescriptors.First();

        Assert.Multiple(() =>
        {
            Assert.That(descriptor.HandlerName, Is.EqualTo($"{nameof(IChallenge)}-{nameof(ISolution)}-0".ToLower()));
            Assert.That(descriptor.ChallengeType, Is.EqualTo(typeof(IChallenge)));
            Assert.That(descriptor.SolutionType, Is.EqualTo(typeof(ISolution)));
            Assert.Null(descriptor.HandlerType);
            Assert.Null(descriptor.ImplementationFactory);
            Assert.NotNull(descriptor.SolverFunction);
        });

        Mock<IServiceProvider> serviceProviderMock = new();
        await (Task<ISolution>)descriptor.SolverFunction!.Invoke(serviceProviderMock.Object,
            It.IsAny<IChallenge>());

        mock.Verify(x => x.Invoke(), Times.Once);
    }
}