using MovieManager.ClassLibrary;
using MovieManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MovieManager.BusinessLogic
{
    public class GenreService
    {
        public GenreService()
        {
            CultureInfo PronoCi = new CultureInfo(2052);
        }

        public List<Genre> GetAll ()
        {
            var results = new List<Genre>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    results = dbContext.Genres.ToList();
                    results.Sort();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return results;
        }

        public List<Genre> Get(string searchString)
        {
            var results = new List<Genre>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var sqlString = @$"select * from Genre where Name like '%{searchString}%'";
                    results = dbContext.Database.SqlQuery<Genre>(sqlString).ToList();
                    results.Sort();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return results;
        }

        public List<string> GetAllNames()
        {
            var results = new List<string>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    results = dbContext.Genres.Select(x => x.Name).ToList();
                    results.Sort();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting genre names. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public List<string> GetLikedGenres()
        {
            var results = new List<string>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    results = dbContext.Genres.Where(x => x.Liked).Select(x => x.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting liked actor. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public bool LikeGenre(string genreName)
        {
            try
            {
                using (var context = new DatabaseContext())
                {
                    var genre = context.Genres.Where(x => x.Name == genreName).FirstOrDefault();
                    genre.Liked = !genre.Liked;
                    context.SaveChanges();
                    return genre.Liked;
                }

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when setting genre's like flag. \n\r");
                Log.Error(ex.ToString());
            }
            return false;
        }
    }
}
