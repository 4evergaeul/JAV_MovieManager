using Microsoft.Extensions.Options;
using MovieManager.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.BusinessLogic
{
    public class UtilityService
    {
        private Dictionary<string, int> diskPortMapping;
        private const int STARTPORT = 8100;

        public UtilityService(IOptions<UserSettings> config)
        {
            diskPortMapping = new Dictionary<string, int>();
            var currPort = STARTPORT;
            foreach (var l in config.Value?.MovieDirectory.Split(","))
            {
                diskPortMapping.Add(l.Trim().Substring(0, 1), currPort);
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
