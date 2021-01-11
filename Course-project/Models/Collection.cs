using System.Collections.Generic;

namespace Course_project.Models
{
    public class Collection
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Theme { get; set; }
        public string UrlImg { get; set; }
        public string Owner { get; set; }
        public List<Collection> Items { get; set; }
    }
}
