using Xunit;
using Shouldly;
using MovieManager.BusinessLogic;
using System;
using Serilog;

namespace MovieManager.Testing
{
    public class ScrapeServiceTest
    {
        public ScrapeServiceTest()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File($"logs/movieSrv-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt")
                            .CreateLogger();
        }

        [Fact]
        public void ScrapeActorTest()
        {
            var scrpeService = new ScrapeService();
            scrpeService.GetActorInformation();
        }
    }
}
