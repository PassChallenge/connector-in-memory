using PassChallenge.Core.Handlers;

namespace PassChallenge.Connectors.InMemory.Tests.Tools;

public class ChallengeHandler : IChallengeHandler<TestChallenge, TestSolution>
{
    public Task<TestSolution> Handle(TestChallenge challenge, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TestSolution("Answer"));
    }
}