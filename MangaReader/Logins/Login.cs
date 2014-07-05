using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MangaReader.Logins
{
    [XmlInclude(typeof(Login))]
    public class Login
    {
        public string Name { get; set; }

        public string Password { get; set; }
    }
}
