using System;

namespace Course_project.Models
{
    public class Messenge
    {
        public Guid Id { get; set; }
        public string Sender { get; set; }
        public string messenge { get; set; }
        public string UrlImg { get; set; }
        public DateTime Time { get; set; }
    }
}
