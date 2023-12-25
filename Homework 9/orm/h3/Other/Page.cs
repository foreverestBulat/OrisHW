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

    public Page(string localPath)
    {
        LocalPath = localPath;
    }

    public string Render()
    {
        string absolutePath = Path.GetFullPath(LocalPath);
        string applyPath;

        if (File.Exists(absolutePath))
            applyPath = absolutePath;
        else
            applyPath = path404;

        return File.ReadAllText(applyPath);
    }
}