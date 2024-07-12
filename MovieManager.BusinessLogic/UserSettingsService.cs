using Microsoft.Extensions.Options;
using MovieManager.ClassLibrary;
using MovieManager.ClassLibrary.Settings;
using MovieManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieManager.BusinessLogic
{
    public class UserSettingsService
    {
        private IOptions<AppSettings> _options;
        private MovieService _movieService;
        private XmlProcessor _xmlProcessor;

        public UserSettingsService(IOptions<AppSettings> options
            ,MovieService movieService
            ,XmlProcessor xmlProcessor)
        {
            _options = options;
            _movieService = movieService;
            _xmlProcessor = xmlProcessor;
        }


        public UserSettings GetUserSettings()
        {
            var settings = new UserSettings();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var sqlString = "select Value from UserSettings where Name = 'MovieDirectory'";
                    settings.MovieDirectory = dbContext.Database.SqlQuery<string>(sqlString).FirstOrDefault();
                    sqlString = "select Value from UserSettings where Name = 'ActorFiguresDMMDirectory'";
                    settings.ActorFiguresDMMDirectory = dbContext.Database.SqlQuery<string>(sqlString).FirstOrDefault();
                    sqlString = "select Value from UserSettings where Name = 'ActorFiguresAllDirectory'";
                    settings.ActorFiguresAllDirectory = dbContext.Database.SqlQuery<string>(sqlString).FirstOrDefault();
                    sqlString = "select Value from UserSettings where Name = 'PotPlayerDirectory'";
                    settings.PotPlayerDirectory = dbContext.Database.SqlQuery<string>(sqlString).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return settings;
        }

        public void SetUserSettings(UserSettings settings)
        {
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.MovieDirectory}' where Name = 'MovieDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.ActorFiguresDMMDirectory}' where Name = 'ActorFiguresDMMDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.ActorFiguresAllDirectory}' where Name = 'ActorFiguresAllDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.PotPlayerDirectory}' where Name = 'PotPlayerDirectory'");
                }
                var disks = new HashSet<string>();

                if (!string.IsNullOrEmpty(settings.MovieDirectory))
                {
                    var movieDirectories = settings.MovieDirectory.Split('|');
                    foreach (var m in movieDirectories)
                    {
                        var disk = m.Substring(0, 1).Trim();
                        if (!disks.Contains(disk))
                        {
                            disks.Add(disk);
                        }
                    }                }

                if (!string.IsNullOrEmpty(settings.ActorFiguresDMMDirectory))
                {
                    var disk = settings.ActorFiguresDMMDirectory.Substring(0, 1).Trim();
                    if (!disks.Contains(disk))
                    {
                        disks.Add(disk);
                    }
                }

                if (!string.IsNullOrEmpty(settings.ActorFiguresAllDirectory))
                {
                    var disk = settings.ActorFiguresAllDirectory.Substring(0, 1).Trim();
                    if (!disks.Contains(disk))
                    {
                        disks.Add(disk);
                    }
                }
                // Remove movies
                var scanner = new FileScanner(_xmlProcessor);
                _movieService.DeleteRemovedMovies(scanner.ScanFilesForImdbId(GetUserSettings().MovieDirectory));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void SetUserSettings_HttpServer(UserSettings settings)
        {
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.MovieDirectory}' where Name = 'MovieDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.ActorFiguresDMMDirectory}' where Name = 'ActorFiguresDMMDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.ActorFiguresAllDirectory}' where Name = 'ActorFiguresAllDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.PotPlayerDirectory}' where Name = 'PotPlayerDirectory'");
                }
                var disks = new HashSet<string>();
                var currentPort = AppStaticProperties.diskPortMappings.Count() == 0 ? _options.Value.HttpServerStartPort : AppStaticProperties.diskPortMappings.Values.Max() + 1;
                // Start http-server for movie libs.
                if (!string.IsNullOrEmpty(settings.MovieDirectory))
                {
                    var movieDirectories = settings.MovieDirectory.Split('|');
                    foreach (var m in movieDirectories)
                    {
                        var disk = m.Substring(0, 1).Trim();
                        if (!AppStaticProperties.diskPortMappings.ContainsKey(disk))
                        {
                            AppStaticMethods.CreateHttpServer(currentPort, disk);
                            currentPort++;
                        }
                        if (!disks.Contains(disk))
                        {
                            disks.Add(disk);
                        }
                    }
                }
                // Start http-server for DMM actor thumbnails.
                if (!string.IsNullOrEmpty(settings.ActorFiguresDMMDirectory))
                {
                    var disk = settings.ActorFiguresDMMDirectory.Substring(0, 1).Trim();
                    if (!AppStaticProperties.diskPortMappings.ContainsKey(disk))
                    {
                        AppStaticMethods.CreateHttpServer(currentPort, disk);
                        currentPort++;
                    }
                    if (!disks.Contains(disk))
                    {
                        disks.Add(disk);
                    }
                }
                // Start http-server for All actor thumbnails
                if (!string.IsNullOrEmpty(settings.ActorFiguresAllDirectory))
                {
                    var disk = settings.ActorFiguresAllDirectory.Substring(0, 1).Trim();
                    if (!AppStaticProperties.diskPortMappings.ContainsKey(disk))
                    {
                        AppStaticMethods.CreateHttpServer(currentPort, disk);
                        currentPort++;
                    }
                    if (!disks.Contains(disk))
                    {
                        disks.Add(disk);
                    }
                }
                // Remove unused port.
                foreach (var dvp in AppStaticProperties.diskPortMappings)
                {
                    if (!disks.Contains(dvp.Key))
                    {
                        AppStaticMethods.DisposeHttpServer(dvp.Key);
                    }
                }// Remove movies
                var scanner = new FileScanner(_xmlProcessor);
                _movieService.DeleteRemovedMovies(scanner.ScanFilesForImdbId(GetUserSettings().MovieDirectory));
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
