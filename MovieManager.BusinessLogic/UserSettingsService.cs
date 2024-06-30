using MovieManager.ClassLibrary;
using MovieManager.Data;
using Serilog;
using System;
using System.Linq;

namespace MovieManager.BusinessLogic
{
    public class UserSettingsService
    {

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
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
