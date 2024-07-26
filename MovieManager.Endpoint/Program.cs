using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MovieManager.BusinessLogic;
using Serilog;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace MovieManager.Endpoint
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void Main(string[] args)
        {
            Run(args);
        }

        public static void Run(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    try
                    {
                        Log.Information("Starting web app...");
                        var endpointHost = int.Parse(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["EndpointHost"]);
                        for (int port = endpointHost; port <= endpointHost; ++port)
                        {
                            Log.Information($"Trying to start the web app from port number {port}");
                            if (AppStaticMethods.IsPortAvailable(port))
                            {
                                webBuilder.UseStartup<Startup>().UseUrls($"http://localhost:{port}");
                                AppStaticProperties.WebAppHost = $@"http://localhost:{port}/index.html";
                            }
                        }
                        Process.Start("explorer.exe", AppStaticProperties.WebAppHost);
                        Log.Information("App started successfully!");
                    }
                    catch(Exception ex)
                    {
                        Log.Error($"An error occurs when starting web app. {ex.ToString()} ");
                        MessageBox.Show($"程序初始化失败，请联系开发者。错误信息：{ex.ToString()}");
                        Application.Current.Shutdown();
                    }
                });
    }
}
