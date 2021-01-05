using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course_project.ViewModels
{
    public class CreateUserViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Status { get; set; }
        public DateTime dateReg { get; set; }
        public DateTime dateLog { get; set; }
    }
}
