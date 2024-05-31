using System.ComponentModel.DataAnnotations;

namespace MovieManager.ClassLibrary
{
    public class ActorTagMapping
    {
        [Key]
        public int Id { get; set; }
        public string ActorName { get; set; }
        public string TagName { get; set; }
    }
}
