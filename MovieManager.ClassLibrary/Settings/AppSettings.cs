using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.ClassLibrary.Settings
{
    public class AppSettings
    {
        public string WebAppDirectory { get; set; }
        public string WebAppHost { get; set; }
        public string EndpointHost { get; set; }
        public int HttpServerStartPort { get; set; }
    }
}
