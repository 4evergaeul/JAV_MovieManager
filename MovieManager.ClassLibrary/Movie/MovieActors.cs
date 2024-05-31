using System.ComponentModel.DataAnnotations;

namespace MovieManager.ClassLibrary
{
    public class MovieActors
    {
        [Key]
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string ActorName { get; set; }
    }
}
