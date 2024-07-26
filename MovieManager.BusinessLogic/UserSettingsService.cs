using MovieManager.ClassLibrary;
using MovieManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieManager.BusinessLogic
{
    public class UserSettingsService
    {
        private MovieService _movieService;
        private XmlProcessor _xmlProcessor;

        public UserSettingsService(MovieService movieService
            ,XmlProcessor xmlProcessor)
        {
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
                var prevMovieDir = GetUserSettings().MovieDirectory;
                using (var dbContext = new DatabaseContext())
                {
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.MovieDirectory}' where Name = 'MovieDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.ActorFiguresDMMDirectory}' where Name = 'ActorFiguresDMMDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.ActorFiguresAllDirectory}' where Name = 'ActorFiguresAllDirectory'");
                    dbContext.Database.ExecuteSqlCommand($"update UserSettings set value = '{settings.PotPlayerDirectory}' where Name = 'PotPlayerDirectory'");
                }

                var newMovieDir = GetUserSettings().MovieDirectory;
                var movieDirToRemove = new List<string>();
                // Remove added movies from previous movie directories.
                if (!string.IsNullOrEmpty(prevMovieDir) && !string.IsNullOrEmpty(newMovieDir))
                {
                    var preMovieDirs = prevMovieDir.Split("|");
                    var newMovieDirs = newMovieDir.Split("|");
                    foreach (var preMovieDir in preMovieDirs)
                    {
                        if (!newMovieDir.Contains(preMovieDir))
                        {
                            movieDirToRemove.Add(preMovieDir);
                        }
                    }
                }
                foreach (var dir in movieDirToRemove)
                { 
                    var scanner = new FileScanner(_xmlProcessor);
                    _movieService.DeleteMoviesFromDirectory(scanner.ScanFilesForImdbId(dir));
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
