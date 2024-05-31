using System.ComponentModel.DataAnnotations;

namespace MovieManager.ClassLibrary
{
    public class Tag
    {
        [Key]
        public string Name { get; set; }
        public bool Liked { get; set; }
    }
}
