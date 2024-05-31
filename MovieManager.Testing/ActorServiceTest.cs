using MovieManager.BusinessLogic;
using Serilog;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MovieManager.Testing
{
    public class ActorServiceTest
    {
        public ActorServiceTest()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/movieSrv-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt")
                .CreateLogger();
        }

        [Fact]
        public void ActorService_GetAll()
        {
            var actorSrv = new ActorService();
            var actors = actorSrv.GetAll();
            actors.Count.ShouldNotBe(0);
        }
    }
}
