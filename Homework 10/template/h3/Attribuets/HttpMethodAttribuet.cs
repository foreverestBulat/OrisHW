using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Attribuets;

public class HttpMethodAttribuet : Attribute
{
    public HttpMethodAttribuet(string actionName)
    {
        ActionName = actionName;
    }

    public string ActionName { get; set; }
}
