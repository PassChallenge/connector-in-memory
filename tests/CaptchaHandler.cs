using KillDNS.CaptchaSolver.Core.Captcha;
using KillDNS.CaptchaSolver.Core.Handlers;
using KillDNS.CaptchaSolver.Core.Solutions;

namespace KillDNS.CaptchaSolver.Connectors.InMemory.Tests;

public class CaptchaHandler : ICaptchaHandler<PictureCaptcha, TextSolution>
{
    public Task<TextSolution> Handle(PictureCaptcha request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TextSolution("Answer", SolutionResultType.Solved));
    }
}