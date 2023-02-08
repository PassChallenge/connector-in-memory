using System;
using System.Threading;
using System.Threading.Tasks;
using KillDNS.CaptchaSolver.Core.Handlers;
using KillDNS.CaptchaSolver.Core.Producer;

namespace KillDNS.CaptchaSolver.Connectors.InMemory;

public class InMemoryProducer : ProducerWithSpecifiedCaptchaAndSolutions, IProducerWithCaptchaHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private ICaptchaHandlerFactory? _captchaHandlerFactory;

    public InMemoryProducer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public override Task<TSolution> ProduceAndWaitSolution<TCaptcha, TSolution>(TCaptcha captcha,
        CancellationToken cancellationToken = default)
    {
        if (_captchaHandlerFactory == null)
            throw new InvalidOperationException("Captcha handler factory is not set.");
        
        if (captcha == null) 
            throw new ArgumentNullException(nameof(captcha));

        var messageHandler = _captchaHandlerFactory.CreateHandler<TCaptcha, TSolution>(_serviceProvider);
        return messageHandler.Handle(captcha, cancellationToken);
    }

    public void SetCaptchaHandlerFactory(ICaptchaHandlerFactory captchaHandlerFactory)
    {
        _captchaHandlerFactory =
            captchaHandlerFactory ?? throw new ArgumentNullException(nameof(captchaHandlerFactory));
    }
}