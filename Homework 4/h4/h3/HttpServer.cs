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
    private AppSettingsConfig config;

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
        config = GetConfig("appsettings.json");
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
                    StreamReader site = new StreamReader($"{config.StaticPathFiles}/index.html");
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
                    Console.WriteLine(ex.Message);
                }
            }
            else if (requestedPath.EndsWith(".css"))
            {
                try
                {
                    StreamReader site = new StreamReader($"{config.StaticPathFiles}/style.css");
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
            else
            {
                StreamReader site = new StreamReader($"{config.StaticPathFiles}/404.html");
                byte[] buffer = Encoding.UTF8.GetBytes(site.ReadToEnd());
                Console.WriteLine();
                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;

                using Stream output = response.OutputStream;

                await output.WriteAsync(buffer);
                await output.FlushAsync();
            }

            if (!(waitFinish.Status == TaskStatus.Running))
                break;

            Console.WriteLine("Запрос обработан");
        }

        server.Close();
        ((IDisposable)server).Dispose();
        Console.WriteLine("Server has been stopped.");
    }


    private async void SendCSSFile(string filePath)
    {
        string fullPath = Path.Combine(Environment.CurrentDirectory, config.StaticPathFiles, filePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            byte[] fileBytes = File.ReadAllBytes(fullPath);
            response.ContentType = "text/css";
            response.ContentLength64 = fileBytes.Length;
            using Stream outputStream = response.OutputStream;
            await outputStream.WriteAsync(fileBytes);
            await outputStream.FlushAsync();
        }
        else
        {
            // Если файл не найден, отправляем код ошибки 404 - Not Found
            response.StatusCode = 404;
            response.Close();
        }
    }

    private string GetImageContentType(string imagePath)
    {
        string extension = Path.GetExtension(imagePath).ToLower();
        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".svg":
                return "image/svg+xml";
            default:
                return "application/octet-stream"; // Если формат неизвестен, отправляем общий тип содержимого
        }
    }

    //private async void UseDataBase()
    //{
    //    //IMyDataContext db = new MyDataContext();    
    //}

    private async void SendImageFile(string imagePath)
    {
        string fullPath = Path.Combine(Environment.CurrentDirectory, "static", imagePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            byte[] imageBytes = File.ReadAllBytes(fullPath);
            string contentType = GetImageContentType(fullPath);
            response.ContentType = contentType;
            response.ContentLength64 = imageBytes.Length;
            using Stream outputStream = response.OutputStream;
            await outputStream.WriteAsync(imageBytes);
            await outputStream.FlushAsync();
        }
        else
        {
            // Если файл не найден, отправляем код ошибки 404 - Not Found
            response.StatusCode = 404;
            response.Close();
        }
    }

    internal bool CheckExistFileHTML(AppSettingsConfig config)
    {
        if (File.Exists($"{config.StaticPathFiles}/index.html"))
        {
            return true;
        }
        else
        {
            return false;
            //Console.WriteLine("index.html не найден");
            //response.StatusCode = 404;
            //response.Close();
        }
    }

    private void CheckExistFolderStatic(AppSettingsConfig config)
    {
        if (!Directory.Exists(config.StaticPathFiles))
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), config.StaticPathFiles));
                Console.WriteLine("Была создана папка static", config.StaticPathFiles);
            }
            catch
            {
                Console.WriteLine("Невозможно создать папку");
            }
        }
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
            CheckExistFolderStatic(config);

            return config;
        }
        else
        {
            throw new FileNotFoundException(filename);
        }
    }
}