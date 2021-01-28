using System;
using System.Collections.Generic;

namespace Course_project.Models
{
    public class Collection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Summary { get; set; }
        public string Theme { get; set; }
        public string Owner { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
