using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Course_project.Models
{
    public class User :IdentityUser<Guid>
    {
        public string Status { get; set; }
        public virtual List<Collection> Collections { get; set; }
    }
}
