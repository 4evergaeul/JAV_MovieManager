using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using MovieManager.Data;
using MovieManager.Endpoint;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using Serilog;
using System.Linq.Expressions;

namespace MovieManager.TrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private Process webAppProcess;
        private List<Process> httpServerProcesses = new List<Process>();

        public void Test() { }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/log_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt", flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();

            Log.Information("Application starting up...");

            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            Program.Run(null);
            ExecuteCommands();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CloseApp();
            base.OnExit(e);
        }

        private void ExecuteCommands()
        {
            Log.Information("Application is setting up...");
            // Start http-server.
            Log.Information("Creating http-server for hosting media...");
            try
            {
                var currentPort = int.Parse(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["HttpServerStartPort"]);
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    var info = new ProcessStartInfo("cmd.exe", "/K " + $"http-server {c}:/ -p {currentPort}");
                    info.CreateNoWindow = true;
                    httpServerProcesses.Add(Process.Start(info));
                    Log.Information($"Created http-server for {c} drive at port {currentPort}");
                    Thread.Sleep(100);
                    currentPort++;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An errro occured when creating http-server: {ex.ToString()}");
                MessageBox.Show($"程序初始化失败，请联系开发者。错误信息：{ex.ToString()}");
                CloseApp(true);
            }
            
            // Start web app.
            Log.Information("Starting web app...");
            try
            { 
                var webAppLocation = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["WebAppDirectory"];
                var filename = "output.txt";
                var filenameForRead = $"output_{DateTime.Now.ToString("MMddyyyy_hhmmss")}.txt";
                var filePath = $@"{Environment.CurrentDirectory}\{filename}";
                var filePathForRead = $@"{Environment.CurrentDirectory}\{filenameForRead}";
                if (Directory.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                var webAppProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + @$"serve {webAppLocation} > {filename}")
                {
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                webAppProcess = Process.Start(webAppProcessInfo);
                Thread.Sleep(1000);
                File.Copy(filePath, filePathForRead);
                using (var reader = new StreamReader(filePathForRead))
                {
                    var line = reader.ReadLine();
                    string port = "";

                    var match = Regex.Match(line, @"http:\/\/localhost:(\d+)");
                    if (match.Success)
                    {
                        port = match.Groups[1].Value;
                    }
                    if (port != "")
                    {
                        AppController.WebAppHost = $"http://localhost:{port}";
                        Log.Information($"Web App started at port {port}");
                    }
                    else 
                    { 
                        Log.Warning("Port not found in the output.");
                    }
                }
                AppController.WebAppHost = $"http://localhost:{3000}";
                Thread.Sleep(1000);
                File.Delete(filePathForRead);
                Process.Start("explorer.exe", AppController.WebAppHost);
            }
            catch(Exception ex)
            {
                Log.Error($"An error occurs when starting web app. {ex.ToString()} ");
                MessageBox.Show($"程序初始化失败，请联系开发者。错误信息：{ex.ToString()}");
                CloseApp(true);
            }
            Log.Information("App started successfully!");
        }

        private void CloseApp(bool forceShutdown = false)
        {
            Log.Information("Application is closing...");
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            if (webAppProcess != null)
            { 
                KillProcessAndChildrens(webAppProcess.Id);
            }
            foreach (var p in httpServerProcesses)
            {
                KillProcessAndChildrens(p.Id);
            }            
            Process.GetCurrentProcess().Kill();
            if (forceShutdown ) 
            {
                Application.Current.Shutdown();
            }
            Log.Information("Application is closed.");
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
