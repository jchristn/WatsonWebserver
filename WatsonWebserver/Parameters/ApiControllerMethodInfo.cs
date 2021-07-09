using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WatsonWebserver
{
    internal class ApiControllerMethodInfo
    {
        private Type ClassType;
        private MethodInfo Method;
        private bool IsStatic;
        private ApiControllerParameterInfo[] Params;
        private HttpMethod HttpMethod;

        /// <summary>
        /// Controller class Context field
        /// </summary>
        internal FieldInfo ContextField;

        internal ApiControllerMethodInfo(Type classType, MethodInfo method, HttpMethod httpMethod)
        {
            this.ClassType = classType;
            this.Method = method;
            this.IsStatic = method.IsStatic;
            this.HttpMethod = httpMethod;

            this.ContextField = classType.GetField("Context");

            var parameters = method.GetParameters();
            this.Params = new ApiControllerParameterInfo[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var methodParam = parameters[i];

                Params[i] = new ApiControllerParameterInfo()
                {
                    Name = methodParam.Name,
                    IsValueType = methodParam.ParameterType.IsValueType,
                    ParameterType = methodParam.ParameterType,
                    IsHttpContext = methodParam.ParameterType == typeof(HttpContext)
                };
            }
        }

        static object[] constructorNoParam = new object[] { };
        internal async Task Invoke(HttpContext ctx)
        {
            object controllerClassObject; // Static or instantiated API controller class

            if (this.IsStatic)
            {
                // Static Controller class type
                controllerClassObject = this.ClassType;
            }
            else
            {
                // Instantiate API controller class
                ConstructorInfo controllerConstructor = this.ClassType.GetConstructor(Type.EmptyTypes);
                controllerClassObject = controllerConstructor.Invoke(constructorNoParam);

                // Initialize Context field (controller.Context = ctx)
                this.ContextField.SetValue(controllerClassObject, ctx);
            }

            //
            // Search for method parameters in QueryString
            //  pass default values if not found
            //
            var invokeParameters = new object[this.Params.Length];

            var queryParameters = ctx.Request.Query.Elements;
            for (int i = 0; i < invokeParameters.Length; i++)
            {
                var methodParam = this.Params[i];
                if (methodParam.IsHttpContext)
                {
                    // Pass HttpContext as parameter
                    invokeParameters[i] = ctx;
                }
                else
                {
                    if (queryParameters.TryGetValue(methodParam.Name, out var p))
                    {
                        // Query parameter found, try to instantiate
                        invokeParameters[i] = JsonConvert.DeserializeObject(p, methodParam.ParameterType);
                    }
                    else
                    {
                        if (HttpMethod == HttpMethod.GET)
                        {
                            // Missing parameter, default values should be passed
                            invokeParameters[i] = methodParam.GetDefaultValue();
                        }
                        else if (HttpMethod == HttpMethod.POST)
                        {
                            // Instantiate parameter by post data
                            invokeParameters[i] = JsonConvert.DeserializeObject(ctx.Request.DataAsString, methodParam.ParameterType); 
                        }
                    }
                }
            }

            // Invoke controller method
            Task task = (Task)this.Method.Invoke(controllerClassObject, invokeParameters);
            await task.ConfigureAwait(false);

            // Get result
            var resultProperty = task.GetType().GetProperty("Result");
            object result = resultProperty.GetValue(task);

            // Serialize to JSON and send 
            await ctx.Response.Send(JsonConvert.SerializeObject(result)).ConfigureAwait(false);
        }
    }
}
