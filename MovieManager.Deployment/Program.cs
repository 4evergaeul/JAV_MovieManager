using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Xml;

namespace MovieManager.Deployment
{
    class Program
    {
        static void Main()
        {
            // Step 1: Build MovieManager.Tray
            // Assuming the build process is done manually or using a build tool
            // You can use a build tool like MSBuild or a script to automate the build process
            // Here's an example of how you can use MSBuild to build the MovieManager.Tray projec
            string solutionDirectory = Environment.CurrentDirectory;
            for (int i = 0; i < 4; i++)
            {
                solutionDirectory = Directory.GetParent(solutionDirectory).FullName;
            }
            string trayProjectPath = $@"{solutionDirectory}\MovieManager.TrayApp\MovieManager.TrayApp.csproj";
            string msBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = msBuildPath,
                Arguments = $"{trayProjectPath} /p:Configuration=Release /p:Platform=\"Any CPU\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process buildProcess = Process.Start(startInfo);
            buildProcess.WaitForExit();


            // Step 2: Copy build folder to Tray build folder
            string webBuildFolder = $@"{solutionDirectory}\MovieManager.Web\build";
            string trayBuildFolder = $@"{solutionDirectory}\MovieManager.TrayApp\bin\Any CPU\Release\netcoreapp3.1";
            CopyDirectory(webBuildFolder, $@"{trayBuildFolder}\build");

            // Step 3: Update appsettings.json in Tray build folder
            string appSettingsPath = Path.Combine(trayBuildFolder, "appsettings.json");
            UpdateAppSettings(appSettingsPath, "WebAppDirectory", "build");

            // Step 4: Update MovieManager.TrayApp.dll.config in Tray build folder
            string configFilePath = Path.Combine(trayBuildFolder, "MovieManager.TrayApp.dll.config");
            UpdateConfig(configFilePath, "DatabaseLocation", "MovieDb.db");

            // Step 5: Copy MovieDb_Clean.db to Tray build folder
            string dbSourcePath = $@"{solutionDirectory}\MovieManager.DB\MovieDb_Clean.db";
            string dbDestPath = Path.Combine(trayBuildFolder, "MovieDb.db");
            File.Copy(dbSourcePath, dbDestPath, true);

            // Step 6: Rename Tray build folder
            string newFolderName = $"MovieManager_{DateTime.Now.ToString("MMddyyyy_hhmmss")}";
            string newFolderPath = Path.Combine(Path.GetDirectoryName(trayBuildFolder), newFolderName);
            Directory.Move(trayBuildFolder, newFolderPath);
        }

        static void CopyDirectory(string sourceDir, string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            FileSystemInfo[] fileSystemInfos = dir.GetFileSystemInfos();

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
            {
                string sourcePath = fileSystemInfo.FullName;
                string targetPath = Path.Combine(targetDir, fileSystemInfo.Name);

                if (fileSystemInfo.GetType() == typeof(DirectoryInfo))
                {
                    CopyDirectory(sourcePath, targetPath);
                }
                else
                {
                    File.Copy(sourcePath, targetPath, true);
                }
            }
        }

        static void UpdateAppSettings(string filePath, string key, string value)
        {
            string json = File.ReadAllText(filePath);
            JObject jObject = JObject.Parse(json);
            jObject["AppSettings"][key] = value;
            File.WriteAllText(filePath, jObject.ToString());
        }

        static void UpdateConfig(string filePath, string key, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            XmlNode node = xmlDoc.SelectSingleNode($"//appSettings/add[@key='{key}']");
            if (node != null)
            {
                node.Attributes["value"].Value = value;
            }
            else
            {
                XmlElement element = xmlDoc.CreateElement("add");
                XmlAttribute attributeKey = xmlDoc.CreateAttribute("key");
                attributeKey.Value = key;
                element.Attributes.Append(attributeKey);

                XmlAttribute attributeValue = xmlDoc.CreateAttribute("value");
                attributeValue.Value = value;
                element.Attributes.Append(attributeValue);

                XmlNode appSettingsNode = xmlDoc.SelectSingleNode("//appSettings");
                appSettingsNode.AppendChild(element);
            }

            xmlDoc.Save(filePath);
        }
    }
}
