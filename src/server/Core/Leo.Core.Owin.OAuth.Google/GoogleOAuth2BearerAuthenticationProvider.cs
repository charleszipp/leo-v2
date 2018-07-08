using Flurl;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security.Claims;
using Flurl.Http;
using Leo.Core.Security;

namespace Leo.Core.Owin.OAuth.Google
{
    public class GoogleOAuth2BearerAuthenticationProvider : IGoogleOAuth2AuthenticationProvider, IOAuthBearerAuthenticationProvider
    {
        private readonly IUserManager _users;
        private readonly ISecureDataFormat<AuthenticationTicket> _accessTokenFormat;

        public GoogleOAuth2BearerAuthenticationProvider(IUserManager users, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            _users = users;
            _accessTokenFormat = accessTokenFormat;
        }

        public void ApplyRedirect(GoogleOAuth2ApplyRedirectContext context)
        {
            context.Response.Redirect(context.RedirectUri);
        }

        public async Task Authenticated(GoogleOAuth2AuthenticatedContext context)
        {
            var userId = await _users.CreateOrReplaceUserAsync(context.Identity.AuthenticationType, context.Id, context.Name, context.Email);
            context.Identity.AddClaim(new System.Security.Claims.Claim(Security.ClaimTypes.AccessToken, context.AccessToken));
            context.Identity.AddClaim(new System.Security.Claims.Claim(Security.ClaimTypes.UserId, userId));
        }

        public async Task ReturnEndpoint(GoogleOAuth2ReturnEndpointContext context)
        {
            var result = await context.OwinContext.Authentication.AuthenticateAsync("Google");
            var ticket = new AuthenticationTicket(result.Identity, result.Properties);
            var token = _accessTokenFormat.Protect(ticket);
            context.RedirectUri = context.RedirectUri.SetQueryParam("access_token", token);
        }

        public Task RequestToken(OAuthRequestTokenContext context)
        {
            if (string.IsNullOrEmpty(context.Token))
            {
                var token = context.Request.Query.FirstOrDefault(q => q.Key.Equals("access_token", StringComparison.OrdinalIgnoreCase));
                if (token.Value != null && token.Value.Any())
                {
                    context.Token = token.Value.FirstOrDefault();
                }
            }
            return Task.FromResult<object>(null);
        }

        public async Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (context.Ticket.Identity.Claims.Count() == 0)
            {
                context.Rejected();
            }
            else if (context.Ticket.Identity.Claims.All(c => c.Issuer == ClaimsIdentity.DefaultIssuer))
            {
                context.Rejected();
            }
            else
            {
                var accessToken = context.Ticket.Identity.FindFirst(Security.ClaimTypes.AccessToken);
                if (accessToken != null && !string.IsNullOrEmpty(accessToken.Value))
                {
                    try
                    {
                        var response = await "https://www.googleapis.com/oauth2/v3/tokeninfo"
                        .SetQueryParam("access_token", accessToken.Value)
                        .GetAsync();

                        if (!response.IsSuccessStatusCode)
                            context.Rejected();
                        else
                            context.Validated();
                    }
                    catch (FlurlHttpException ex)
                    {
                        context.Rejected();
                    }
                }
                else
                {
                    context.Rejected();
                }
            }
        }

        public Task ApplyChallenge(OAuthChallengeContext context)
        {
            context.OwinContext.Response.Headers.AppendValues("WWW-Authenticate", context.Challenge);
            return Task.FromResult(0);
        }
    }
}