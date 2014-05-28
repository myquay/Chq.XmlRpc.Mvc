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
    public class XmlRpcModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //Get access to the list of parameters in the calling action.
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType()).FindAction(controllerContext, controllerContext.RouteData.GetRequiredString("action"));
            var parameters = controllerDescriptor.GetParameters().ToList();
            
            //XML-RPC relies on the ordering of the elements, get the position of the current parameter that we're binding
            var parameterOfInterest = parameters.Single(p => bindingContext.ModelName == p.ParameterName);
            var paramNumber = parameters.IndexOf(parameterOfInterest);

            XDocument xDoc = XDocument.Load(controllerContext.HttpContext.Request.InputStream);
            var inputParameters = xDoc.Descendants("params").Elements().ToArray();

            if (inputParameters.Length <= paramNumber) return null; //No data for this parameter, return null

            var model = XmlRpcData.DeserialiseValue(inputParameters[paramNumber].Elements("value").Single(), bindingContext.ModelType);
            controllerContext.HttpContext.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin); //Reset the stream for the next pass
            return model;
        }
    }
}
