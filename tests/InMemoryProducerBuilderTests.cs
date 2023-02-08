using KillDNS.CaptchaSolver.Core.Captcha;
using KillDNS.CaptchaSolver.Core.Handlers;
using KillDNS.CaptchaSolver.Core.Solutions;
using Moq;
using NUnit.Framework;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Tests;

public class InMemoryProducerBuilderTests
{
    [Test]
    public async Task AddCaptchaHandler_With_HandlerFunc_Is_Correct()
    {
        InMemoryProducerBuilder builder = new();
        Mock<Func<Task<ISolution>>> mock = new();

        builder.AddCaptchaHandler<ICaptcha, ISolution>((_, _) => mock.Object.Invoke());
        
        Assert.That(builder.Descriptors.Count, Is.EqualTo(1));

        var descriptorPair = builder.Descriptors.First();
        
        Assert.That(descriptorPair.Key, Is.EqualTo((typeof(ICaptcha), typeof(ISolution))));
        
        Assert.Multiple(() =>
        {
            Assert.That(descriptorPair.Value.CaptchaType, Is.EqualTo(typeof(ICaptcha)));
            Assert.That(descriptorPair.Value.SolutionType, Is.EqualTo(typeof(ISolution)));
            Assert.Null(descriptorPair.Value.HandlerType);
            Assert.Null(descriptorPair.Value.ImplementationFactory);
            Assert.NotNull(descriptorPair.Value.SolverFunction);
        });
        
        Mock<IServiceProvider> serviceProviderMock = new();
        await (Task<ISolution>)descriptorPair.Value.SolverFunction!.Invoke(serviceProviderMock.Object, It.IsAny<ICaptcha>());
        
        mock.Verify(x => x.Invoke(), Times.Once);
    }

    [Test]
    public async Task AddCaptchaHandler_With_HandlerFactory_Is_Correct()
    {
        InMemoryProducerBuilder builder = new();
        Mock<Func<ICaptchaHandler<ICaptcha, ISolution>>> getHandlerMock = new();
        Mock<ICaptchaHandler<ICaptcha, ISolution>> handlerMock = new();
        handlerMock.Setup(x => x.Handle(It.IsAny<ICaptcha>(), It.IsAny<CancellationToken>()));

        getHandlerMock.Setup(x => x.Invoke()).Returns(handlerMock.Object);

        builder.AddCaptchaHandler<ICaptcha, ISolution, ICaptchaHandler<ICaptcha, ISolution>>(_ =>
            getHandlerMock.Object.Invoke());

        Assert.That(builder.Descriptors.Count, Is.EqualTo(1));

        var descriptorPair = builder.Descriptors.First();

        Assert.That(descriptorPair.Key, Is.EqualTo((typeof(ICaptcha), typeof(ISolution))));

        Assert.Multiple(() =>
        {
            Assert.That(descriptorPair.Value.CaptchaType, Is.EqualTo(typeof(ICaptcha)));
            Assert.That(descriptorPair.Value.SolutionType, Is.EqualTo(typeof(ISolution)));
            Assert.That(descriptorPair.Value.HandlerType, Is.EqualTo(typeof(ICaptchaHandler<ICaptcha, ISolution>)));
            Assert.Null(descriptorPair.Value.SolverFunction);
            Assert.NotNull(descriptorPair.Value.ImplementationFactory);
        });

        Mock<IServiceProvider> serviceProviderMock = new();

        ICaptchaHandler<ICaptcha, ISolution> handler =
            (ICaptchaHandler<ICaptcha, ISolution>)descriptorPair.Value.ImplementationFactory!.Invoke(serviceProviderMock
                .Object);

        getHandlerMock.Verify(x => x.Invoke(), Times.Once);

        await handler.Handle(It.IsAny<ICaptcha>(), It.IsAny<CancellationToken>());

        handlerMock.Verify(x => x.Handle(It.IsAny<ICaptcha>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void AddCaptchaHandler_With_Handler_Is_Correct()
    {
        InMemoryProducerBuilder builder = new();
        Mock<ICaptchaHandler<ICaptcha, ISolution>> handlerMock = new();
        handlerMock.Setup(x => x.Handle(It.IsAny<ICaptcha>(), It.IsAny<CancellationToken>()));

        builder.AddCaptchaHandler<ICaptcha, ISolution, ICaptchaHandler<ICaptcha, ISolution>>();

        Assert.That(builder.Descriptors.Count, Is.EqualTo(1));

        var descriptorPair = builder.Descriptors.First();

        Assert.That(descriptorPair.Key, Is.EqualTo((typeof(ICaptcha), typeof(ISolution))));

        Assert.Multiple(() =>
        {
            Assert.That(descriptorPair.Value.CaptchaType, Is.EqualTo(typeof(ICaptcha)));
            Assert.That(descriptorPair.Value.SolutionType, Is.EqualTo(typeof(ISolution)));
            Assert.That(descriptorPair.Value.HandlerType, Is.EqualTo(typeof(ICaptchaHandler<ICaptcha, ISolution>)));
            Assert.Null(descriptorPair.Value.SolverFunction);
            Assert.Null(descriptorPair.Value.ImplementationFactory);
        });
    }
}