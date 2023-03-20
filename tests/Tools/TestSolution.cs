using PassChallenge.Core.Solutions;

namespace PassChallenge.Connectors.InMemory.Tests.Tools;

public class TestSolution : ISolution
{
    public TestSolution(string? answer, SolutionResultType resultType = SolutionResultType.Solved)
    {
        ResultType = resultType;
        Answer = answer;
    }

    public string? Answer { get; }

    public SolutionResultType ResultType { get; }

    public string? ErrorMessage { get; }
}