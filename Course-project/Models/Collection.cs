using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int CountItems { get; set; }

        [NotMapped]
        public IFormFile Img { get; set; }
        public string ImageStorageName { get; set; }
        public string UrlImg { get; set; }
    }
}
