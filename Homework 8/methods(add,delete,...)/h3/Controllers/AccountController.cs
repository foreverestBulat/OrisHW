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
        account.Id = accounts.Count();
        accounts.Add(account);
        return account;
    }

    [Get("delete")]
    public void Remove(string stringId)
    {
        accounts.Remove((Account)GetByID(stringId));
        Console.WriteLine("Пользователь удален");
    }

    [Post("update")]
    public object Update(string json)
    {
        var account = JsonSerializer.Deserialize<Account>(json);
        var accountInList = accounts.Where(acc => acc.Login == account.Login).FirstOrDefault();
        if (accountInList != null)
        {
            accountInList.Login = account.Login;
            accountInList.Password = account.Password;
        }
        Console.WriteLine("Обновлен пользователь");
        return account;
    }

    [Get("getall")]
    public object GetAll()
    {
        return accounts;
    }

    [Get("getbyid")]
    public object GetByID(string stringId)
    {
        int id = -1;
        int.TryParse(stringId, out id);
        if (id < 0 || id > accounts.Count() - 1)
            throw new ArgumentException("Не существует пользователь");
        return accounts.Where(acc => acc.Id == id).FirstOrDefault();
    }
}