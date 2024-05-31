using MovieManager.BusinessLogic;
using MovieManager.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MovieManager.Testing
{
    public class PotPlayerServiceTest
    {
        [Fact]
        public void BuildPotPlayerPlayListTest()
        {
            //var scrapeService = new ScrapeService();
            //var movieSrv = new MovieService(scrapeService);
            //var potplayerSrv = new PotPlayerService(movieSrv);
            //var searchList = new List<string>() { "" };
            //var movieLocations = movieSrv.GetMoviesByFilters(FilterType.Actors, searchList, false).Select(x => x.MovieLocation).ToList();
            //var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Roaming\\PotPlayerMini64\\Playlist\\";
            //potplayerSrv.BuildPlayList("Test", path, movieLocations);
        }
    }
}
