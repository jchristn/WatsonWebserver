using System;
using System.Collections.Generic;
using System.Text;

namespace WatsonWebserver
{
    /// <summary>
    /// Annotates a controller with a route prefix that applies to all actions within the controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RoutePrefixAttribute : Attribute
    {
        public string RoutePrefix;

        public RoutePrefixAttribute(string prefix)
        {
            this.RoutePrefix = prefix;
        }
    }
}
