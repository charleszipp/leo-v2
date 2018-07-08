using System;

namespace Flurl.Http
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequestRouteAttribute : Attribute
    {
        public RequestRouteAttribute(string routeTemplate, string method = "post")
        {
            RouteTemplate = routeTemplate;
            Method = method;
        }

        public string Method { get; private set; }

        public string RouteTemplate { get; private set; }
    }
}