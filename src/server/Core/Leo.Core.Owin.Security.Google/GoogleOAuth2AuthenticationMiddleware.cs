using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using Microsoft.Owin.Logging;
using System.Net.Http;

namespace Leo.Core.Owin.Security.Google
{
    public class GoogleOAuth2AuthenticationMiddleware : Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationMiddleware
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;

        public GoogleOAuth2AuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, GoogleOAuth2AuthenticationOptions options) 
            : base(next, app, options)
        {
            _logger = app.CreateLogger<GoogleOAuth2AuthenticationMiddleware>();

            _httpClient = new HttpClient(ResolveHttpMessageHandler(Options));
            _httpClient.Timeout = Options.BackchannelTimeout;
            _httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
        }

        protected override AuthenticationHandler<GoogleOAuth2AuthenticationOptions> CreateHandler()
        {
            return new GoogleOAuth2AuthenticationHandler(_httpClient, _logger);
        }

        private static HttpMessageHandler ResolveHttpMessageHandler(GoogleOAuth2AuthenticationOptions options)
        {
            HttpMessageHandler handler = options.BackchannelHttpHandler ?? new WebRequestHandler();

            // If they provided a validator, apply it or fail.
            if (options.BackchannelCertificateValidator != null)
            {
                // Set the cert validate callback
                var webRequestHandler = handler as WebRequestHandler;
                if (webRequestHandler == null)
                {
                    throw new InvalidOperationException("An ICertificateValidator cannot be specified at the same time as an HttpMessageHandler unless it is a WebRequestHandler.");
                }
                webRequestHandler.ServerCertificateValidationCallback = options.BackchannelCertificateValidator.Validate;
            }

            return handler;
        }
    }
}
