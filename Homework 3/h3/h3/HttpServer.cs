using System.IO;
using System.Net;
using System.Text;

namespace MyHttpServer;

public class HttpServer
{
    private HttpListener server;

    internal HttpListenerContext context;
    internal HttpListenerResponse response;
    internal HttpListenerRequest request;

    internal Task startServer;
    internal Task waitFinish;

    public HttpServer()
    {
        server = new HttpListener();
    }

    public void Start()
    {
        startServer = new Task(() => Run());
        waitFinish = new Task(() => Wait());
        startServer.Start();
        waitFinish.Start();

        Task.WaitAll(new Task[] { startServer, waitFinish });
    }

    private void Wait()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (input == "stop")
                break;
        }
    }

    private async void Run()
    {
        var config = GetConfig("appsettings.json");
        server.Prefixes.Add($"{config.Address}:{config.Port}/");
        Console.WriteLine($"Server has been started. For address: {config.Address}:{config.Port}");
        server.Start();

        while (true)
        {
            context = await server.GetContextAsync();
            request = context.Request;
            response = context.Response;

            string requestedPath = request.Url.LocalPath;
            Console.WriteLine(requestedPath);

            if (requestedPath.EndsWith("/"))
            {
                try
                {
                    StreamReader site = new StreamReader("static/index.html");
                    byte[] buffer = Encoding.UTF8.GetBytes(site.ReadToEnd());
                    Console.WriteLine();
                    response.ContentType = "text/html";
                    response.ContentLength64 = buffer.Length;

                    using Stream output = response.OutputStream;

                    await output.WriteAsync(buffer);
                    await output.FlushAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
            else if (requestedPath.EndsWith(".css"))
            {
                try
                {
                    StreamReader site = new StreamReader("static/style.css");
                    byte[] buffer = Encoding.UTF8.GetBytes(site.ReadToEnd());
                    Console.WriteLine();
                    response.ContentType = "text/css";
                    response.ContentLength64 = buffer.Length;

                    using Stream output = response.OutputStream;

                    await output.WriteAsync(buffer);
                    await output.FlushAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }

            if (!(waitFinish.Status == TaskStatus.Running))
                break;

            Console.WriteLine("Запрос обработан");
        }

        server.Close();
        ((IDisposable)server).Dispose();
        Console.WriteLine("Server has been stopped.");
    }

    private AppSettingsConfig GetConfig(string filename)
    {
        AppSettingsConfig config;
        if (File.Exists(filename))
        {
            using (var file = new FileStream("appsettings.json", FileMode.Open))
            {
                config = System.Text.Json.JsonSerializer.Deserialize<AppSettingsConfig>(file);
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return config;
        }
        else
        {
            throw new FileNotFoundException(filename);
        }
    }
}