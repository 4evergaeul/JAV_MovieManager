using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using MovieManager.Endpoint;
using System;
using System.Collections.Generic;
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
        }

        private void ExecuteCommands()
        {
            var webAppProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + @"serve C:\Projects\MovieManager\MovieManager.Web\build");
            var locations = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("UserSettings")["MovieDirectory"].Split(",").ToList();
            locations.AddRange(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("UserSettings")["ActorFiguresAllDirectory"].Split(",").ToList());
            var disks = new HashSet<string>();
            foreach(var l in locations)
            {
                var disk = l.Trim().Substring(0, 1);
                if (!disks.Contains(disk))
                {
                    disks.Add(disk);
                }
            }
            var currentPort = STARTPORT;
            foreach (var disk in disks)
            {
                var info = new ProcessStartInfo("cmd.exe", "/K " + $"http-server {disk}:/ -p {currentPort}");
                info.CreateNoWindow = true;
                Process.Start(info);
                Thread.Sleep(1000);
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
