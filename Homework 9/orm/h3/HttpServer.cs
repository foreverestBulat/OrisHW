using MyHttpServer.Handlers;
using MyHttpServer.Services.EmailSender;
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
            //context = await server.GetContextAsync();
            //request = context.Request;
            //response = context.Response;

            Console.WriteLine("Новый запрос");
            context = await server.GetContextAsync();

            Handler staticFilesHandler = new StaticFileHandlers(config);
            Handler controllerHandler = new ControllerHandler();
            staticFilesHandler.Successor = controllerHandler;
            staticFilesHandler.HandleRequest(context);
            Console.WriteLine("Запрос обработан");

            //string requestedPath = request.Url.LocalPath;

            //if (request.HttpMethod == "POST")
            //{
            //    string dataString;
            //    string name;
            //    string lastname;
            //    string birthday;
            //    string phone;

            //    using (var reader = new StreamReader(request.InputStream))
            //    {
            //        dataString = reader.ReadToEnd();
            //    }

            //    var datas = dataString.Split('&');
            //    name = datas[0].Split('=')[1];
            //    lastname = datas[1].Split('=')[1];
            //    birthday = datas[2].Split('=')[1];
            //    phone = datas[3].Split('=')[1];

            //    var subject = "Метод HOST";

            //    string body = $"{name}\n" +
            //        $"{lastname}\n" +
            //        $"{birthday}\n" +
            //        $"{phone}";


            //    var mail = EmailSenderService.CreateMail
            //        (name, config.EmailFrom, config.EmailTo, subject, body);
            //    EmailSenderService.SendMail
            //        (config.SmtpHost, config.SmtpPort, config.EmailFrom, config.EmailPassword, mail);

            //    Console.WriteLine("Анкета отправлена на почту Додо");
            //}





            //Console.WriteLine(requestedPath);
            //if (requestedPath.Contains("static"))
            //{
            //    SendStaticFile(requestedPath);
            //}
            //else if (requestedPath.EndsWith(".css"))
            //{
            //    // Отправить файл CSS
            //    SendCSSFile(requestedPath);
            //}
            //else if (requestedPath.StartsWith("/images/"))
            //{
            //    // Отправить изображение
            //    SendImageFile(requestedPath);
            //}
            //else
            //{
            //    // Отправка файл HTML
            //    SendHTMLFile();
            //}

            //if (!(waitFinish.Status == TaskStatus.Running))
            //    break;

            //Console.WriteLine("Запрос обработан");
        }

        server.Close();
        ((IDisposable)server).Dispose();
        Console.WriteLine("Server has been stopped.");
    }

    private async void SendStaticFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            response.ContentType = "text/css";
            response.ContentLength64 = fileBytes.Length;
            using Stream outputStream = response.OutputStream;
            await outputStream.WriteAsync(fileBytes);
            await outputStream.FlushAsync();
        }
        else
        {
            response.StatusCode = 404;
            response.Close();
        }
    }

    private async void SendHTMLFile()
    {
        if (CheckExistFileHTML(config))
        {
            StreamReader site = new StreamReader($"{config.StaticPathFiles}/html/index.html");
            byte[] buffer = Encoding.UTF8.GetBytes(site.ReadToEnd());
            Console.WriteLine();
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;

            using Stream output = response.OutputStream;

            await output.WriteAsync(buffer);
            await output.FlushAsync();
        }
        else
        {
            StreamReader site = new StreamReader($"{config.StaticPathFiles}/html/404.html");
            byte[] buffer = Encoding.UTF8.GetBytes(site.ReadToEnd());
            Console.WriteLine();
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;

            using Stream output = response.OutputStream;

            await output.WriteAsync(buffer);
            await output.FlushAsync();
        }
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