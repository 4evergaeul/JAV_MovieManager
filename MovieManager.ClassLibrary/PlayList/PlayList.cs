using System.ComponentModel.DataAnnotations;

namespace MovieManager.ClassLibrary
{
    public class PlayList
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Liked { get; set; }
        public string ImdbId { get; set; }
    }
}
