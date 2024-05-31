using MovieManager.ClassLibrary;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieManager.BusinessLogic
{
    public class FileScanner
    {
        private XmlProcessor _xmlEngine;
        private readonly List<string> MovieExtensions = new List<string> { ".avi", ".mp4", ".wmv", ".mkv" };

        public FileScanner(XmlProcessor xmlEngine)
        {
            _xmlEngine = xmlEngine;
        }

        public List<Movie> ScanFiles(string rootDirectory, int dateRange = -1)
        {
            var movies = new List<Movie>();
            try
            {
                var nfos = new List<string>();  
                var allMovies = Directory.GetFiles(rootDirectory, $"*.*", SearchOption.AllDirectories)
                        .Where(f => MovieExtensions.Any(f.ToLower().EndsWith)).ToList();
                if (dateRange == -1)
                {
                    nfos = Directory.GetFiles(rootDirectory, "*.nfo", SearchOption.AllDirectories).ToList();
                }
                else
                {
                    nfos = Directory.GetFiles(rootDirectory, "*.nfo", SearchOption.AllDirectories)
                        .Where(f => File.GetCreationTime(f) >= DateTime.Now.AddDays(-dateRange)).ToList();
                }
                movies = ProcessNfos(nfos, allMovies);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when scanning files! \n\r");
                Log.Error(ex.ToString());
            }
            return movies;
        }

        public List<Movie> ScanFiles(string rootDirectory, DateTime startDate)
        {
            var movies = new List<Movie>();
            try
            {
                var nfos = new List<string>();  
                var allMovies = Directory.GetFiles(rootDirectory, $"*.*", SearchOption.AllDirectories)
                        .Where(f => MovieExtensions.Any(f.ToLower().EndsWith)).ToList();
                nfos = Directory.GetFiles(rootDirectory, "*.nfo", SearchOption.AllDirectories)
                    .Where(f => File.GetCreationTime(f) >= startDate).ToList();
                movies = ProcessNfos(nfos, allMovies);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when scanning files! \n\r");
                Log.Error(ex.ToString());
            }
            return movies;
        }

        public List<string> ScanFilesForImdbId(string rootDirectory)
        {
            var imdbIds = new List<string>();
            try
            {
                var nfos = new List<string>();
                var dirs = rootDirectory.Split(",");
                foreach (var dir in dirs)
                {
                    nfos = Directory.GetFiles(dir.Trim(), "*.nfo", SearchOption.AllDirectories).ToList();
                    foreach(var nfo in nfos)
                    {
                        try
                        {
                            var imdbId = Path.GetFileNameWithoutExtension(nfo).Split(' ')?[0];
                            if(!string.IsNullOrEmpty(imdbId))
                            {
                                imdbIds.Add(imdbId);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"An error occurs when scanning files for imdb ids! \n\r");
                            Log.Error(ex.ToString());
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when scanning files for imdb ids! \n\r");
                Log.Error(ex.ToString());
            }
            return imdbIds;
        }

        private List<Movie> ProcessNfos(List<string> nfos, List<string> allMovies)
        {
            var movies = new List<Movie>();
            var currentNfo = String.Empty;
            try 
            {
                foreach (var nfo in nfos)
                {
                    currentNfo = nfo;
                    var movie = _xmlEngine.ParseXmlFile(nfo);
                    if (movie != null)
                    {
                        var imdb = movie.Title.Split(' ')?[0];
                        if (!string.IsNullOrEmpty(imdb))
                        {
                            // Update movie locations
                            movie.ImdbId = imdb;
                            var movieFileLoc = allMovies.Where(x => x.Contains(imdb)).ToList();
                            if (movieFileLoc?.Count() > 0)
                            {
                                var sb = new StringBuilder();
                                foreach (var loc in movieFileLoc)
                                {
                                    sb.Append(loc + "|");
                                    movie.MovieLocation = sb.ToString();
                                }
                                // Update fanart/poster
                                var nfoFileName = Path.GetFileNameWithoutExtension(nfo);
                                var moviePath = Path.GetDirectoryName(movieFileLoc.FirstOrDefault());
                                var movieName = Path.GetFileNameWithoutExtension(movieFileLoc.FirstOrDefault());
                                var fanArtLoc = Directory.GetFiles(moviePath, $"{movieName}-fanart.jpg").FirstOrDefault();
                                if (string.IsNullOrEmpty(fanArtLoc))
                                {
                                    fanArtLoc = Directory.GetFiles(moviePath, $"{nfoFileName}-fanart.jpg").FirstOrDefault();
                                    if (string.IsNullOrEmpty(fanArtLoc))
                                    {
                                        fanArtLoc = Directory.GetFiles(moviePath, $"fanart.jpg").FirstOrDefault();
                                    }
                                }
                                var posterLoc = Directory.GetFiles(moviePath, $"{movieName}-poster.jpg").FirstOrDefault();
                                if (string.IsNullOrEmpty(posterLoc))
                                {
                                    posterLoc = Directory.GetFiles(moviePath, $"{nfoFileName}-poster.jpg").FirstOrDefault();
                                    if (string.IsNullOrEmpty(posterLoc))
                                    {
                                        posterLoc = Directory.GetFiles(moviePath, $"poster.jpg").FirstOrDefault();
                                    }
                                }
                                movie.FanArtLocation = fanArtLoc;
                                movie.PosterFileLocation = posterLoc;
                                // Update created date
                                movie.DateAdded = File.GetCreationTime(movieFileLoc.FirstOrDefault()).ToString("yyyy-MM-dd");

                                movies.Add(movie);
                            }
                            else
                            {
                                Log.Warning($"Skipped nfo files: {currentNfo} because the movie location is null.\n\r");
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error($"An error occurs when processing nfo files: {currentNfo} \n\r");
                Log.Error(ex.ToString());
            }
            return movies;
        }
    }
}
