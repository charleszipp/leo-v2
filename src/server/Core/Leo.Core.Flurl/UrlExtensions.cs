using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Flurl
{
    public static class UrlExtensions
    {
        public static Url WithParams(this Url url, string route, object request, bool hasBody = false)
        {
            var routeUrl = route.WithParams(request, hasBody);
            var rvalue = url.AppendPathSegment(routeUrl.Path);
            rvalue.Query = routeUrl.Query;

            return rvalue;
        }

        public static Url WithParams(this string url, string route, object request, bool hasBody = false)
        {
            var routeUrl = route.WithParams(request, hasBody);
            var rvalue = url.AppendPathSegment(routeUrl.Path);
            rvalue.Query = routeUrl.Query;

            return rvalue;
        }

        public static Url WithParams(this string url, object request, bool hasBody = false)
        {
            return WithParams(new Url(url), request, hasBody);
        }

        public static Url WithParams(this Url url, object request, bool hasBody = false)
        {
            string rvalue = url.ToString();

            IDictionary<string, object> queryParams = new Dictionary<string, object>();

            Type requestType = request.GetType();
            var requestProperties = requestType.GetProperties().ToList();
            var requestPropertiesMatched = new List<PropertyInfo>();

            var matches = Regex.Matches(rvalue, @"(?<=\{)(.*?)(?=\})");
            if (matches != null && matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    bool requestPropertyMatched = false;

                    foreach (var requestProperty in requestProperties)
                    {
                        if (requestProperty.Name.Equals(matches[i].Value, StringComparison.OrdinalIgnoreCase))
                        {
                            var value = requestProperty.GetValue(request);
                            if (value != null)
                            {
                                rvalue = rvalue.Replace("{" + matches[i].Value + "}", value.ToString());
                                requestPropertiesMatched.Add(requestProperty);
                                requestPropertyMatched = true;
                                break;
                            }
                            else
                            {
                                throw new ArgumentException("Request property matching template parameter is null.", requestProperty.Name);
                            }
                        }
                    }

                    if (!requestPropertyMatched)
                    {
                        throw new ArgumentException("No matching request property found for template parameter.", matches[i].Value);
                    }
                }
            }

            if (!hasBody)
            {
                var requestPropertiesNotMatched = requestProperties.Except(requestPropertiesMatched);

                foreach (var prop in requestPropertiesNotMatched)
                {
                    var value = prop.GetValue(request);
                    if (value != null)
                    {
                        rvalue = rvalue.SetQueryParam(prop.Name.ToLower(), value);
                    }
                }
            }

            return new Url(rvalue);
        }
    }
}