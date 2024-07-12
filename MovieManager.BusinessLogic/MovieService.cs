using Microsoft.Extensions.Options;
using MovieManager.ClassLibrary;
using MovieManager.ClassLibrary.RequestBody;
using MovieManager.Data;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieManager.BusinessLogic
{
    public class MovieService
    {
        private ScrapeService _scrapeService;

        public MovieService(ScrapeService scrapeService)
        {
            _scrapeService = scrapeService;
        }

        public async Task InsertMovies(List<Movie> movies, bool scrapeActorInfo = false, bool forceUpdate = false)
        {
            var currentImdb = string.Empty;
            var count = 0;
            try
            {
                using (var context = new DatabaseContext())
                {
                    var distinctActors = movies.SelectMany(x => x.Actors.Select(x => x)).Distinct().ToList();
                    var distinctGenres = movies.SelectMany(x => x.Genres.Select(x => x)).Distinct().ToList();
                    var distinctTags = movies.SelectMany(x => x.Tags.Select(x => x)).Distinct().ToList();

                    var tasks = new List<Task>();
                    tasks.Add(InsertActors(context, distinctActors, scrapeActorInfo));
                    tasks.Add(InsertGenres(context, distinctGenres));
                    tasks.Add(InsertTags(context, distinctTags));
                    await Task.WhenAll(tasks);

                    foreach (var movie in movies)
                    {
                        try
                        {
                            var status = "";
                            currentImdb = movie.ImdbId;
                            Log.Debug($"Start to process movie: {movie.ImdbId}.");
                            var exisitingMovie = context.Movies.Where(x => x.ImdbId == movie.ImdbId).FirstOrDefault();
                            if (exisitingMovie == null)
                            {
                                InsertMovie(context, movie);
                                count++;
                                status = "add";
                                
                            }
                            else if (exisitingMovie != null)
                            {
                                exisitingMovie.Actors = context.MovieActors.Where(x => x.ImdbId == movie.ImdbId).Select(x => x.ActorName).ToList();
                                exisitingMovie.Genres = context.MovieGenres.Where(x => x.ImdbId == movie.ImdbId).Select(x => x.GenreName).ToList();
                                exisitingMovie.Tags = context.MovieTags.Where(x => x.ImdbId == movie.ImdbId).Select(x => x.TagName).ToList();
                                if (!string.IsNullOrEmpty(movie.DateAdded) && !string.IsNullOrEmpty(exisitingMovie.DateAdded))
                                {
                                    //|| (DateTime.Parse(movie.DateAdded) > DateTime.Parse(exisitingMovie.DateAdded))
                                    if (CheckUpdate(movie,exisitingMovie) || forceUpdate)
                                    {
                                        UpdateMovie(context, movie, exisitingMovie);
                                        status = "update";
                                        Log.Information($"Updating {movie.ImdbId} data...");
                                        count++;
                                    }
                                }
                            }
                            context.SaveChanges();
                            switch (status)
                            {
                                case "add":
                                    Log.Debug($"Movie: {movie.ImdbId} has been added.");
                                    break;
                                case "update":
                                    Log.Debug($"Movie: {movie.ImdbId} has been updated.");
                                    break;
                                case "":
                                    Log.Debug($"No Changes on {movie.ImdbId}");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"An error occurs when processing movie: {currentImdb} \n\r");
                            Log.Error(ex.ToString());
                        }
                    }
                    context.SaveChanges();
                    Log.Debug($"{count} movies have been added!");
                    Log.Debug($"All movies have been processed successfully!");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when processing movies. \n\r");
                Log.Error(ex.ToString());
            }
        }

        public List<string> DeleteRemovedMovies(List<string> movies)
        {
            var moviesToRemove = new List<Movie>();
            using(var context = new DatabaseContext())
            {
                moviesToRemove = context.Movies.Where(x => movies.Contains(x.ImdbId) == false).ToList();
                DeleteMovies(moviesToRemove);
            }
            return moviesToRemove.Select(x => x.ImdbId).ToList();
        }

        public List<MovieViewModel> GetMovies()
        {
            var result = new List<MovieViewModel>();
            try
            {
                using (var context = new DatabaseContext())
                {
                    var movies = context.Movies.ToList();
                    result = BuildMovieViewModel(movies);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting all movies. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public int GetMoviesCount()
        {
            var result = 0;
            try
            {
                using (var context = new DatabaseContext())
                {                   
                    result = context.Movies.Count();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting all movies. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public List<MovieViewModel> GetMoviesByFilters(FilterType filterType, List<string> filters, bool isAndOperator)
        {
            var result = new List<MovieViewModel>();
            var movies = new List<Movie>();
            try
            {
                var sb = new StringBuilder();
                var searchString = string.Empty;
                var searchLength = filters.Count;
                var sqlString = string.Empty;
                if (filterType != FilterType.Directors || filterType != FilterType.Years)
                {
                    for (int i = 0; i < filters.Count; ++i)
                    {
                        if (i == filters.Count - 1)
                        {
                            sb.Append($"'{filters[i]}'");
                        }
                        else
                        {
                            sb.Append($"'{filters[i]}',");
                        }
                    }
                    searchString = sb.ToString();
                }

                var yearSearchFilters = new List<int>();
                if(filterType == FilterType.Years)
                {
                    foreach (var yearString in filters)
                    {
                        yearSearchFilters.Add(int.Parse(yearString));
                    }
                }

                using (var context = new DatabaseContext())
                {
                    switch (filterType)
                    {
                        case FilterType.Actors:
                            sqlString = "select * from " +
                            "(select ImdbId, count(ImdbId) as cnt " +
                            $"from MovieActors WHERE ActorName in ({searchString}) group by ImdbId) abc " +
                            $"join Movie m on abc.ImdbId = m.ImdbId where cnt >= {(isAndOperator ? 1 : searchLength).ToString()};";
                            movies = context.Database.SqlQuery<Movie>(sqlString).ToList();
                            break;
                        case FilterType.Genres:
                            sqlString = "select * from " +
                            "(select ImdbId, count(ImdbId) as cnt " +
                            $"from MovieGenres WHERE GenreName in ({searchString}) group by ImdbId) abc " +
                            $"join Movie m on abc.ImdbId = m.ImdbId where cnt >= {(isAndOperator ? 1 : searchLength).ToString()};";
                            movies = context.Database.SqlQuery<Movie>(sqlString).ToList();
                            break;
                        case FilterType.Tags:
                            sqlString = "select * from " +
                            "(select ImdbId, count(ImdbId) as cnt " +
                            $"from MovieTags WHERE TagName in ({searchString}) group by ImdbId) abc " +
                            $"join Movie m on abc.ImdbId = m.ImdbId where cnt >= {(isAndOperator ? 1 : searchLength).ToString()};";
                            movies = context.Database.SqlQuery<Movie>(sqlString).ToList();
                            break;
                        case FilterType.Directors:
                            movies = context.Movies.Where(x => filters.Contains(x.Director)).ToList();
                            break;
                        case FilterType.Years:
                            movies = context.Movies.Where(x => yearSearchFilters.Contains(x.Year)).ToList();
                            break;
                    }
                    result = BuildMovieViewModel(movies);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting movies. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public List<MovieViewModel> GetMoviesWildcard(SearchRequest searchRequest)
        {
            var result = new List<MovieViewModel>();
            try
            {
                var searchString = searchRequest.SearchString.Trim().Replace(' ', '%');
                var sqlString = "";
                using (var context = new DatabaseContext())
                {
                    switch (searchRequest.SearchType)
                    {
                        case "Actor":
                            sqlString = $"select * from Movie m join MovieActors ma on m.ImdbId = ma.ImdbId where Title like '%{searchString}%' and ActorName = '{searchRequest.SearchString2}'";
                            break;
                        case "Genre":
                            sqlString = $"select * from Movie m join MovieGenres mg on m.ImdbId = mg.ImdbId where Title like '%{searchString}%' and GenreName = '{searchRequest.SearchString2}'";
                            break;
                        case "Tag":
                            sqlString = $"select * from Movie m join MovieTags mt on m.ImdbId = mt.ImdbId where Title like '%{searchString}%' and TagName = '{searchRequest.SearchString2}'";
                            break;
                        default:
                            sqlString = $"select * from Movie where Title like '%{searchString}%'";
                            break;
                    }
                    var movies = context.Database.SqlQuery<Movie>(sqlString).ToList();
                    result = BuildMovieViewModel(movies);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting movies. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public List<MovieViewModel> GetMoviesByQuery(string searchString)
        {
            var result = new List<MovieViewModel>();
            try
            {
                using (var context = new DatabaseContext())
                {
                    var movies = context.Database.SqlQuery<Movie>(searchString).ToList();
                    result = BuildMovieViewModel(movies);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting movies. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public List<MovieViewModel> GetMostRecentMovies()
        {
            var result = new List<MovieViewModel>();
            var movies = new List<Movie>();
            try
            {
                using (var context = new DatabaseContext())
                {
                    //var month = DateTime.Now.AddMonths(-3).Month.ToString();
                    //var year = DateTime.Now.AddMonths(-3).Year.ToString();
                    //var sqlString = $"select * from Movie where DATE(DateAdded) > Date('{year}-{month}-01') order by DateAdded desc";
                    //movies = context.Database.SqlQuery<Movie>(sqlString).ToList();
                    movies = context.Movies.OrderByDescending(x => x.DateAdded).Take(100).ToList();
                    result = BuildMovieViewModel(movies);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting movies. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public List<MovieViewModel> GetLikedMovies()
        {
            var result = new List<MovieViewModel>();
            var movies = new List<Movie>();
            try
            {
                using (var context = new DatabaseContext())
                {
                    movies = context.Movies.Where(x => x.Liked).ToList();
                    result = BuildMovieViewModel(movies);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting liked movies. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public List<int> GetMovieYears()
        {
            var result = new List<int>();
            try
            {
                using (var context = new DatabaseContext())
                {
                    result = context.Movies.Select(x => x.Year).Distinct().ToList();
                    result.Sort();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting movie years. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        public MovieDetails GetMovieDetails(MovieViewModel mvm)
        {
            MovieDetails movieDetails = null;
            using(var context = new DatabaseContext())
            {
                var movie = context.Movies.Where(x => x.ImdbId == mvm.ImdbId).FirstOrDefault();
                var actors = context.MovieActors
                   .Where(x => x.ImdbId == mvm.ImdbId)
                   .Join(context.Actors,
                           ma => ma.ActorName,
                           a => a.Name,
                           (ma, a) => new ActorViewModel()
                           {
                               Name = a.Name,
                               DateofBirth = a.DateofBirth,
                               Height = a.Height,
                               LastUpdated = a.LastUpdated,
                               Cup = a.Cup,
                               Liked = a.Liked
                           }).ToList();
                var genres = context.MovieGenres
                    .Where(x => x.ImdbId == mvm.ImdbId)
                    .Join(context.Genres,
                            mg => mg.GenreName,
                            g => g.Name,
                            (mg, g) => new GenreViewModel()
                            {
                                Name = g.Name
                            }).ToList();
                var tags = context.MovieTags
                    .Where(x => x.ImdbId == mvm.ImdbId)
                    .Join(context.Tags,
                            mt => mt.TagName,
                            t => t.Name,
                            (mt, t) => new TagViewModel()
                            {
                                Name = t.Name
                            }).ToList();
                movieDetails = new MovieDetails()
                {
                    ImdbId = mvm.ImdbId,
                    Title = mvm.Title,
                    Plot = movie.Plot,
                    Year = movie.Year,
                    Runtime = movie.Runtime,
                    Studio = movie.Studio,
                    PosterFileLocation = mvm.PosterFileLocation,
                    FanArtLocation = mvm.FanArtLocation,
                    MovieLocation = mvm.MovieLocation,
                    PlayedCount = movie.PlayedCount,
                    DateAdded = movie.DateAdded,
                    ReleaseDate = movie.ReleaseDate,
                    Liked = movie.Liked,
                    Genres = genres,
                    Tags = tags,
                    Actors = actors
                };
            }
            return movieDetails;

        }

        public bool LikeMovie(string imdbId)
        {
            try
            {
                using(var context = new DatabaseContext())
                {
                    var movie = context.Movies.Where(x => x.ImdbId == imdbId).FirstOrDefault();
                    movie.Liked = !movie.Liked;
                    context.SaveChanges();
                    return movie.Liked;
                }

            }
            catch(Exception ex)
            {
                Log.Error($"An error occurs when setting movie's like flag. \n\r");
                Log.Error(ex.ToString());
            }
            return false;
        }

        public string GetImagePath(ImageRequest r)
        {
            var result = "";
            try
            {
                using (var context = new DatabaseContext())
                {
                    switch (r.ImageType)
                    {
                        case 0:
                            result = context.Movies.Where(x => x.ImdbId == r.Id).FirstOrDefault()?.PosterFileLocation;
                            break;
                        case 1:
                            result = context.Movies.Where(x => x.ImdbId == r.Id).FirstOrDefault()?.FanArtLocation;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when getting image path. \n\r");
                Log.Error(ex.ToString());
            }
            return result;
        }

        private bool CheckUpdate(Movie movie, Movie existingMovie)
        {
            var properties = typeof(Movie).GetProperties();
            foreach (var prop in properties)
            {
                // Skip ImdbId
                if (prop.Name == nameof(Movie.ImdbId)
                    || prop.Name == nameof(Movie.PlayedCount))
                {
                    continue;
                }

                if (prop.PropertyType == typeof(List<string>))
                {
                    var movieList = prop.GetValue(movie) as List<string>;
                    var existingMovieList = prop.GetValue(existingMovie) as List<string>;

                    if (movieList.Count != existingMovieList.Count)
                    {
                        return true;
                    }
                    else
                    {
                        if (!movieList.SequenceEqual(existingMovieList))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    // Get the values of the properties for both objects
                    var movieValue = prop.GetValue(movie);
                    var existingMovieValue = prop.GetValue(existingMovie);

                    // Compare the values, if they are not equal return true
                    if (!Equals(movieValue, existingMovieValue))
                    {
                        return true;
                    }
                }

            }

            // If all properties are equal, return false
            return false;
        }

        private List<MovieViewModel> BuildMovieViewModel(List<Movie> movies)
        {
            var results = new List<MovieViewModel>();
            var lockObject = new object();
            var keyValuePairs = new List<KeyValuePair<Movie, bool>>();
            foreach (var m in movies)
            {
                keyValuePairs.Add(new KeyValuePair<Movie, bool>(m, false));
            }
            var taskArray = new Task[4];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < keyValuePairs.Count; j++)
                    {
                        lock (lockObject)
                        {
                            if (!keyValuePairs[j].Value)
                            {
                                var newKvp = new KeyValuePair<Movie, bool>(keyValuePairs[j].Key, true);
                                keyValuePairs[j] = newKvp;
                                var movie = keyValuePairs[j].Key;
                               
                                var movieLocations = movie.MovieLocation.Split('|');
                                var movieClips = new List<MovieViewModel>();
                                for (int k = 0; k < movieLocations.Length; k++)
                                {
                                    if (!string.IsNullOrEmpty(movieLocations[k]))
                                    {
                                        var title = string.Empty;
                                        if (k == 0)
                                        {
                                            title = movie.Title;
                                        }
                                        else
                                        {
                                            title = $"{movie.Title}-cd{k}";
                                        }
                                        movieClips.Add(new MovieViewModel()
                                        {
                                            ImdbId = movie.ImdbId,
                                            Title = title,
                                            // Commented for deprecating http-server.
                                            //PosterFileLocation = AppStaticMethods.GetDiskPort(movie.PosterFileLocation?.Substring(0, 1)) + movie.PosterFileLocation?.Remove(0, 3),
                                            //FanArtLocation = AppStaticMethods.GetDiskPort(movie.PosterFileLocation?.Substring(0, 1)) + movie.FanArtLocation?.Remove(0, 3),
                                            PosterFileLocation = movie.PosterFileLocation,
                                            FanArtLocation = movie.FanArtLocation,
                                            MovieLocation = movieLocations[k],
                                            DateAdded = movie.DateAdded,
                                            Director = movie.Director
                                        });
                                    }
                                }
                                results.AddRange(movieClips);
                            }
                        }
                    }
                });
            }

            Task.WaitAll(taskArray);
            return results;
        }

        private async Task InsertActors(DatabaseContext context, List<string> actors, bool scrapeActorInfo)
        {
            var allActors = context.Actors.Select(x => x.Name).ToHashSet();

            foreach(var actor in actors)
            {
                if (!allActors.Contains(actor))
                {
                    context.Actors.Add(new Actor()
                    {
                        Name = actor
                    });
                }
            }
            if(scrapeActorInfo)
            {
                _scrapeService.GetActorInformation();
            }
            await context.SaveChangesAsync();
        }

        private async Task InsertGenres(DatabaseContext context, List<string> genres)
        {
            var allGenres = context.Genres.Select(x => x.Name).ToHashSet();

            foreach (var genre in genres)
            {
                if (!allGenres.Contains(genre))
                {
                    context.Genres.Add(new Genre()
                    {
                        Name = genre
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        private async Task InsertTags(DatabaseContext context, List<string> tags)
        {
            var allTags = context.Tags.Select(x => x.Name).ToHashSet();

            foreach (var tag in tags)
            {
                if (!allTags.Contains(tag))
                {
                    context.Tags.Add(new Tag()
                    {
                        Name = tag
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        private void InsertMovie(DatabaseContext context, Movie movie)
        {
            context.Movies.Add(movie);
            InsertForeignKeys(context, movie);
        }

        private void InsertForeignKeys(DatabaseContext context, Movie movie)
        {
            foreach (var actor in movie.Actors)
            {
                context.MovieActors.Add(new MovieActors()
                {
                    ImdbId = movie.ImdbId,
                    ActorName = actor
                });
            }
            foreach (var genre in movie.Genres)
            {
                context.MovieGenres.Add(new MovieGenres()
                {
                    ImdbId = movie.ImdbId,
                    GenreName = genre
                });
            }
            foreach (var tag in movie.Tags)
            {
                context.MovieTags.Add(new MovieTags()
                {
                    ImdbId = movie.ImdbId,
                    TagName = tag
                });
            }
        }

        private void UpdateMovie(DatabaseContext context, Movie movie, Movie exisitingMovie)
        {
            exisitingMovie.ImdbId = movie.ImdbId;
            exisitingMovie.Title = movie.Title;
            exisitingMovie.Plot = movie.Plot;
            exisitingMovie.Year = movie.Year;
            exisitingMovie.Runtime = movie.Runtime;
            exisitingMovie.Studio = movie.Studio;
            exisitingMovie.PosterFileLocation = movie.PosterFileLocation;
            exisitingMovie.FanArtLocation = movie.FanArtLocation;
            exisitingMovie.MovieLocation = movie.MovieLocation;
            exisitingMovie.DateAdded = movie.DateAdded;
            exisitingMovie.ReleaseDate = movie.ReleaseDate;
            exisitingMovie.Genres = movie.Genres;
            exisitingMovie.Tags = movie.Tags;
            exisitingMovie.Actors = movie.Actors;
            exisitingMovie.Director = movie.Director;

            DeleteForeignKeys(context, movie);
            InsertForeignKeys(context, movie);
        }

        private void DeleteMovies(List<Movie> moviesToRemove)
        {
            using(var context = new DatabaseContext())
            {
                foreach (var movie in moviesToRemove)
                {
                    var m = context.Movies.Where(x => x.ImdbId == movie.ImdbId).FirstOrDefault();
                    DeleteForeignKeys(context, m);
                    //DeleteFromPlayList(context, m);
                    context.Movies.Remove(m);
                    context.SaveChanges();
                }
            }
        }

        private void DeleteForeignKeys(DatabaseContext context, Movie movie)
        {
            var existingMovieActors = context.MovieActors.Where(x => x.ImdbId == movie.ImdbId).ToList();
            var exsitingMovieGenres = context.MovieGenres.Where(x => x.ImdbId == movie.ImdbId).ToList();
            var exsitingMovieTags = context.MovieTags.Where(x => x.ImdbId == movie.ImdbId).ToList();

            context.MovieActors.RemoveRange(existingMovieActors);
            context.MovieGenres.RemoveRange(exsitingMovieGenres);
            context.MovieTags.RemoveRange(exsitingMovieTags);

            context.SaveChanges();
        }

        private void DeleteFromPlayList(DatabaseContext context, Movie movie)
        {
            var playListMovie = context.PlayLists.Where(x => x.ImdbId == movie.ImdbId).ToList();
            context.PlayLists.RemoveRange(playListMovie);
            context.SaveChanges();
        }
    }
}
