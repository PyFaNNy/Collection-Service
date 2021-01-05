using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.Models
{
    public class User :IdentityUser<Guid>
    {
        public string Status { get; set; }
    }
}
