﻿using MovieManager.ClassLibrary;
using MovieManager.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MovieManager.BusinessLogic
{
    public class PotPlayerService
    {
        private MovieService _movieService;

        public PotPlayerService(MovieService movieService)
        {
            _movieService = movieService;
        }

        public void BuildPlayList(string playListName, string path, List<PlayListItem> movies, FileMode fileMode = FileMode.Create)
        {
            if (playListName.Contains("undefined"))
            { 
                playListName = playListName.Replace("undefined", $"{DateTime.Now.ToString("yyyyMMdd_hhmmss")}");
            }
            try
            {
                var movieLocations = movies.Select(x => x.MovieLocation.Split("|").Where(x => !string.IsNullOrEmpty(x)).ToList()).ToList();
                var imdbIds = movies.Select(x => x.ImdbId).ToList();
                var fs = new FileStream($"{path}\\{playListName.Replace(":", "-")}.dpl", fileMode);
                using(var writer = new StreamWriter(fs))
                {
                    if(fileMode == FileMode.Create)
                    {
                        var defaultInput = "DAUMPLAYLIST\nplaytime=0\ntopindex=0\nfoldertype=2\nsaveplaypos=0\n";
                        writer.WriteLine(defaultInput);
                    }
                    var count = 1;
                    for (int i = 0; i < movieLocations.Count; i++)
                    {                        
                        foreach (var movieLocation in movieLocations[i]) 
                        {
                            var l = $"{count++}*file*{movieLocation}";
                            writer.WriteLine(l);
                        }
                    }
                }
                using(var context = new DatabaseContext())
                {
                    foreach(var imdbId in imdbIds)
                    {
                        var movie = context.Movies.Where(x => x.ImdbId == imdbId).FirstOrDefault();
                        if(movie != null)
                        {
                            movie.PlayedCount += 1;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error($"An error occurs when creating potplayer list. \n\r");
                Log.Error(ex.ToString());
            }
        }

        public void BuildPlayListByActors(string playListName, string path, List<string> actors, FileMode fileMode = FileMode.Create)
        {
            try
            {
                var imdbIds = new HashSet<string>();
                var movieLocations = new List<string>();
                foreach(var actor in actors)
                {
                    var movies = _movieService.GetMoviesByFilters(FilterType.Actors, new List<string> { actor }, false).ToList();
                    foreach (var m in movies)
                    {
                        var imdbId = m.ImdbId;
                        var currentMovies = m.MovieLocation.Split("|").Where(x => !string.IsNullOrEmpty(x)).ToList();
                        if (!string.IsNullOrEmpty(imdbId) && !imdbIds.Contains(imdbId) && currentMovies.Count > 0)
                        {
                            imdbIds.Add(imdbId);
                            movieLocations.AddRange(currentMovies);
                        }
                    }
                }
                
                var fs = new FileStream($"{path}\\{playListName.Replace(":", "-")}.dpl", fileMode);
                using (var writer = new StreamWriter(fs))
                {
                    if (fileMode == FileMode.Create)
                    {
                        var defaultInput = "DAUMPLAYLIST\nplaytime=0\ntopindex=0\nfoldertype=2\nsaveplaypos=0\n";
                        writer.WriteLine(defaultInput);
                    }
                    for (int i = 0; i < movieLocations.Count; i++)
                    {
                        writer.WriteLine($"{i + 1}*file*{movieLocations[i]}");
                    }
                }
                using (var context = new DatabaseContext())
                {
                    foreach (var imdbId in imdbIds)
                    {
                        var movie = context.Movies.Where(x => x.ImdbId == imdbId).FirstOrDefault();
                        if (movie != null)
                        {
                            movie.PlayedCount += 1;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurs when creating potplayer list. \n\r");
                Log.Error(ex.ToString());
            }
        }
    }
}
