using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieManager.BusinessLogic;
using MovieManager.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManager.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private string badRequestMessage = "Value cannot be null!";
        private string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Roaming\\PotPlayerMini64\\Playlist\\";
        private string potPlayerExe = @"C:\Program Files\DAUM\PotPlayer\PotPlayerMini64.exe";
        private PotPlayerService _potPlayerService;

        public PlayListController(PotPlayerService potPlayerService)
        {
            _potPlayerService = potPlayerService;
        }


        [HttpPost]
        [Route("/playlist/create/{playListName}")]
        public ActionResult CreatePlayList([FromBody] List<PlayListItem> movies, string playListName)
        {
            if (movies == null || movies.Count == 0)
            {
                return BadRequest(badRequestMessage);
            }
            try
            {
                _potPlayerService.BuildPlayList(playListName, path, movies);
                Process.Start(potPlayerExe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
            return Ok();
        }

        [HttpPost]
        [Route("/playlist/createbyactors/{playListName}")]
        public ActionResult CreatePlayListByActors([FromBody] List<string> actors, string playListName)
        {
            if (actors == null || actors.Count == 0)
            {
                return BadRequest(badRequestMessage);
            }
            try
            {
                _potPlayerService.BuildPlayListByActors(playListName, path, actors);
                Process.Start(potPlayerExe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
            return Ok();
        }

        [HttpPut]
        [Route("/playlist/append/default")]
        public ActionResult AppendToDefaultPlayList([FromBody] List<PlayListItem> movies)
        {
            if (movies == null || movies.Count == 0)
            {
                return BadRequest(badRequestMessage);
            }
            try
            {
                _potPlayerService.BuildPlayList("TempPlayList", path, movies, System.IO.FileMode.Append);
                Process.Start(potPlayerExe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
            return Ok();
        }
    }
}