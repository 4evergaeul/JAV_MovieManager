using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieManager.ClassLibrary
{
    public class Actor
    {
        [Key]
        public string Name { get; set; }
        public string DateofBirth { get; set; }
        public string Height { get; set; }
        public string Cup { get; set; }
        public string Bust { get; set; }
        public string Waist { get; set; }
        public string Hips { get; set; }
        public string Looks { get; set; }
        public string Body { get; set; }
        public string SexAppeal { get; set; }
        public string Overall { get; set; }
        [NotMapped]
        public List<string> Tags { get; set; }
        public string LastUpdated { get; set; }
        public bool Liked { get; set; }
    }
}
