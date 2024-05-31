using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieManager.BusinessLogic
{
    public class DirectorService
    {
        private MovieService _movieService;

        public DirectorService(MovieService movieService)
        {
            _movieService = movieService;
        }

        public List<String> GetUniqueDirectors()
        {
            return _movieService.GetMovies().Select(x => x.Director).Distinct().ToList();
        }
    }
}
