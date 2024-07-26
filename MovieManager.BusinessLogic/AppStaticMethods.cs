using Serilog;
using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MovieManager.BusinessLogic
{
    public static class AppStaticMethods
    {
        public static string GetDiskPort(string disk)
        {
            if (String.IsNullOrEmpty(disk))
            {
                return "";
            }
            return $"http://127.0.0.1:{AppStaticProperties.diskPortMappings[disk]}//";
        }

        public static void CreateHttpServer(int currentPort, string disk)
        {
            var info = new ProcessStartInfo("cmd.exe", "/K " + $"http-server {disk}:/ -p {currentPort}");
            info.CreateNoWindow = true;
            AppStaticProperties.portHttpServerProcessMappings.Add(currentPort, Process.Start(info));
            AppStaticProperties.diskPortMappings.Add(disk, currentPort);
            Log.Information($"Created http-server for {disk} drive at port {currentPort}");
            Thread.Sleep(100);
        }

        public static void DisposeHttpServer(string disk)
        {
            var portNumber = AppStaticProperties.diskPortMappings[disk];
            AppStaticProperties.diskPortMappings.Remove(disk);
            KillProcessAndChildrens(AppStaticProperties.portHttpServerProcessMappings[portNumber].Id);
            AppStaticProperties.portHttpServerProcessMappings.Remove(portNumber);
        }

        public static void KillProcessAndChildrens(int pid)
        {
            ManagementObjectSearcher processSearcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection processCollection = processSearcher.Get();

            try
            {
                Process proc = Process.GetProcessById(pid);
                if (!proc.HasExited) proc.Kill();
            }
            catch (ArgumentException ex)
            {
                // Process already exited.
                Log.Warning($"Process already exited. {ex.ToString()}");
            }

            if (processCollection != null)
            {
                foreach (ManagementObject mo in processCollection)
                {
                    KillProcessAndChildrens(Convert.ToInt32(mo["ProcessID"]));
                }
            }
        }

        public static bool IsPortAvailable(int port)
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
