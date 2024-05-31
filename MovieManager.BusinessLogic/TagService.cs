using MovieManager.ClassLibrary;
using MovieManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MovieManager.BusinessLogic
{
    public class TagService
    {
        public TagService()
        {
            CultureInfo PronoCi = new CultureInfo(2052);
        }

        public List<Tag> GetAll()
        {
            var results = new List<Tag>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    results = dbContext.Tags.ToList();
                    results.Sort();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return results;
        }

        public List<Tag> Get(string searchString)
        {
            var results = new List<Tag>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    var sqlString = @$"select * from Tag where Name like '%{searchString}%'";
                    results = dbContext.Database.SqlQuery<Tag>(sqlString).ToList();
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
                    results = dbContext.Tags.Select(x => x.Name).ToList();
                    results.Sort();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting tag names. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public List<string> GetLikedTags()
        {
            var results = new List<string>();
            try
            {
                using (var dbContext = new DatabaseContext())
                {
                    results = dbContext.Tags.Where(x => x.Liked).Select(x => x.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting liked tags. \n\r");
                Log.Error(ex.ToString());
            }
            return results;
        }

        public bool LikeTag(string tagName)
        {
            try
            {
                using (var context = new DatabaseContext())
                {
                    var tag = context.Tags.Where(x => x.Name == tagName).FirstOrDefault();
                    tag.Liked = !tag.Liked;
                    context.SaveChanges();
                    return tag.Liked;
                }

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when setting tag's like flag. \n\r");
                Log.Error(ex.ToString());
            }
            return false;
        }
    }
}
