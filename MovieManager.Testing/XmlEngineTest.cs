using Xunit;
using Shouldly;
using MovieManager.BusinessLogic;

namespace MovieManager.Testing
{
    public class XmlEngineTest
    {
        [Fact]
        public void ParseXmlTest()
        {
            var fileLocation = @"";
            var xmlEngine = new XmlProcessor();
            var movie = xmlEngine.ParseXmlFile(fileLocation);
            movie.ShouldNotBeNull();
        }
    }
}
