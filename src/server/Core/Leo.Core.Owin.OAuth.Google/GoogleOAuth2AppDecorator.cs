using Leo.Core.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Leo.Core.Owin.OAuth.Google
{
    public class GoogleOAuth2AppDecorator : AppDecorator
    {
        private readonly IUserManager _users;

        public GoogleOAuth2AppDecorator(AppBuilder build, IUserManager users)
            : base(build)
        {
            _users = users;
        }

        public override void Configure(IAppBuilder app)
        {
            _build.Configure(app);

            IDataProtector dataProtecter = app.CreateDataProtector(
                    typeof(OAuthBearerAuthenticationMiddleware).FullName,
                    AuthenticationTypes.GoogleBearer, "v1");
            var accessTokenFormat = new TicketDataFormat(dataProtecter);

            var provider = new GoogleOAuth2BearerAuthenticationProvider(_users, accessTokenFormat);

            app.UseGoogleOAuth2BearerAuthentication(
                "1083822203490-es26p7k77k7qgpreiv8nbuhfjqbjlg4t.apps.googleusercontent.com",
                "xpVEQhsJegOXegYBn82PeaJO",
                provider,
                provider,
                accessTokenFormat
                );
        }
    }
}