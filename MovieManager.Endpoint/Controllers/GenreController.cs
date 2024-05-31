using Microsoft.AspNetCore.Mvc;
using MovieManager.BusinessLogic;

namespace MovieManager.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private string notFoundMessage = "No genres found!";
        private string badRequestMessage = "Value cannot be null!";
        private GenreService _genreService;

        public GenreController(GenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [Route("/genres")]
        public ActionResult GetAll()
        {
            var genres = _genreService.GetAll();
            if (genres.Count > 0)
            {
                return Ok(genres);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/genres/{searchString}")]
        public ActionResult Get(string searchString)
        {
            var genres = _genreService.Get(searchString);
            if (genres.Count > 0)
            {
                return Ok(genres);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/genres/names")]
        public ActionResult GetAllNames()
        {
            var genres = _genreService.GetAllNames();
            if (genres.Count > 0)
            {
                return Ok(genres);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/genres/like")]
        public ActionResult GetLikedGenres()
        {
            var genres = _genreService.GetLikedGenres();
            if (genres.Count > 0)
            {
                return Ok(genres);
            }
            return NotFound(notFoundMessage);
        }

        [HttpPut]
        [Route("/genres/like/{genreName}")]
        public ActionResult LikeActor(string genreName)
        {
            if (string.IsNullOrEmpty(genreName))
            {
                return BadRequest(badRequestMessage);
            }
            return Ok(_genreService.LikeGenre(genreName));
        }
    }
}
