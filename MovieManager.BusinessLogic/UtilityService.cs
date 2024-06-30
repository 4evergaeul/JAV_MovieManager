using Microsoft.Extensions.Options;
using MovieManager.ClassLibrary;
using MovieManager.ClassLibrary.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MovieManager.BusinessLogic
{
    public class UtilityService
    {
        private Dictionary<string, int> diskPortMapping;

        public UtilityService(IOptions<AppSettings> appConfig)
        {
            diskPortMapping = new Dictionary<string, int>();
            var currPort = appConfig.Value.HttpServerStartPort;

            for (char c = 'A'; c <= 'Z'; c++)
            {
                diskPortMapping.Add(c.ToString(), currPort);
                currPort++;
            }         
        }

        public string GetDiskPort(string disk)
        {
            if (String.IsNullOrEmpty(disk))
            {
                return "";
            }
            return $"http://127.0.0.1:{diskPortMapping[disk]}//";
        }
    }
}
