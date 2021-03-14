using System;

namespace Course_project.Models
{
    public class RecentActivity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Messenge { get; set; }
        public DateTime Time { get; set; }
    }
}
