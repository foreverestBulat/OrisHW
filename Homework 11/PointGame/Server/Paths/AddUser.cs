using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Paths
{
    public class AddUser
    {
        public string UserName { get; set; }
        public Color Color { get; set; }

        public AddUser(string username, Color color)
        {
            UserName = username;
            Color = color;
        }
    }
}
