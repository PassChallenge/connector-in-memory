using KillDNS.CaptchaSolver.Connectors.InMemory.Tests.Tools;
using KillDNS.CaptchaSolver.Core.Captcha;
using KillDNS.CaptchaSolver.Core.Handlers;
using KillDNS.CaptchaSolver.Core.Solutions;
using Moq;
using NUnit.Framework;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Tests;

public class InMemoryProducerTests
{
    [Test]
    public void InMemoryProducer_Constructor_Is_Correct()
    {
        Mock<IServiceProvider> mock = new();
        InMemoryProducer _ = new(mock.Object);
        Assert.Pass();
    }

    [Test]
    public void InMemoryProducer_Constructor_When_ServiceProvider_Is_Null_Throws_ArgumentNullException()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<ArgumentNullException>(() => new InMemoryProducer(null!));
    }

    [Test]
    public void SetCaptchaHandlerFactory_Is_Correct()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<ICaptchaHandlerFactory> captchaHandlerFactoryMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetCaptchaHandlerFactory(captchaHandlerFactoryMock.Object);

        Assert.Pass();
    }

    [Test]
    public void SetCaptchaHandlerFactory_When_CaptchaHandlerFactory_Is_Null_Throws_ArgumentNullException()
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);

        Assert.Throws<ArgumentNullException>(() => producer.SetCaptchaHandlerFactory(null!));
    }

    [Test]
    public async Task ProduceAndWaitSolution_Is_Correct()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<ICaptchaHandlerFactory> captchaHandlerFactoryMock = new();
        Mock<ICaptchaHandler<ICaptcha, ISolution>> captchaHandlerMock = new();

        captchaHandlerFactoryMock.Setup(x => x.CreateHandler<ICaptcha, ISolution>(serviceProviderMock.Object))
            .Returns(captchaHandlerMock.Object);

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetCaptchaHandlerFactory(captchaHandlerFactoryMock.Object);
        await producer.ProduceAndWaitSolution<ICaptcha, ISolution>(new Mock<ICaptcha>().Object);

        captchaHandlerFactoryMock.Verify(x => x.CreateHandler<ICaptcha, ISolution>(serviceProviderMock.Object),
            Times.Once);
        captchaHandlerMock.Verify(x => x.Handle(It.IsAny<ICaptcha>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void ProduceAndWaitSolution_When_CaptchaHandlerFactory_Is_Null_Throws_InvalidOperationException()
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await producer.ProduceAndWaitSolution<ICaptcha, ISolution>(new Mock<ICaptcha>().Object));
    }

    [Test]
    public void ProduceAndWaitSolution_When_Captcha_Is_Null_Throws_ArgumentNullException()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<ICaptchaHandlerFactory> captchaHandlerFactoryMock = new();

        InMemoryProducer producer = new(serviceProviderMock.Object);
        producer.SetCaptchaHandlerFactory(captchaHandlerFactoryMock.Object);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await producer.ProduceAndWaitSolution<ICaptcha, ISolution>(null!));
    }
}