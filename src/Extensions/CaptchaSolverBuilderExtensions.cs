using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KillDNS.CaptchaSolver.Core;
using KillDNS.CaptchaSolver.Core.Extensions;
using KillDNS.CaptchaSolver.Core.Handlers;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Extensions;

public static class CaptchaSolverBuilderExtensions
{
    public static CaptchaSolverBuilder<InMemoryProducer> SetupInMemoryProducer(
        this CaptchaSolverBuilder<InMemoryProducer> captchaSolverBuilder,
        Action<InMemoryProducerBuilder> configure)
    {
        InMemoryProducerBuilder builder = new();
        configure.Invoke(builder);

        MethodInfo addToSupportCaptchaAndSolutionMethodInfo =
            (typeof(CaptchaSolverBuilder<InMemoryProducer>)).GetMethod(nameof(CaptchaSolverBuilder<InMemoryProducer>
                .AddSupportCaptchaAndSolution))!;

        Dictionary<(Type captchaType, Type solutionType), Type> allHandlerTypes = new();

        foreach (Type handlerType in builder.MessageHandlers)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(x => x.GetGenericTypeDefinition() == typeof(ICaptchaHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                var handlerTypes = handlerInterface.GetGenericArguments();
                addToSupportCaptchaAndSolutionMethodInfo.MakeGenericMethod(handlerTypes)
                    .Invoke(captchaSolverBuilder, Array.Empty<object>());

                allHandlerTypes.Add((handlerTypes[0], handlerTypes[1]), handlerType);
            }
        }
        
        captchaSolverBuilder.SetProducerHandlerFactory(new CaptchaHandlerFactory(allHandlerTypes));

        return captchaSolverBuilder;
    }
}