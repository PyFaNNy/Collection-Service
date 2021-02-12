using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Models
{
    public class Like
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string ItemId { get; set; }
    }
}
