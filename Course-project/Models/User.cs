using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Course_project.Models
{
    public class User :IdentityUser
    {
        public string Status { get; set; }
        public virtual List<Collection> Collections { get; set; } = new List<Collection>();
    }
}
