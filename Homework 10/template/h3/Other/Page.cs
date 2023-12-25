using MyHttpServer.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Other;

public class Page
{
    static string path404 = "static/html/404.html";

    public string LocalPath;
    private Dictionary<string, object> InsertInPage;

    public Page(string localPath, Dictionary<string, object> insertInPage = null)
    {
        LocalPath = localPath;
        InsertInPage = insertInPage;
    }

    public string Render()
    {
        string absolutePath = Path.GetFullPath(LocalPath);
        string applyPath;

        if (File.Exists(absolutePath))
            applyPath = absolutePath;
        else
            applyPath = path404;

        TemplateEngine engine = new TemplateEngine();
        engine.SetVariables(File.ReadAllText(applyPath), InsertInPage);
        engine.Render();

        return engine.GetResult();
    }
}