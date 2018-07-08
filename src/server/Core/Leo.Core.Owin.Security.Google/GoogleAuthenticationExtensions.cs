using Microsoft.Owin.Security.Google;
using Owin;
using System;

namespace Leo.Core.Owin.Security.Google
{
    public static class GoogleAuthenticationExtensions
    {
        public static IAppBuilder UseLeosGoogleAuthentication(this IAppBuilder app, GoogleOAuth2AuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(GoogleOAuth2AuthenticationMiddleware), app, options);
            return app;
        }
    }
}