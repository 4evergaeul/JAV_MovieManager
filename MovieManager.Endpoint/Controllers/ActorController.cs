using Microsoft.AspNetCore.Mvc;
using MovieManager.BusinessLogic;
using MovieManager.ClassLibrary;
using System.Collections.Generic;

namespace MovieManager.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private string notFoundMessage = "No actors found!";
        private string badRequestMessage = "Value cannot be null!";
        private ActorService _actorService;

        public ActorController(ActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        [Route("/actors/all")]
        public ActionResult GetAll()
        {
            var actors = _actorService.GetAll();
            if (actors.Count > 0)
            {
                return Ok(actors);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/actors/names")]
        public ActionResult GetAllNames()
        {
           var actors = _actorService.GetAllNames();
            if (actors.Count > 0)
            {
                return Ok(actors);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/actors/{searchString}")]
        public ActionResult GetByName(string searchString)
        {
            var actors = _actorService.GetByName(searchString);
            if (actors.Count > 0)
            {
                return Ok(actors);
            }
            return NotFound(notFoundMessage);
        }

        [HttpPost]
        [Route("/actors/getbynames")]
        public ActionResult GetByNames([FromBody] List<string> names)
        {
            var actors = _actorService.GetByNames(names);
            if (actors.Count > 0)
            {
                return Ok(actors);
            }
            return NotFound(notFoundMessage);
        }

        [HttpPost]
        [Route("/actors/ranges")]
        public ActionResult GetNamesByRange([FromBody] ActorRangeRequest actorRangeRequest)
        {
            var actors = _actorService.GetNamesByRange(
                actorRangeRequest.HeightLower,
                actorRangeRequest.HeightUpper,
                actorRangeRequest.CupLower,
                actorRangeRequest.CupUpper,
                actorRangeRequest.Age);
            return Ok(actors);
        }

        [HttpGet]
        [Route("/actors/like")]
        public ActionResult GetLikedActorNames()
        {
            var actors = _actorService.GetLikedActorNames();
            if (actors.Count > 0)
            {
                return Ok(actors);
            }
            return NotFound(notFoundMessage);
        }

        [HttpPut]
        [Route("/actors/like/{actorName}")]
        public ActionResult LikeActor(string actorName)
        {
            if (string.IsNullOrEmpty(actorName))
            {
                return BadRequest(badRequestMessage);
            }
            return Ok(_actorService.LikeActor(actorName));
        }
    }
}
