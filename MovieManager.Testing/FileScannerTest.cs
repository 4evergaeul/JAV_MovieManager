using Microsoft.VisualBasic.FileIO;
using MovieManager.BusinessLogic;
using MovieManager.Data;
using Serilog;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Xunit;

namespace MovieManager.Testing
{
    public class FileScannerTest
    {
        public FileScannerTest()
        {
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .WriteTo.File($"logs/movieSrv-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt")
                        .CreateLogger();
        }

        [Fact]
        public void ScanFilesTest()
        {
            var fileLocation = @"";
            var xmlEngine = new XmlProcessor();
            var fileScanner = new FileScanner(xmlEngine);
            var movies = fileScanner.ScanFiles(fileLocation, DateTime.Parse("1/1/2022"));
            movies.Count().ShouldNotBe(0);
        }

        //[Fact]
        //public void UpdateArt()
        //{
        //    var nfos = Directory.GetFiles(@"E:\MyFile\New\有码", "*.nfo", System.IO.SearchOption.AllDirectories)
        //                .Where(f => File.GetCreationTime(f) >= DateTime.Parse("1/1/2022")).ToList();

        //    foreach (var nfo in nfos)
        //    {
        //        var xmlDoc = new XmlDocument();
        //        xmlDoc.Load(nfo);

        //        var fanart = xmlDoc.

        //        var filename = Path.GetFileNameWithoutExtension(nfo);
        //        var dir = Path.GetDirectoryName(nfo);
        //        var fanart = Directory.GetFiles(dir, $"{filename}-fanart.jpg").FirstOrDefault();
        //        var poster = Directory.GetFiles(dir, $"{filename}-poster.jpg").FirstOrDefault();
        //        var thumb = Directory.GetFiles(dir, $"{filename}-thumb.jpg").FirstOrDefault();

        //        fanart = Path.GetFileNameWithoutExtension(fanart);
        //        poster = Path.GetFileNameWithoutExtension(poster);
        //        thumb = Path.GetFileNameWithoutExtension(thumb);
        //    }
        //}

        /*[Fact]
        public void RenameNfoAndJpg()
        {
            var movieExtensions = new List<string> { ".avi", ".mp4", ".wmv", ".mkv" };
            var nfos = Directory.GetFiles(@"E:\MyFile\New\有码", "*.nfo", System.IO.SearchOption.AllDirectories)
                        .Where(f => File.GetCreationTime(f) >= DateTime.Parse("1/1/2022")).ToList();
            var t = new List<string>();
            foreach (var nfo in nfos)
            {
                try
                {
                    var dir = Path.GetDirectoryName(nfo);
                    var nfoFilename = Path.GetFileNameWithoutExtension(nfo);
                    var processedNfoFilename = nfoFilename;
                    if (nfoFilename.Contains("-C"))
                    {
                        processedNfoFilename = nfoFilename.Substring(0, nfoFilename.Length - 2);
                    }
                    var movie = Directory.GetFiles(dir, $"{processedNfoFilename}*.*").Where(f => movieExtensions.Any(f.ToLower().EndsWith)).FirstOrDefault();
                    var movieFilename = Path.GetFileNameWithoutExtension(movie);

                    if (!string.IsNullOrEmpty(movie) && movieFilename != nfoFilename)
                    {
                        t.Add(nfo);
                        var fanart = Directory.GetFiles(dir, $"{nfoFilename}-fanart.jpg").FirstOrDefault();
                        var poster = Directory.GetFiles(dir, $"{nfoFilename}-poster.jpg").FirstOrDefault();
                        var thumb = Directory.GetFiles(dir, $"{nfoFilename}-thumb.jpg").FirstOrDefault();

                        FileSystem.RenameFile(fanart, $"{movieFilename}-fanart.jpg");
                        FileSystem.RenameFile(poster, $"{movieFilename}-poster.jpg");
                        FileSystem.RenameFile(thumb, $"{movieFilename}-thumb.jpg");
                        FileSystem.RenameFile(nfo, $"{movieFilename}.nfo");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString() + "\n\r");
                }
            }

        }*/

        /*[Fact]
        public void CorrectFilename()
        {
            var movieExtensions = new List<string> { ".avi", ".mp4", ".wmv", ".mkv" };
            var movies = Directory.GetFiles(@"E:\MyFile\New\有码", "*..*", System.IO.SearchOption.AllDirectories)
                        .Where(f => movieExtensions.Any(f.ToLower().EndsWith)).ToList();
            var renameMovies = new List<string>();
            foreach (var m in movies)
            {
                var movieFilename = Path.GetFileNameWithoutExtension(m);
                var nfo = Directory.GetFiles(Path.GetDirectoryName(m), $"{movieFilename}.nfo").FirstOrDefault();
                if (String.IsNullOrEmpty(nfo) || Path.GetFileNameWithoutExtension(nfo) != movieFilename)
                {
                    renameMovies.Add(m);
                }
            }

            foreach (var m in renameMovies)
            {
                var filename = Path.GetFileNameWithoutExtension(m);
                var ext = Path.GetExtension(m);
                var correctFilename = filename.Substring(0, filename.Length - 1) + ext;
                FileSystem.RenameFile(m, correctFilename);
            }

        }*/

        /*[Fact]
        public void DeleteInvalidNfos()
        {
            var fileLocation = @"E:\MyFile\New\有码\";
            var nfos = Directory.GetFiles(fileLocation, "*.nfo", SearchOption.AllDirectories)
                .Where(f => File.GetCreationTime(f) >= DateTime.Now.AddDays(-90)).ToList();
            var invalidNfos = new List<string>();
            var xmlDoc = new XmlDocument();
            foreach (var nfo in nfos)
            {
                xmlDoc.Load(nfo);
                var year = xmlDoc.GetElementsByTagName("year")[0]?.InnerText;
                if(string.IsNullOrEmpty(year))
                {
                    invalidNfos.Add(nfo);
                }
            }
            foreach(var nfo in invalidNfos)
            {
                File.Delete(nfo);
            }
        }*/
    }
}
