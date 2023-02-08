using KillDNS.CaptchaSolver.Core.Solutions;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Tests.Tools;

public class TestSolution : ISolution
{
    public TestSolution(SolutionResultType resultType)
    {
        ResultType = resultType;
    }

    public SolutionResultType ResultType { get; }
}