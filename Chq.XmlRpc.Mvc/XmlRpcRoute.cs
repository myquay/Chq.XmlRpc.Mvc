using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace System.Web.Mvc
{
    public class XmlRpcRoute : Route
    {
        public XmlRpcRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }
        public XmlRpcRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler) { }
        public XmlRpcRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler) { }
        public XmlRpcRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData routeData = base.GetRouteData(httpContext);

            if (routeData == null) return null;
            if (httpContext.Request.InputStream == null || httpContext.Request.InputStream.Length == 0) return null;

            var xml = XDocument.Load(httpContext.Request.InputStream);

            var rootElement = xml.Document.Element("methodCall");
            if (rootElement == null) throw new HttpException(400, @"The ""methodCall"" element is missing from the XML-RPC request body.");

            var methodNameElement = rootElement.Element("methodName");
            if (methodNameElement == null) throw new HttpException(400, @"The ""methodName"" element is missing from the XML-RPC request body.");
               
            if(!Regex.IsMatch(methodNameElement.Value, "^[A-Za-z0-9_:/]+.[A-Za-z0-9_:/]+$"))
                throw new HttpException(400, @"The ""methodName"" element is in the incorrect format.");

            var methodNameParts = methodNameElement.Value.Split('.');
                routeData.Values["controller"] = methodNameParts[0];
                routeData.Values["action"] = methodNameParts[1];

            //We don't want the stream to be left in a "read" state for the model binding later on
            httpContext.Request.InputStream.Seek(0, IO.SeekOrigin.Begin);

            return routeData;
        }
    }

    //Make it easier to define the routes
    public static class XmlRpcRouteCollectionExtensions
    {
        public static XmlRpcRoute MapXmlRpcRoute(this RouteCollection routes, string name, string url)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (url == null) throw new ArgumentNullException("url");

            XmlRpcRoute route = new XmlRpcRoute(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(),
                DataTokens = new RouteValueDictionary()
            };

            routes.Add(name, route);

            return route;
        }
    }
}
