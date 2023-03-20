using PassChallenge.Connectors.InMemory;
using PassChallenge.Connectors.InMemory.Extensions;
using PassChallenge.Connectors.InMemory.Tests.Tools;
using PassChallenge.Core.Extensions;
using PassChallenge.Core.Solver;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace PassChallenge.Connectors.InMemory.Tests;

public class ResolveTests
{
    [Test]
    public async Task Resolve_Handler_As_Class_Is_Correct()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddChallengeSolver<InMemoryProducer>(builder =>
        {
            builder.SetupInMemoryProducer(producerBuilder =>
            {
                producerBuilder.DescriptorStorageBuilder
                    .AddChallengeHandler<TestChallenge, TestSolution, ChallengeHandler>();
            });
        });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IChallengeSolverFactory factory = serviceProvider.GetRequiredService<IChallengeSolverFactory>();
        IChallengeSolver<TestChallenge, TestSolution> solver = factory.CreateSolver<TestChallenge, TestSolution>();
        TestSolution solution = await solver.Solve(new TestChallenge());

        string expected = "Answer";
        string? actual = solution.Answer;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task Resolve_Handler_As_Func_Is_Correct()
    {
        string expected = "Answer";

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddChallengeSolver<InMemoryProducer>(builder =>
        {
            builder.SetupInMemoryProducer(producerBuilder =>
            {
                producerBuilder.DescriptorStorageBuilder
                    .AddChallengeHandler<TestChallenge, TestSolution>((_, _) =>
                        Task.FromResult(new TestSolution(expected)));
            });
        });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        IChallengeSolverFactory factory = serviceProvider.GetRequiredService<IChallengeSolverFactory>();
        IChallengeSolver<TestChallenge, TestSolution> solver = factory.CreateSolver<TestChallenge, TestSolution>();
        TestSolution solution = await solver.Solve(new TestChallenge());

        string? actual = solution.Answer;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Resolve_Handler_Is_Not_Registered_Throws_InvalidOperationException()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddChallengeSolver<InMemoryProducer>(builder =>
        {
            builder.SetupInMemoryProducer(_ => { });
        });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        IChallengeSolverFactory factory = serviceProvider.GetRequiredService<IChallengeSolverFactory>();
        Assert.Throws<InvalidOperationException>(() => factory.CreateSolver<TestChallenge, TestSolution>());
    }

    [Test]
    public void Resolve_Handler_When_Producer_Is_Not_Setup_Throws_InvalidOperationException()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddChallengeSolver<InMemoryProducer>(_ => { });

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        IChallengeSolverFactory factory = serviceProvider.GetRequiredService<IChallengeSolverFactory>();
        Assert.Throws<InvalidOperationException>(() => factory.CreateSolver<TestChallenge, TestSolution>());
    }
}