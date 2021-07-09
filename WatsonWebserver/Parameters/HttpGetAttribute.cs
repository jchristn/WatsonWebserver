using System;
using System.Collections.Generic;
using System.Text;

namespace WatsonWebserver
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpGetAttribute : Attribute
    {
        public string MethodName;

        public HttpGetAttribute()
        {
        }

        public HttpGetAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
