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
        private const string version = "JavMovieManager_07262024_1";
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

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/log_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt", flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();

            Log.Information($"Application starting up...Current Version: {version}");

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
            initializingWindow.Hide();
            initializingWindow.Close();
        }

        private void CloseApp(bool forceShutdown = false)
        {
            var initializingWindow = new InitializingWindow();
            Log.Information("Application is closing...");
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
            }
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
