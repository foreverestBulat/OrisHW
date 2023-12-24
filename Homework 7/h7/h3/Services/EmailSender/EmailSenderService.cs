﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyHttpServer.Services.EmailSender;

internal class EmailSenderService
{
    public static MailMessage CreateMail(string name, string emailFrom, string emailTo, string subject, string body)
    {
        var from = new MailAddress(emailFrom, name);
        var to = new MailAddress(emailTo);
        var mail = new MailMessage(from, to);
        mail.Subject = subject;
        mail.Body = body;
        //mail.IsBodyHtml = true;
        return mail;
    }

    public static void SendMail(string host, int smtpPort, string emailFrom, string pass, MailMessage mail)
    {
        SmtpClient smtpClient = new SmtpClient(host, smtpPort);
        smtpClient.Credentials = new NetworkCredential(emailFrom, pass);
        smtpClient.EnableSsl = true;
        smtpClient.Send(mail);
    }
}