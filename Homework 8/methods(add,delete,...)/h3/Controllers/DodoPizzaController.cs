using MyHttpServer.Attribuets;
using MyHttpServer.models;
using MyHttpServer.Other;
using MyHttpServer.Services.EmailSender;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyHttpServer.Controllers;

[Controller("dodopizza")]
public class DodoPizzaController
{
    public static Page Page = new Page("static/html/index.html");

    AppSettingsConfig config = Configuration.GetConfig("appsettings.json");

    [Get("show")]
    public Page SendPage()
    {
        return Page;
    }

    [Post("send")]
    public object SendMail(string json)
    {
        string dataString;

        var mailSend = JsonSerializer.Deserialize<MailSend>(json);

        string name;
        string lastname;
        string birthday;
        string phone;


        name = mailSend.name;
        lastname = mailSend.lastname;
        birthday = mailSend.birthDay;
        phone = mailSend.number;

        var subject = "Метод HOST";

        string body = $"{name}\n" +
            $"{lastname}\n" +
            $"{birthday}\n" +
            $"{phone}";


        var mail = EmailSenderService.CreateMail
            (name, config.EmailFrom, config.EmailTo, subject, body);
        EmailSenderService.SendMail
            (config.SmtpHost, config.SmtpPort, config.EmailFrom, config.EmailPassword, mail);

        Console.WriteLine("Анкета отправлена на почту Додо");

        return null;
    }
}