using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KillDNS.CaptchaSolver.Core.Captcha;
using KillDNS.CaptchaSolver.Core.Handlers;
using KillDNS.CaptchaSolver.Core.Solutions;

namespace KillDNS.CaptchaSolver.Connectors.InMemory;

public class InMemoryProducerBuilder : IBuilderWithCaptchaHandlers
{
    internal readonly Dictionary<(Type captchaType, Type solutionType), CaptchaHandlerDescriptor> Descriptors = new();

    public IBuilderWithCaptchaHandlers AddCaptchaHandler<TCaptcha, TSolution, THandler>()
        where TCaptcha : ICaptcha
        where TSolution : ISolution
        where THandler : ICaptchaHandler<TCaptcha, TSolution>
    {
        Descriptors.Add((typeof(TCaptcha), typeof(TSolution)),
            CaptchaHandlerDescriptor.Create<TCaptcha, TSolution, THandler>());
        return this;
    }

    public IBuilderWithCaptchaHandlers AddCaptchaHandler<TCaptcha, TSolution, THandler>(
        Func<IServiceProvider, THandler> factory)
        where TCaptcha : ICaptcha
        where TSolution : ISolution
        where THandler : ICaptchaHandler<TCaptcha, TSolution>
    {
        Descriptors.Add((typeof(TCaptcha), typeof(TSolution)),
            CaptchaHandlerDescriptor.Create<TCaptcha, TSolution, THandler>(factory));
        return this;
    }

    public IBuilderWithCaptchaHandlers AddCaptchaHandler<TCaptcha, TSolution>(
        Func<IServiceProvider, TCaptcha, Task<TSolution>> func)
        where TCaptcha : ICaptcha where TSolution : ISolution
    {
        Descriptors.Add((typeof(TCaptcha), typeof(TSolution)), CaptchaHandlerDescriptor.Create(func));
        return this;
    }
}