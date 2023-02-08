using KillDNS.CaptchaSolver.Connectors.InMemory.Extensions;
using KillDNS.CaptchaSolver.Connectors.InMemory.Tests.Tools;
using KillDNS.CaptchaSolver.Core.Captcha;
using KillDNS.CaptchaSolver.Core.Extensions;
using KillDNS.CaptchaSolver.Core.Solutions;
using KillDNS.CaptchaSolver.Core.Solver;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Tests;

public class ResolveTests
{
    [Test]
    public async Task Resolve_Handler_As_Class_Is_Correct()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddCaptchaSolver<InMemoryProducer>(builder =>
        {
            builder.SetupInMemoryProducer(producerBuilder =>
            {
                producerBuilder.AddCaptchaHandler<PictureCaptcha, TextSolution, CaptchaHandler>();
            });
        });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        ICaptchaSolverFactory factory = serviceProvider.GetRequiredService<ICaptchaSolverFactory>();
        ICaptchaSolver<PictureCaptcha, TextSolution> solver = factory.CreateSolver<PictureCaptcha, TextSolution>();
        TextSolution solution = await solver.Solve(new PictureCaptcha(Array.Empty<byte>()));

        string expected = "Answer";
        string? actual = solution.Answer;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task Resolve_Handler_As_Func_Is_Correct()
    {
        string expected = "Answer";

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddCaptchaSolver<InMemoryProducer>(builder =>
        {
            builder.SetupInMemoryProducer(producerBuilder =>
            {
                producerBuilder.AddCaptchaHandler<PictureCaptcha, TextSolution>((_, _) =>
                    Task.FromResult(new TextSolution(expected)));
            });
        });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        ICaptchaSolverFactory factory = serviceProvider.GetRequiredService<ICaptchaSolverFactory>();
        ICaptchaSolver<PictureCaptcha, TextSolution> solver = factory.CreateSolver<PictureCaptcha, TextSolution>();
        TextSolution solution = await solver.Solve(new PictureCaptcha(Array.Empty<byte>()));

        string? actual = solution.Answer;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Resolve_Handler_Is_Not_Registered_Throws_InvalidOperationException()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddCaptchaSolver<InMemoryProducer>(builder => { builder.SetupInMemoryProducer(_ => { }); });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        ICaptchaSolverFactory factory = serviceProvider.GetRequiredService<ICaptchaSolverFactory>();
        Assert.Throws<InvalidOperationException>(() => factory.CreateSolver<PictureCaptcha, TextSolution>());
    }

    [Test]
    public void Resolve_Handler_When_Producer_Is_Not_Setup_Throws_InvalidOperationException()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddCaptchaSolver<InMemoryProducer>(_ => { });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        ICaptchaSolverFactory factory = serviceProvider.GetRequiredService<ICaptchaSolverFactory>();
        Assert.Throws<InvalidOperationException>(() => factory.CreateSolver<PictureCaptcha, TextSolution>());
    }
}