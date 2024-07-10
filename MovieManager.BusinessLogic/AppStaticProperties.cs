using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MovieManager.BusinessLogic
{
    public static class AppStaticProperties
    {
        public static string WebAppHost { get; set; }
        public static Dictionary<string, int> diskPortMappings { get; set; } = new Dictionary<string, int>();
        public static Dictionary<int, Process> portHttpServerProcessMappings { get; set; } = new Dictionary<int, Process>();
    }
}