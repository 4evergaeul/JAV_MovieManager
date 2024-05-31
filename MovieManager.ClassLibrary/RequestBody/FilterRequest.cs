using System.Collections.Generic;

namespace MovieManager.ClassLibrary
{
    public enum FilterType { Actors, Genres, Tags, Directors, Years }

    public class FilterRequest
    {
        public FilterType FilterType { get; set; }
        public List<string> Filters { get; set; }
        public bool IsAndOperator { get; set; }
    }
}
