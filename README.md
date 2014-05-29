# Chq.XmlRpc.Mvc

Lightweight model binding for XML-RPC requests in ASP.NET MVC

# Usage

1. Configure the route binding

```c#
routes.MapXmlRpcRoute("xml-rpc", "api/xml-rpc");
```

2. Add the model binder provider

```c#
ModelBinderProviders.BinderProviders.Add(new XmlRpcModelBinderProvider());
```

3. You're done, start adding controllers. For a start here's one that supports the newPost method of the MetaWeblog API

```c#
public class MetaWeblogController : Controller
{
    public XmlRpcResult NewPost(string blogid, string username, string password, Post post, bool publish)
    {
            ...omitted, logic to create a new post...
            return new XmlRpcResult(id.ToString());
    }
}
```

You can read more here: [Implementing XML-RPC services in ASP.NET MVC](http://http://www.michael-mckenna.com/Blog/implementing-xml-rpc-services-in-asp-net-mvc/)

Chq.XmlRpc.Mvc is Copyright © 2014 [Michael McKenna](http://www.michael-mckenna.com/) and other contributors under the [MIT license](http://opensource.org/licenses/mit-license.html).
