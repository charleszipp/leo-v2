using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;

namespace Leo.Core.Security.Owin
{
    public class GoogleAuthenticationOptions : AuthenticationOptions
    {
        internal static class Defaults
        {
            internal const string Scope = "profile email";
            internal const string CallbackPath = "/signin-google";
            internal const string AccessType = null;
            internal const string AuthenticationType = "leogoogle";
        }

        public GoogleAuthenticationOptions(
            string clientId,
            string clientSecret,
            string authenticationType = Defaults.AuthenticationType, 
            string callbackPath = Defaults.CallbackPath, 
            string scope = Defaults.Scope,
            string accessType = Defaults.AccessType,
            ISecureDataFormat<AuthenticationProperties> _stateDataFormat = null) 
            : base(authenticationType)
        {
            CallbackPath = new PathString(callbackPath);
            Scope = scope;
            ClientId = clientId;
        }

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }

        public PathString CallbackPath { get; private set; }

        public string Scope { get; private set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}
