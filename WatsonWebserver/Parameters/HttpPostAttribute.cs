using System;
using System.Collections.Generic;
using System.Text;

namespace WatsonWebserver
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpPostAttribute : Attribute
    {
        public string MethodName;

        public HttpPostAttribute()
        {
        }

        public HttpPostAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
