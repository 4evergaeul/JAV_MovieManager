using System.ComponentModel.DataAnnotations;

namespace MovieManager.ClassLibrary
{
    public class MovieGenres
    {
        [Key]
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string GenreName { get; set; }
    }
}
