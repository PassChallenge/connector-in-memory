using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PassChallenge.Core.Challenges;
using PassChallenge.Core.Handlers;
using PassChallenge.Core.Producer;
using PassChallenge.Core.Solutions;
using PassChallenge.Core.Solver;

namespace PassChallenge.Connectors.InMemory;

public class InMemoryProducer : IProducerWithChallengeHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private IChallengeHandlerFactory? _challengeHandlerFactory;

    public InMemoryProducer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }


    public void SetAvailableChallengeAndSolutionStorage(
        IAvailableChallengeAndSolutionStorage availableChallengeAndSolutionStorage)
    {
    }

    public bool CanProduce<TChallenge, TSolution>(string? handlerName = default)
        where TChallenge : IChallenge where TSolution : ISolution
    {
        if (_challengeHandlerFactory == null)
            throw new InvalidOperationException("Challenge handler factory is not set.");

        return _challengeHandlerFactory.CanProduce<TChallenge, TSolution>(handlerName);
    }

    public string GetDefaultHandlerName<TChallenge, TSolution>() where TChallenge : IChallenge where TSolution : ISolution
    {
        if (_challengeHandlerFactory == null)
            throw new InvalidOperationException("Challenge handler factory is not set.");

        return _challengeHandlerFactory.GetDefaultHandlerName<TChallenge, TSolution>();
    }

    public IReadOnlyCollection<string> GetHandlerNames<TChallenge, TSolution>()
        where TChallenge : IChallenge where TSolution : ISolution
    {
        if (_challengeHandlerFactory == null)
            throw new InvalidOperationException("Challenge handler factory is not set.");

        return _challengeHandlerFactory.GetHandlerNames<TChallenge, TSolution>();
    }

    public Task<TSolution> ProduceAndWaitSolution<TChallenge, TSolution>(TChallenge challenge, string? handlerName = default,
        CancellationToken cancellationToken = default) where TChallenge : IChallenge where TSolution : ISolution
    {
        if (_challengeHandlerFactory == null)
            throw new InvalidOperationException("Challenge handler factory is not set.");

        if (challenge == null)
            throw new ArgumentNullException(nameof(challenge));
        
        IChallengeHandler<TChallenge, TSolution> messageHandler =
            _challengeHandlerFactory.CreateHandler<TChallenge, TSolution>(_serviceProvider, handlerName);
        return messageHandler.Handle(challenge, cancellationToken);
    }

    public void SetChallengeHandlerFactory(IChallengeHandlerFactory challengeHandlerFactory)
    {
        _challengeHandlerFactory =
            challengeHandlerFactory ?? throw new ArgumentNullException(nameof(challengeHandlerFactory));
    }
}