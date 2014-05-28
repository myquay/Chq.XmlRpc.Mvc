using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Chq.XmlRpc.Mvc
{
    public class XmlRpcResult : ActionResult
    {
        private XDocument _responseObject;

        public XmlRpcResult(object data)
        {
            _responseObject = new XDocument(new XElement("methodResponse"));

            if (data is Exception)
            {
                //Encode as a fault
                _responseObject.Element("methodResponse").Add(
                    new XElement("fault", 
                        new XElement("value", 
                            new XElement("string", 
                                (data as Exception).Message))));
            }
            else
            {
                //Encode as params
                _responseObject.Element("methodResponse").Add(
                    new XElement("params",
                        new XElement("param",
                            XmlRpcData.SerialiseValue(data))));
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (_responseObject != null)
            {
                var response = _responseObject.ToString();
                context.HttpContext.Response.ContentType = "text/xml";

                context.HttpContext.Response.Headers["content-length"] = ASCIIEncoding.UTF8.GetBytes(response).Length.ToString();
                context.HttpContext.Response.Output.Write(response);
            }
        }
    }
}
