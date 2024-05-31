using MovieManager.ClassLibrary;
using MovieManager.Data;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MovieManager.Testing
{
    public class DatabaseContextTest
    {
        [Fact]
        public void GetMovies()
        {
            var movies = new List<Movie>();
            using (var context = new DatabaseContext())
            {
                movies = context.Movies.Take(10).ToList();
            }
            movies.Count.ShouldNotBe(0);
        }

        [Fact]
        public void CovertDate()
        {
            using(var context = new DatabaseContext())
            {
                var actors = context.Actors.ToList();
                foreach (var actor in actors)
                {
                    if(!string.IsNullOrEmpty(actor.DateofBirth))
                    {
                        var temp = new DateTime();
                        if (DateTime.TryParse(actor.DateofBirth, out temp))
                        {
                            actor.DateofBirth = temp.ToString("yyyy-MM-dd");
                        }
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
