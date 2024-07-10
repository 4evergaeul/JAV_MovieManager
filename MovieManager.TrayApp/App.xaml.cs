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


                //for (char c = 'A'; c <= 'Z'; c++)
                //{
                //    var info = new ProcessStartInfo("cmd.exe", "/K " + $"http-server {c}:/ -p {currentPort}");
                //    info.CreateNoWindow = true;
                //    httpServerProcesses.Add(Process.Start(info));
                //    Log.Information($"Created http-server for {c} drive at port {currentPort}");
                //    Thread.Sleep(100);
                //    currentPort++;
                //}
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
                    while(string.IsNullOrEmpty(line)) 
                    {
                        line = reader.ReadLine();
                    }
                    string port = "";

                    var match = Regex.Match(line, @"http:\/\/localhost:(\d+)");
                    if (match != null)
                    {
                        if (match.Success)
                        {
                            port = match.Groups[1].Value;
                        }
                        if (port != "")
                        {
                            AppStaticProperties.WebAppHost = $"http://localhost:{port}";
                            Log.Information($"Web App started at port {port}");
                        }
                        else
                        {
                            Log.Warning("Port not found in the output.");
                        }
                    }
                    else
                    {
                        Log.Warning("Port not found in the output. Use 3000 as port for now.");
                        AppStaticProperties.WebAppHost = $"http://localhost:3000";
                    }

                }
                AppStaticProperties.WebAppHost = $"http://localhost:{3000}";
                Thread.Sleep(1000);
                File.Delete(filePathForRead);
                Process.Start("explorer.exe", AppStaticProperties.WebAppHost);
            }
            catch(Exception ex)
            {
                Log.Error($"An error occurs when starting web app. {ex.ToString()} ");
                MessageBox.Show($"程序初始化失败，请联系开发者。错误信息：{ex.ToString()}");
                CloseApp(true);
            }
            Log.Information("App started successfully!");
            initializingWindow.Hide();
            initializingWindow.Close();
        }

        private void CloseApp(bool forceShutdown = false)
        {
            var initializingWindow = new InitializingWindow();
            Log.Information("Application is closing...");
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
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
    }
}
