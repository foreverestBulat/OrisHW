using MyHttpServer;
using MyHttpServer.Attribuets;
using MyHttpServer.models;
using MyHttpServer.Other;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHttpServer.Controllers;

[Controller("auth")]
public class AccountController
{
    static IMyDataContext dataBase = Data.Base;

    static List<Account> accounts = new List<Account>();

    public static Page Page = new Page("static/html/authorization.html");

    [Get("show")]
    public Page SendPage()
    {
        return Page;
    }

    [Post("add")]
    public object Add(string json)
    {
        var account = JsonSerializer.Deserialize<Account>(json);
        account.Id = (int)dataBase.Count<Account>() + 1;
        dataBase.Add(account);
        return account;
    }

    [Get("delete")]
    public void Remove(string stringId)
    {
        int id = -1;
        int.TryParse(stringId, out id);
        dataBase.Delete<Account>(id);
        Console.WriteLine("Удален пользователь");
    }

    [Post("update")]
    public object Update(string json)
    {
        var account = JsonSerializer.Deserialize<Account>(json);
        dataBase.Update<Account>(account);
        Console.WriteLine("Обновлен пользователь");
        return account;
    }

    [Get("getall")]
    public object GetAll()
    {
        return dataBase.Select<Account>();
    }

    [Get("getbyid")]
    public object GetByID(string stringId)
    {
        int.TryParse(stringId, out int id);
        if (id < 0)
            throw new ArgumentException("Не существует пользователь");

        return dataBase.SelectByID<Account>(id);
    }
}