using Microsoft.VisualBasic.FileIO;
using MovieManager.BusinessLogic;
using MovieManager.ClassLibrary;
using MovieManager.Data;
using Serilog;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace MovieManager.Testing
{
    //public class MovieServiceTest
    //{
    //    public MovieServiceTest()
    //    {
    //        Log.Logger = new LoggerConfiguration()
    //            .MinimumLevel.Debug()
    //            .WriteTo.Console()
    //            .WriteTo.File($"logs/movieSrv-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt")
    //            .CreateLogger();
    //    }

    //    [Fact]
    //    public async void MovieServiceProcess_All()
    //    {
    //        var fileLocation = @"E:\MyFile\New\有码";
    //        var xmlEngine = new XmlProcessor();
    //        var fileScanner = new FileScanner(xmlEngine);
    //        var scrapeService = new ScrapeService();
    //        var movieSvc = new MovieService(scrapeService);
    //        var movies = fileScanner.ScanFiles(fileLocation);
    //        await movieSvc.InsertMovies(movies);
    //    }

    //    [Fact]
    //    public async void MovieServiceProcess()
    //    {
    //        var fileLocations = new List<string>() { @"E:\MyFile\New\有码", @"F:\Library\多P" };
    //        var xmlEngine = new XmlProcessor();
    //        var fileScanner = new FileScanner(xmlEngine);
    //        var scrapeService = new ScrapeService();
    //        var movieSvc = new MovieService(scrapeService);
    //        foreach (var f in fileLocations)
    //        {
    //            var movies = fileScanner.ScanFiles(f, DateTime.Parse("1/1/2010"));
    //            await movieSvc.InsertMovies(movies, false, false);
    //        }
    //    }

    //    [Fact]
    //    public void GetMovies()
    //    {
    //        var scrapeService = new ScrapeService();
    //        var movieSvc = new MovieService(scrapeService);
    //        var movies = movieSvc.GetMovies();
    //        movies.Count().ShouldNotBe(0);
    //    }

    //    [Fact]
    //    public void GetMoviesByActors()
    //    {
    //        var scrapeService = new ScrapeService();
    //        var movieSvc = new MovieService(scrapeService);
    //        var actors = new List<string>() { "" };
    //        var movies = movieSvc.GetMoviesByFilters(FilterType.Actors, actors, false);
    //        movies.Count().ShouldNotBe(0);
    //    }

    //    [Fact]
    //    public void GetMoviesByImdbId()
    //    {
    //        var scrapeService = new ScrapeService();
    //        var movieSvc = new MovieService(scrapeService);
    //        var imdbId = "IPX";
    //        var movies = movieSvc.GetMoviesWildcard(imdbId);
    //        movies.Count().ShouldNotBe(0);
    //    }

    //    //[Fact]
    //    //public void RenameTest()
    //    //{
    //    //    using (var context = new DatabaseContext())
    //    //    {
    //    //        var movieRenames = context.Movies.Select(x => new MovieRename()
    //    //        {
    //    //            ImdbId = x.ImdbId,
    //    //            Title = x.Title,
    //    //            MovieLocations = x.MovieLocation
    //    //        }).ToList();
    //    //        foreach (var m in movieRenames)
    //    //        {
    //    //            m.Filename = Path.GetFileNameWithoutExtension(m.MovieLocations.Split('|')[0]);
    //    //            m.MovieLocations = m.MovieLocations.Split('|')[0];
    //    //        }
    //    //        var notMatchedFilenames = movieRenames.Where(x => x.Filename.Length < 15).ToList();
    //    //        foreach (var file in notMatchedFilenames)
    //    //        {
    //    //            var ext = Path.GetExtension(file.MovieLocations);
    //    //            FileSystem.RenameFile(file.MovieLocations, $"{file.Title}{ext}");
    //    //        }

    //    //        //var movies = context.Movies.Where(x => Path.GetFileNameWithoutExtension(x.MovieLocation.Split('|')[0]) != x.ImdbId).ToList();
    //    //    }
    //    //}

    //    //public class MovieRename
    //    //{
    //    //    public string ImdbId { get; set; }
    //    //    public string Title { get; set; }
    //    //    public string Filename { get; set; }
    //    //    public string MovieLocations { get; set; }
    //    //}
    //}
}
