using Demo.Application.Features;

namespace Demo.Api.Middleware;

/// <summary>
/// Pipeline for MediatR that adds the User ClaimsPrincipal to the request
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class UserPipeline<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is BaseRequest br)
        {
            br.CurrentUser = _httpContext?.User;
#if DEBUG
            br.BaseWebUrl = "https://localhost:7060";
#else
            br.BaseWebUrl = "https://productionapigoeshere.com";
#endif
        }

        return next();
    }
}
