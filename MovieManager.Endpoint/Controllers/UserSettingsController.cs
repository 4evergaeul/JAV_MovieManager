using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieManager.BusinessLogic;
using MovieManager.ClassLibrary;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace MovieManager.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserSettingsController : ControllerBase
    {
        private string badRequestMessage = "Value cannot be null!";
        private string notFoundMessage = "No User Settings found!";
        private UserSettingsService _config;

        public UserSettingsController(UserSettingsService config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("/usersettings")]
        public ActionResult Get()
        {
            if(_config == null)
            {
                return NotFound(notFoundMessage);
            }
            UserSettings result = _config.GetUserSettings();
            return Ok(result);
        }

        [HttpPut]
        [Route("/usersettings/update")]
        public ActionResult Update([FromBody] UserSettings userSettings)
        {
            if(userSettings == null)
            {
                return BadRequest(badRequestMessage);
            }
            _config.SetUserSettings(userSettings);
            return Ok(userSettings);
        }
    }
}
