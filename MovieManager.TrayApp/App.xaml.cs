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
using System.Windows.Controls;
using MovieManager.BusinessLogic;
using System.Net.Sockets;
using System.Net;

namespace MovieManager.TrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private Process webAppProcess;

        public void Test() { }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (IsAnotherInstanceRunning())
            {
                MessageBox.Show("程序已在其他进程打开，请在右下角托盘图标打开程序。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

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
            var initializingWindow = new InitializingWindow();
            initializingWindow.Show();
            Log.Information("Application is setting up...");

            // Start http-server.
            //StartHttpServers();

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
                var webAppStartPort = int.Parse(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["WebAppStartPort"]);
                for (int port = webAppStartPort; port <= webAppStartPort + 100; port++)
                {
                    Log.Information($"Trying to start the web app from port number {port}");
                    if (IsPortAvailable(port))
                    {
                        Log.Information($"Starting the web app from port number {port}");
                        var command = @$"set PORT={port} && serve {webAppLocation}";
                        var webAppProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + command)
                        {
                            RedirectStandardOutput = false,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        webAppProcess = Process.Start(webAppProcessInfo);
                        Thread.Sleep(1000);
                        AppStaticProperties.WebAppHost = $"http://localhost:{port}";
                        Process.Start("explorer.exe", AppStaticProperties.WebAppHost);
                        break;
                    }
                    if (port == webAppStartPort + 100)
                    {
                        Log.Error($"Unable to launch the web app from all posiable ports.");
                        MessageBox.Show($"程序初始化失败，请联系开发者。");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when starting web app. {ex.ToString()} ");
                MessageBox.Show($"程序初始化失败，请联系开发者。错误信息：{ex.ToString()}");
                CloseApp(true);
            }
            Log.Information("App started successfully!");
            initializingWindow.Hide();
            initializingWindow.Close();
        }

        private void StartHttpServers()
        {
            Log.Information("Creating http-server for hosting media...");
            try
            {
                var currentPort = int.Parse(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["HttpServerStartPort"]);
                using (var dbContext = new DatabaseContext())
                {
                    var sqlString = "select Value from UserSettings where Name = 'MovieDirectory'";
                    var movieDir = dbContext.Database.SqlQuery<string>(sqlString).FirstOrDefault();
                    sqlString = "select Value from UserSettings where Name = 'ActorFiguresDMMDirectory'";
                    var actorDMMDir = dbContext.Database.SqlQuery<string>(sqlString).FirstOrDefault();
                    sqlString = "select Value from UserSettings where Name = 'ActorFiguresAllDirectory'";
                    var actorAllDir = dbContext.Database.SqlQuery<string>(sqlString).FirstOrDefault();

                    // Start http-server for movie libs.
                    if (!string.IsNullOrEmpty(movieDir))
                    {
                        var movieDirectories = movieDir.Split('|');
                        foreach (var m in movieDirectories)
                        {
                            var disk = m.Substring(0, 1).Trim();
                            if (!AppStaticProperties.diskPortMappings.ContainsKey(disk))
                            {
                                AppStaticMethods.CreateHttpServer(currentPort, disk);
                                currentPort++;
                            }
                        }
                    }

                    // Start http-server for DMM actor thumbnails.
                    if (!string.IsNullOrEmpty(actorDMMDir))
                    {
                        var disk = actorDMMDir.Substring(0, 1).Trim();
                        if (!AppStaticProperties.diskPortMappings.ContainsKey(disk))
                        {
                            AppStaticMethods.CreateHttpServer(currentPort, disk);
                            currentPort++;
                        }
                    }

                    // Start http-server for All actor thumbnails
                    if (!string.IsNullOrEmpty(actorAllDir))
                    {
                        var disk = actorAllDir.Substring(0, 1).Trim();
                        if (!AppStaticProperties.diskPortMappings.ContainsKey(disk))
                        {
                            AppStaticMethods.CreateHttpServer(currentPort, disk);
                            currentPort++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An errro occured when creating http-server: {ex.ToString()}");
                MessageBox.Show($"程序初始化失败，请联系开发者。错误信息：{ex.ToString()}");
                CloseApp(true);
            }
        }

        private void CloseApp(bool forceShutdown = false)
        {
            var initializingWindow = new InitializingWindow();
            Log.Information("Application is closing...");
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
            }
            if (webAppProcess != null)
            {
                AppStaticMethods.KillProcessAndChildrens(webAppProcess.Id);
            }
            Log.Information("Web App shutdown.");
            foreach (var p in AppStaticProperties.portHttpServerProcessMappings.Values)
            {

                AppStaticMethods.KillProcessAndChildrens(p.Id);
            }
            Log.Information("Http-servers shutdown.");
            Log.Information("Application is closed.");
            Process.GetCurrentProcess().Kill();
            if (forceShutdown)
            {
                Application.Current.Shutdown();
            }
        }

        private bool IsAnotherInstanceRunning()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            return processes.Any(p => p.Id != currentProcess.Id);
        }

        private bool IsPortAvailable(int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
