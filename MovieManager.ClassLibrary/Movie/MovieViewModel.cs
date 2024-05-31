using System.Collections.Generic;

namespace MovieManager.ClassLibrary
{
    public class MovieViewModel
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string PosterFileLocation { get; set; }
        public string FanArtLocation { get; set; }
        public string MovieLocation { get; set; }
        public string DateAdded { get; set; }
    }
}
