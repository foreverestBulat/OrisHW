using MyHttpServer.tempOfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Handlers;

public class StaticFileHandlers : Handler
{
    public AppSettingsConfig config;

    public StaticFileHandlers(AppSettingsConfig config)
    {
        this.config = config;
    }

    public override void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        Console.WriteLine(context.Response.StatusCode);

        using var response = context.Response;
        var requestedPath = request.Url.AbsolutePath;
        Console.WriteLine("Static");
        Console.WriteLine("---path" + requestedPath);

        if (requestedPath.Split('/').LastOrDefault().Contains('.') && request.HttpMethod == "GET")
        {
            Console.WriteLine("GET static");
            var segments = request.Url.Segments.TakeLast(2);
            string relativePathStaticFile = String.Join("", segments);
            Console.WriteLine(relativePathStaticFile);

            var pathOfStaticFile = Path.Combine(config.StaticPathFiles, relativePathStaticFile.Trim('/')); // requestedPath.trim
            var pattern = requestedPath?.Split('/')?.LastOrDefault()?.Split('.').LastOrDefault();
            Console.WriteLine(pattern);
            if (File.Exists(pathOfStaticFile) && pattern != null)
            {
                response.ContentType = DictContentType.dictContentType[$".{pattern.ToLower()}"];
                using var fileStream = File.OpenRead(pathOfStaticFile);
                fileStream.CopyTo(response.OutputStream);
            }
            else
            {
                using var fileStream = File.OpenRead(Path.Combine(config.StaticPathFiles, "html/404.html"));
                fileStream.CopyTo(response.OutputStream);
            }
        }
        else if (Successor != null)
        {
            Console.WriteLine(request.Url.LocalPath + "static files handler -> controller handler");
            Successor.HandleRequest(context);
        }
    }
}

