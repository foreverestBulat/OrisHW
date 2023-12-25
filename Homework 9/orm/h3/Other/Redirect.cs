using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Other;

public class Redirect
{
    public string Path { get; set; }
    public int StatusCode { get; set; }

    public Redirect(string path, int statusCode)
    {
        Path = path;
        StatusCode = statusCode;
    }
}
