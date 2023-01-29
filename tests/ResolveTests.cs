using KillDNS.CaptchaSolver.Connectors.InMemory.Extensions;
using KillDNS.CaptchaSolver.Core;
using KillDNS.CaptchaSolver.Core.Captcha;
using KillDNS.CaptchaSolver.Core.Extensions;
using KillDNS.CaptchaSolver.Core.Solutions;
using Microsoft.Extensions.DependencyInjection;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Tests;

public class ResolveTests
{
    private readonly IServiceProvider _serviceProvider;
    
    public ResolveTests()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddCaptchaSolver<InMemoryProducer>(builder =>
        {
            builder.SetupInMemoryProducer(producerBuilder =>
            {
                producerBuilder.AddCaptchaHandler<PictureCaptcha, TextSolution, CaptchaHandler>();
            });
        });
        
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Test]
    public async Task ResolveTest()
    {
        var factory = _serviceProvider.GetRequiredService<ICaptchaSolverFactory>();
        var solver = factory.CreateSolver<PictureCaptcha, TextSolution>();
        TextSolution solution = await solver.Solve(new PictureCaptcha(Array.Empty<byte>()));

        string expected = "Answer";
        string? actual = solution.Answer;
        
        Assert.That(actual, Is.EqualTo(expected));
    }
}