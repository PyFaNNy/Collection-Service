using System;

namespace Course_project.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string ItemId { get; set; }
        public string messenge { get; set; }
        public string UrlImg { get; set; }
    }
}
