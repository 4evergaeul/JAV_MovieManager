using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.ClassLibrary
{
    public class MovieDetails
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Plot { get; set; }
        public int Year { get; set; }
        public int Runtime { get; set; }
        public string Studio { get; set; }
        public string PosterFileLocation { get; set; }
        public string FanArtLocation { get; set; }
        public string MovieLocation { get; set; }
        public int PlayedCount { get; set; }
        public string DateAdded { get; set; }
        public string ReleaseDate { get; set; }
        public bool Liked { get; set; }
        public List<GenreViewModel> Genres { get; set; }
        public List<TagViewModel> Tags { get; set; }
        public List<ActorViewModel> Actors { get; set; }
    }
}
