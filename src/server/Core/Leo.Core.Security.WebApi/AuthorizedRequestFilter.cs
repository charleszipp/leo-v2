using Leo.Core.Api.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Leo.Core.Security.WebApi
{
    public class AuthorizedRequestFilter : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.ActionArguments.Any())
            {
                var request = actionContext.ActionArguments.FirstOrDefault().Value;

                if (request != null && request is IAuthorizedRequest && actionContext.RequestContext.Principal != null)
                {
                    var authorizedRequest = request as IAuthorizedRequest;

                    ClaimsIdentity identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        var userIdClaim = identity.FindFirst(ClaimTypes.UserId);
                        if (userIdClaim != null)
                            authorizedRequest.UserId = userIdClaim.Value;

                        authorizedRequest.Token = actionContext.Request.Headers.Authorization?.Parameter;
                    }
                }
            }

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}