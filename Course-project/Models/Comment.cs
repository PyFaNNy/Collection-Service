 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ItemId { get; set; }
        public string messenge { get; set; }
        public string UrlImg { get; set; }
    }
}
