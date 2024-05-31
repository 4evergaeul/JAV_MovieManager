using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieManager.ClassLibrary
{
    public class Movie
    {
        [Key]
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Plot { get; set; }
        public int Year { get; set; }
        public int Runtime { get; set; }
        public string Director { get; set; }
        public string Studio { get; set; }
        public string PosterFileLocation { get; set; }
        public string FanArtLocation { get; set; }
        public string MovieLocation { get; set; }
        public int PlayedCount { get; set; }
        public string DateAdded { get; set; }
        public string ReleaseDate { get; set; }
        public bool Liked { get; set; }
        [NotMapped]
        public List<string> Genres { get; set; }
        [NotMapped]
        public List<string> Tags { get; set; }
        [NotMapped]
        public List<string> Actors { get; set; }

    }
}
