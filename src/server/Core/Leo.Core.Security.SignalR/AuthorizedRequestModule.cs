using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Leo.Core.Security;
using System.Security.Claims;
using Leo.Core.Api.Interfaces;

namespace Leo.Core.Security.SignalR
{
    public class AuthorizedRequestModule : HubPipelineModule
    {
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            var request = context.Args.FirstOrDefault();

            if(request != null && request is IAuthorizedRequest && context.Hub.Context.User != null)
            {
                var authorizedRequest = request as IAuthorizedRequest;

                ClaimsIdentity identity = context.Hub.Context.User.Identity as ClaimsIdentity;
                if(identity != null)
                {
                    var userIdClaim = identity.FindFirst(ClaimTypes.UserId);
                    if (userIdClaim != null)
                        authorizedRequest.UserId = userIdClaim.Value;
                }
            }

            return base.OnBeforeIncoming(context);
        }
    }
}
