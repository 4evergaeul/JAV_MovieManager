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
    public class DirectorController : Controller
    {
        private string notFoundMessage = "No directors found!";
        private string badRequestMessage = "Value cannot be null!";
        private DirectorService _directorService;

        public DirectorController(DirectorService directorService)
        {
            _directorService = directorService;
        }

        [HttpGet]
        [Route("/directors/names")]
        public ActionResult GetAllNames()
        {
            var directors = _directorService.GetUniqueDirectors();
            if (directors.Count > 0)
            {
                return Ok(directors);
            }
            return NotFound(notFoundMessage);
        }
    }
}
