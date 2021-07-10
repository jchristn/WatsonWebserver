using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatsonWebserver
{
    /// <summary>
    /// Watson webserver routes.
    /// </summary>
    public class WatsonWebserverRoutes
    {
        #region Public-Members

        /// <summary>
        /// Function to call when a preflight (OPTIONS) request is received.  
        /// Often used to handle CORS.  
        /// Leave null to use the default OPTIONS handler.
        /// </summary>
        public Func<HttpContext, Task> Preflight
        {
            get
            {
                return _Preflight;
            }
            set
            {
                if (value == null) _Preflight = PreflightInternal;
                else _Preflight = value;
            }
        }

        /// <summary>
        /// Function to call prior to routing.  
        /// Return 'true' if the connection should be terminated.
        /// Return 'false' to allow the connection to continue routing.
        /// </summary>
        public Func<HttpContext, Task<bool>> PreRouting = null;

        /// <summary>
        /// Content routes; i.e. routes to specific files or folders for GET and HEAD requests.
        /// </summary>
        public ContentRouteManager Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Content));
                _Content = value;
            }
        }

        /// <summary>
        /// Handler for content route requests.
        /// </summary>
        public ContentRouteHandler ContentHandler
        {
            get
            {
                return _ContentHandler;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(ContentHandler));
                _ContentHandler = value;
            }
        }

        /// <summary>
        /// Static routes; i.e. routes with explicit matching and any HTTP method.
        /// </summary>
        public StaticRouteManager Static
        {
            get
            {
                return _Static;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Static));
                _Static = value;
            }
        }

        /// <summary>
        /// Routes for static controllers or non-static (controller based) controllers
        /// </summary>
        public RouteManager Routes
        {
            get
            {
                return _Route;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Routes));
                _Route = value;
            }
        }

        /// <summary>
        /// Parameter routes; i.e. routes with parameters embedded in the URL, such as /{version}/api/{id}.
        /// </summary>
        public ParameterRouteManager Parameter
        {
            get
            {
                return _Parameter;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Parameter));
                _Parameter = value;
            }
        }

        /// <summary>
        /// Dynamic routes; i.e. routes with regex matching and any HTTP method.
        /// </summary>
        public DynamicRouteManager Dynamic
        {
            get
            {
                return _Dynamic;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Dynamic));
                _Dynamic = value;
            }
        }

        /// <summary>
        /// Default route; used when no other routes match.
        /// </summary>
        public Func<HttpContext, Task> Default
        {
            get
            {
                return _Default;
            }
            set
            {
                _Default = value;
            }
        }

        #endregion

        #region Private-Members

        private WatsonWebserverSettings _Settings = new WatsonWebserverSettings();
        private ContentRouteManager _Content = new ContentRouteManager();
        private ContentRouteHandler _ContentHandler = null;

        private RouteManager _Route = new RouteManager();
        private StaticRouteManager _Static = new StaticRouteManager();
        private ParameterRouteManager _Parameter = new ParameterRouteManager();
        private DynamicRouteManager _Dynamic = new DynamicRouteManager();
        private Func<HttpContext, Task> _Default = null;
        private Func<HttpContext, Task> _Preflight = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object using default settings.
        /// </summary>
        public WatsonWebserverRoutes()
        {
            _Preflight = PreflightInternal;
            _ContentHandler = new ContentRouteHandler(_Content);
        }

        /// <summary>
        /// Instantiate the object using default settings and the specified default route.
        /// </summary>
        public WatsonWebserverRoutes(WatsonWebserverSettings settings, Func<HttpContext, Task> defaultRoute)
        {
            if (settings == null) settings = new WatsonWebserverSettings();
            if (defaultRoute == null) throw new ArgumentNullException(nameof(defaultRoute));

            _Settings = settings;
            _Preflight = PreflightInternal;
            _Default = defaultRoute;
            _ContentHandler = new ContentRouteHandler(_Content);
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        private async Task PreflightInternal(HttpContext ctx)
        { 
            ctx.Response.StatusCode = 200;

            string[] requestedHeaders = null;
            if (ctx.Request.Headers != null)
            {
                foreach (KeyValuePair<string, string> curr in ctx.Request.Headers)
                {
                    if (String.IsNullOrEmpty(curr.Key)) continue;
                    if (String.IsNullOrEmpty(curr.Value)) continue;
                    if (String.Compare(curr.Key.ToLower(), "access-control-request-headers") == 0)
                    {
                        requestedHeaders = curr.Value.Split(',');
                        break;
                    }
                }
            }

            string headers = "";

            if (requestedHeaders != null)
            {
                int addedCount = 0;
                foreach (string curr in requestedHeaders)
                {
                    if (String.IsNullOrEmpty(curr)) continue;
                    if (addedCount > 0) headers += ", ";
                    headers += ", " + curr;
                    addedCount++;
                }
            }

            foreach (KeyValuePair<string, string> header in _Settings.Headers)
            {
                ctx.Response.Headers.Add(header.Key, header.Value);
            }

            ctx.Response.ContentLength = 0;
            await ctx.Response.Send().ConfigureAwait(false);
        }

        #endregion
    }
}
