using MovieManager.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using Serilog;

namespace MovieManager.BusinessLogic
{
    public class XmlProcessor
    {
        public Movie ParseXmlFile(string xmlFileLocation)
        {
            Movie movie = null;
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFileLocation);
                var title = xmlDoc.GetElementsByTagName("title")[0]?.InnerText;
                var plot = xmlDoc.GetElementsByTagName("plot")[0]?.InnerText;
                var year = int.Parse(!String.IsNullOrEmpty(xmlDoc.GetElementsByTagName("year")[0]?.InnerText) ?
                    xmlDoc.GetElementsByTagName("year")[0]?.InnerText : DateTime.Now.Year.ToString());
                var runtime = int.Parse(!String.IsNullOrEmpty(xmlDoc.GetElementsByTagName("runtime")[0]?.InnerText) ?
                    xmlDoc.GetElementsByTagName("runtime")[0]?.InnerText : "0");
                var studio = xmlDoc.GetElementsByTagName("studio")[0]?.InnerText;
                var releaseDate = xmlDoc.GetElementsByTagName("release")[0]?.InnerText;
                var director = xmlDoc.GetElementsByTagName("director")[0]?.InnerText;
                var genres = GetGenres(xmlDoc.GetElementsByTagName("genre"));
                var tags = GetTags(xmlDoc.GetElementsByTagName("tag"));
                var actors = GetActors(xmlDoc.GetElementsByTagName("actor"));
                var label = xmlDoc.GetElementsByTagName("label")[0]?.InnerText;

                if (!string.IsNullOrEmpty(label))
                {
                    genres.Add(label);
                    tags.Add(label);
                }
                    
                movie = new Movie()
                {
                    Title = title,
                    Plot = plot,
                    Year = year,
                    Runtime = runtime,
                    Director = director,
                    Studio = studio,
                    ReleaseDate = releaseDate,
                    Genres = genres,
                    Tags = tags,
                    Actors = actors
                };
            }
            catch(Exception ex)
            {
                Log.Error($"An error occurs when processing xml: {xmlFileLocation}. \n\r");
                Log.Error(ex.ToString());
            }
            return movie;
        }

        private List<string> GetGenres(XmlNodeList rawGenres)
        {
            var genres = new List<string>();
            foreach(var rawGenre in rawGenres)
            {
                var genre = ((XmlNode)rawGenre).InnerText.Trim();
                if (!string.IsNullOrEmpty(genre))
                {
                    genres.Add(((XmlNode)rawGenre).InnerText);
                }
            }
            return genres;
        }

        private List<string> GetTags(XmlNodeList rawTags)
        {
            var tags = new List<string>();
            foreach (var rawTag in rawTags)
            {
                var tag = ((XmlNode)rawTag).InnerText.Trim();
                if(!string.IsNullOrEmpty(tag))
                {
                    tags.Add(tag);
                }
            }
            return tags;
        }

        private List<string> GetActors(XmlNodeList rawActors)
        {
            var actors = new List<string>();
            foreach (XmlNode rawActor in rawActors)
            {
                foreach(XmlNode n in rawActor.SelectNodes("name"))
                {
                    var actor = n.InnerText.Trim();
                    if(!string.IsNullOrEmpty(actor))
                    {
                        actors.Add(actor);
                    }
                }
            }
            return actors;
        }
    }
}
