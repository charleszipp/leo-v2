using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Leo.Core.Owin.OAuth.Google
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseGoogleOAuth2BearerAuthentication(
            this IAppBuilder app,
            string clientId,
            string clientSecret,
            IGoogleOAuth2AuthenticationProvider googleProvider,
            IOAuthBearerAuthenticationProvider bearerProvider,
            ISecureDataFormat<AuthenticationTicket> _accessTokenFormat
            )
        {
            app.SetDefaultSignInAsAuthenticationType(AuthenticationTypes.GoogleBearer);

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                AccessTokenFormat = _accessTokenFormat,
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = AuthenticationTypes.GoogleBearer,
                Provider = bearerProvider
            });

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Provider = googleProvider
            });

            return app;
        }
    }
}