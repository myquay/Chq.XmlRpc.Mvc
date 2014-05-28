using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Chq.XmlRpc.Mvc
{
    public class XmlRpcModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null) return null;

            var contentType = httpContext.Request.ContentType;
            if (string.Compare(contentType, @"text/xml", StringComparison.OrdinalIgnoreCase) != 0) return null;

            if (httpContext.Request.InputStream == null || httpContext.Request.InputStream.Length == 0) return null;


            XDocument xml = XDocument.Load(httpContext.Request.InputStream);
            if (xml.Document.Element("methodCall") == null) return null;

            //We don't want the stream to be left in a "read" state for the model binding later on
            httpContext.Request.InputStream.Seek(0, SeekOrigin.Begin);

            return new XmlRpcModelBinder();
        }
    }
}
   
