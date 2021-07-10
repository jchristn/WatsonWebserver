using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WatsonWebserver
{
    public class RouteManager
    {
        Dictionary<string, ApiControllerMethodInfo> routes = new Dictionary<string, ApiControllerMethodInfo>();

        internal void AddRoute(string routePrefix, string methodName, Type classType, MethodInfo classMethod, HttpMethod httpMethod)
        {
            routes.Add($"/{routePrefix.Trim('/')}/{methodName.Trim('/')}", 
                new ApiControllerMethodInfo(classType, classMethod, httpMethod));
        }

        internal ApiControllerMethodInfo Match(HttpRequest req)
        {
            routes.TryGetValue(req.Url.RawWithoutQuery, out var invokeInfo);
            return invokeInfo;
        }
    }
}
