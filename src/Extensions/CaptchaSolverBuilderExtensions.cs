using System;
using KillDNS.CaptchaSolver.Core.Extensions;
using KillDNS.CaptchaSolver.Core.Handlers;
using KillDNS.CaptchaSolver.Core.Solver;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Extensions;

public static class CaptchaSolverBuilderExtensions
{
    public static CaptchaSolverSpecifiedBuilder<InMemoryProducer> SetupInMemoryProducer(
        this CaptchaSolverSpecifiedBuilder<InMemoryProducer> captchaSolverBuilder,
        Action<InMemoryProducerBuilder> configure)
    {
        InMemoryProducerBuilder builder = new();
        configure.Invoke(builder);

        foreach (CaptchaHandlerDescriptor descriptor in builder.Descriptors.Values)
        {
            captchaSolverBuilder.AddSupportCaptchaAndSolution(descriptor);
        }
            
        captchaSolverBuilder.SetCaptchaHandlerFactory(new CaptchaHandlerFactory(builder.Descriptors.Values));
        return captchaSolverBuilder;
    }
}