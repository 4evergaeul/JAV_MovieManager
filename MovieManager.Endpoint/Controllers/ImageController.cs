using Microsoft.AspNetCore.Mvc;
using MovieManager.BusinessLogic;
using MovieManager.ClassLibrary.RequestBody;

namespace MovieManager.Endpoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : Controller
    {
        private string notFoundMessage = "No Image found!";
        private MovieService _movieService;
        private ActorService _actorService;

        public ImageController(MovieService movieService
            , ActorService actorService)
        {
            _movieService = movieService;
            _actorService = actorService;
        }

        [HttpPost] 
        [Route("/images/getimage")]
        public IActionResult GetImage([FromBody] ImageRequest imageRequest)
        {
            var path = "";
            if (imageRequest.ImageType < 10)
            {
                path = _movieService.GetImagePath(imageRequest);
            }
            else if (imageRequest.ImageType >= 10)
            {
                path = _actorService.GetImagePath(imageRequest);
            }
            if (string.IsNullOrEmpty(path))
            {
                return NotFound(notFoundMessage);
            }
            var image = System.IO.File.OpenRead(path);
            return File(image, "image/jpeg");
        }
    }
}
