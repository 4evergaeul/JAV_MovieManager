using System.ComponentModel.DataAnnotations;

namespace MovieManager.ClassLibrary
{
    public class MovieTags
    {
        [Key]
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string TagName { get; set; }
    }
}
