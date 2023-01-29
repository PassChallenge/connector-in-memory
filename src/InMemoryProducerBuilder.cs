using System;
using System.Collections.Generic;
using KillDNS.CaptchaSolver.Core.Captcha;
using KillDNS.CaptchaSolver.Core.Handlers;
using KillDNS.CaptchaSolver.Core.Solutions;

namespace KillDNS.CaptchaSolver.Connectors.InMemory;

public class InMemoryProducerBuilder : IBuilderWithCaptchaHandlers
{
    internal HashSet<Type> MessageHandlers { get; } = new();
    
    public IBuilderWithCaptchaHandlers AddCaptchaHandler<TCaptcha, TSolution, THandler>()
        where TCaptcha : ICaptcha
        where TSolution : ISolution
        where THandler : ICaptchaHandler<TCaptcha, TSolution>
    {
        MessageHandlers.Add(typeof(THandler));
        return this;
    }
}