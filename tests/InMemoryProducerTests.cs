using PassChallenge.Connectors.InMemory;
using PassChallenge.Connectors.InMemory.Tests.Tools;
using PassChallenge.Core.Challenges;
using PassChallenge.Core.Handlers;
using PassChallenge.Core.Solutions;
using PassChallenge.Core.Solver;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace PassChallenge.Connectors.InMemory.Tests;

public class InMemoryProducerTests
{
    [Test]
    public void Constructor_Is_Correct()
    {
        Mock<IServiceProvider> mock = new();
        InMemoryProducer _ = new(mock.Object);
        Assert.Pass();
    }

    [Test]
    public void Constructor_When_ServiceProvider_Is_Null_Throws_ArgumentNullException()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<ArgumentNullException>(() => new InMemoryProducer(null!));
    }

    [Test]
    public void SetChallengeHandlerFactory_Is_Correct()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<IChallengeHandlerFactory> challengeHandlerFactoryMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetChallengeHandlerFactory(challengeHandlerFactoryMock.Object);

        Assert.Pass();
    }

    [Test]
    public void SetChallengeHandlerFactory_When_ChallengeHandlerFactory_Is_Null_Throws_ArgumentNullException()
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);

        Assert.Throws<ArgumentNullException>(() => producer.SetChallengeHandlerFactory(null!));
    }

    [Test]
    public void SetAvailableChallengeAndSolutionStorage_Is_Correct()
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetAvailableChallengeAndSolutionStorage(It.IsAny<IAvailableChallengeAndSolutionStorage>());

        Assert.Pass();
    }

    [Test]
    public void CanProduce_Is_Correct()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<IChallengeHandlerFactory> challengeHandlerFactoryMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetChallengeHandlerFactory(challengeHandlerFactoryMock.Object);
        producer.CanProduce<IChallenge, ISolution>();

        challengeHandlerFactoryMock.Verify(x => x.CanProduce<IChallenge, ISolution>(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void CanProduce_When_ChallengeHandlerFactory_Is_Not_Sets_Throws_InvalidOperationException()
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);

        Assert.Throws<InvalidOperationException>(() => producer.CanProduce<IChallenge, ISolution>(It.IsAny<string>()));
    }

    [Test]
    public void GetDefaultHandlerName_Is_Correct()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<IChallengeHandlerFactory> challengeHandlerFactoryMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetChallengeHandlerFactory(challengeHandlerFactoryMock.Object);
        producer.GetDefaultHandlerName<IChallenge, ISolution>();

        challengeHandlerFactoryMock.Verify(x => x.GetDefaultHandlerName<IChallenge, ISolution>(), Times.Once);
    }

    [Test]
    public void GetDefaultHandlerName_When_ChallengeHandlerFactory_Is_Not_Sets_Throws_InvalidOperationException()
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);

        Assert.Throws<InvalidOperationException>(() => producer.GetDefaultHandlerName<IChallenge, ISolution>());
    }

    [Test]
    public void GetHandlerNames_Is_Correct()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<IChallengeHandlerFactory> challengeHandlerFactoryMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetChallengeHandlerFactory(challengeHandlerFactoryMock.Object);
        producer.GetHandlerNames<IChallenge, ISolution>();

        challengeHandlerFactoryMock.Verify(x => x.GetHandlerNames<IChallenge, ISolution>(), Times.Once);
    }

    [Test]
    public void GetHandlerNames_When_ChallengeHandlerFactory_Is_Not_Sets_Throws_InvalidOperationException()
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);

        Assert.Throws<InvalidOperationException>(() => producer.GetHandlerNames<IChallenge, ISolution>());
    }

    [Test]
    public async Task ProduceAndWaitSolution_With_Default_HandleName_Is_Correct()
    {
        Mock<InMemoryProducer> mock = new(new ServiceCollection().BuildServiceProvider());
        Mock<IChallengeHandlerFactory> challengeHandlerFactoryMock = new();
        Mock<IChallengeHandler<IChallenge, ISolution>> challengeHandlerMock = new();

        challengeHandlerFactoryMock
            .Setup(x => x.CreateHandler<IChallenge, ISolution>(It.IsAny<IServiceProvider>(), It.IsAny<string>()))
            .Returns(challengeHandlerMock.Object);

        mock.Object.SetChallengeHandlerFactory(challengeHandlerFactoryMock.Object);

        IChallenge expectedChallenge = new TestChallenge();
        CancellationToken expectedCancellationToken = new CancellationTokenSource(TimeSpan.FromDays(1)).Token;

        await mock.Object.ProduceAndWaitSolution<IChallenge, ISolution>(expectedChallenge, 
            It.IsAny<string>(), expectedCancellationToken);

        challengeHandlerFactoryMock.Verify(x => x.CreateHandler<IChallenge, ISolution>(
            It.IsAny<IServiceProvider>(),
            It.Is<string>(mo => mo == default)));

        challengeHandlerMock.Verify(x => x.Handle(
            It.Is<IChallenge>(mo => mo == expectedChallenge),
            It.Is<CancellationToken>(mo => mo == expectedCancellationToken)));
    }

    [Test]
    public async Task ProduceAndWaitSolution_With_Concrete_HandleName_Is_Correct()
    {
        Mock<InMemoryProducer> mock = new(new ServiceCollection().BuildServiceProvider());
        Mock<IChallengeHandlerFactory> challengeHandlerFactoryMock = new();
        Mock<IChallengeHandler<IChallenge, ISolution>> challengeHandlerMock = new();

        challengeHandlerFactoryMock
            .Setup(x => x.CreateHandler<IChallenge, ISolution>(It.IsAny<IServiceProvider>(), It.IsAny<string>()))
            .Returns(challengeHandlerMock.Object);

        mock.Object.SetChallengeHandlerFactory(challengeHandlerFactoryMock.Object);

        IChallenge expectedChallenge = new TestChallenge();
        CancellationToken expectedCancellationToken = new CancellationTokenSource(TimeSpan.FromDays(1)).Token;
        string expectedHandlerName = "handlerName";

        await mock.Object.ProduceAndWaitSolution<IChallenge, ISolution>(expectedChallenge, expectedHandlerName,
            expectedCancellationToken);

        challengeHandlerFactoryMock.Verify(x => x.CreateHandler<IChallenge, ISolution>(
            It.IsAny<IServiceProvider>(),
            It.Is<string>(mo => mo == expectedHandlerName)));

        challengeHandlerMock.Verify(x => x.Handle(
            It.Is<IChallenge>(mo => mo == expectedChallenge),
            It.Is<CancellationToken>(mo => mo == expectedCancellationToken)));
    }

    [Test]
    public void ProduceAndWaitSolution_When_ChallengeHandlerFactory_Is_Not_Sets_Throws_InvalidOperationException()
    {
        Mock<InMemoryProducer> mock = new(new ServiceCollection().BuildServiceProvider());

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await mock.Object.ProduceAndWaitSolution<IChallenge, ISolution>(It.IsAny<IChallenge>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()));
    }

    [Test]
    public void ProduceAndWaitSolution_When_Challenge_Is_Null_Throws_ArgumentNullException()
    {
        Mock<InMemoryProducer> mock = new(new ServiceCollection().BuildServiceProvider());
        Mock<IChallengeHandlerFactory> challengeHandlerFactoryMock = new();

        mock.Object.SetChallengeHandlerFactory(challengeHandlerFactoryMock.Object);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await mock.Object.ProduceAndWaitSolution<IChallenge, ISolution>(null!,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()));
    }
}