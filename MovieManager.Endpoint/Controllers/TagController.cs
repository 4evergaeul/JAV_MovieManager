using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieManager.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManager.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private string notFoundMessage = "No tags found!";
        private string badRequestMessage = "Value cannot be null!";
        private TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        [Route("/tags")]
        public ActionResult GetAll()
        {
            var tags = _tagService.GetAll();
            if (tags.Count > 0)
            {
                return Ok(tags);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/tags/{searchString}")]
        public ActionResult Get(string searchString)
        {
            var tags = _tagService.Get(searchString);
            if (tags.Count > 0)
            {
                return Ok(tags);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/tags/names")]
        public ActionResult GetAllNames()
        {
            var tags = _tagService.GetAllNames();
            if (tags.Count > 0)
            {
                return Ok(tags);
            }
            return NotFound(notFoundMessage);
        }

        [HttpGet]
        [Route("/tags/like")]
        public ActionResult GetLikedTags()
        {
            var tags = _tagService.GetLikedTags();
            if (tags.Count > 0)
            {
                return Ok(tags);
            }
            return NotFound(notFoundMessage);
        }

        [HttpPut]
        [Route("/tags/like/{tagName}")]
        public ActionResult LikeActor(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return BadRequest(badRequestMessage);
            }
            return Ok(_tagService.LikeTag(tagName));
        }
    }
}
