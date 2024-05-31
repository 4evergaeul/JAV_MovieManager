using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieManager.ClassLibrary;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MovieManager.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserSettingsController : ControllerBase
    {
        private string badRequestMessage = "Value cannot be null!";
        private string notFoundMessage = "No User Settings found!";
        private IOptions<UserSettings> _config;

        public UserSettingsController(IOptions<UserSettings> config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("/usersettings")]
        public ActionResult Get()
        {
            if(_config.Value == null)
            {
                return NotFound(notFoundMessage);
            }
            UserSettings result = _config.Value;
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
            var f = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var json = System.IO.File.ReadAllText(f);
            dynamic oldUserSettings = JsonConvert.DeserializeObject(json);
            oldUserSettings.UserSettings.MovieDirectory = userSettings.MovieDirectory;
            string output = JsonConvert.SerializeObject(oldUserSettings, Formatting.Indented);
            System.IO.File.WriteAllText(f, output);
            return Ok(userSettings);
        }
    }
}
