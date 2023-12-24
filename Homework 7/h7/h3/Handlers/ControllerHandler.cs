using MyHttpServer.Other;
using MyHttpServer.route;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Handlers;

public class ControllerHandler : Handler
{
    public override void HandleRequest(HttpListenerContext context)
    {
        Console.WriteLine();
        Console.WriteLine(context.User + "----------------User-------------------");

        var request = context.Request;
        using var response = context.Response;

        try
        {
            Console.WriteLine("Try");

            var requestedPath = request.Url.AbsolutePath;
            var parts = context.Request.Url.PathAndQuery.Split("/")
                .Where(part => !string.IsNullOrEmpty(part)).ToArray();

            var controllerName = parts.Length > 0 ? parts[0] : "";
            var actionName = parts.Length > 1 ? parts[1] : "";
            var objectId = parts.Length > 2 ? parts[2] : "";

            var routes = Route.GetRoutes();
            var route = routes.FirstOrDefault(r => r.ControllerName == controllerName && r.ActionName == actionName);

            if (route != null)
            {
                object resultMethod;
                if (request.HttpMethod == "POST")
                {
                    Console.WriteLine("POST not static");
                    object[] parametersArray = new object[]
                    {
                        new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd()
                    };
                    resultMethod = route.MethodInfo.Invoke(route.Instance, parametersArray);
                }
                else
                {
                    object[] parametersArray 
                        = route.MethodInfo.GetParameters().Length > 0 ? new object[] { objectId } : null;
                    resultMethod = route.MethodInfo.Invoke(route.Instance, parametersArray);
                }
                SendAnswer(resultMethod, context);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public static void SendAnswer(object obj, HttpListenerContext context)
    {
        switch (obj)
        {
            case Redirect redirect:
                {
                    if (redirect.StatusCode == 401)
                    {
                        Console.WriteLine("--------401--------");
                        context.Response.StatusCode = 401;
                    }
                    else if (redirect.StatusCode == 301)
                    {
                        Console.WriteLine("--------Redirect--------");

                        context.Response.StatusCode = 301;
                        context.Response.Redirect(redirect.Path);  //($"http://127.0.0.1:2323{redirect.Path}");
                    }
                    break;
                }
            case Page page:
                {
                    Console.WriteLine("--------Page--------");
                    Console.WriteLine(page.LocalPath);
                    string html = page.Render();
                    context.Response.ContentType = "text/html";
                    var buffer = Encoding.UTF8.GetBytes(html);
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    break;
                }
            default: break;
        }
    }
}