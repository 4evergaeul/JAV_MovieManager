using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using MovieManager.Data;
using MovieManager.Endpoint;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows;

namespace MovieManager.TrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int STARTPORT = 8100;
        private TaskbarIcon notifyIcon;
        private Process webAppProcess;
        private List<Process> httpServerProcesses = new List<Process>();


        public void Test() { }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            Program.Run(null);
            ExecuteCommands();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            KillProcessAndChildrens(webAppProcess.Id);
            foreach(var p in httpServerProcesses)
            {
                KillProcessAndChildrens(p.Id);
            }
            base.OnExit(e);
            Process.GetCurrentProcess().Kill();
        }

        private void ExecuteCommands()
        {
            var webAppLocation = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["WebAppDirectory"];
            var webAppProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + @$"serve {webAppLocation}");
            
            var currentPort = int.Parse(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["HttpServerStartPort"]);
            for (char c = 'A'; c <= 'Z'; c++)
            {
                var info = new ProcessStartInfo("cmd.exe", "/K " + $"http-server {c}:/ -p {currentPort}");
                info.CreateNoWindow = true;
                httpServerProcesses.Add(Process.Start(info));
                Thread.Sleep(100);
                currentPort++;
            }

            webAppProcessInfo.CreateNoWindow = true;
            webAppProcess = Process.Start(webAppProcessInfo);
        }

        private void KillProcessAndChildrens(int pid)
        {
            ManagementObjectSearcher processSearcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection processCollection = processSearcher.Get();

            try
            {
                Process proc = Process.GetProcessById(pid);
                if (!proc.HasExited) proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }

            if (processCollection != null)
            {
                foreach (ManagementObject mo in processCollection)
                {
                    KillProcessAndChildrens(Convert.ToInt32(mo["ProcessID"])); 
                }
            }
        }


    }
}
