using System;
using System.Collections.Generic;
using System.Text;

namespace WatsonWebserver
{
    internal class ApiControllerParameterInfo
    {
        internal string Name;
        internal Type ParameterType;
        internal bool IsValueType;
        internal bool IsHttpContext;

        public object GetDefaultValue()
        {
            return IsValueType ? Activator.CreateInstance(ParameterType) : null;
        }
    }
}
