using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Attribuets;

public class GetAttribute : HttpMethodAttribuet
{
    public GetAttribute(string actionName) : base(actionName)
    {

    }
}