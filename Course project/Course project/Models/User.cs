using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Course_project.Models
{
    public class User :IdentityUser
    {
        public string Status { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string About { get; set; }
        public string Hobbies { get; set; }

        [NotMapped]
        public IFormFile Img { get; set; }
        public string ImageStorageName { get; set; }
        public string UrlImg { get; set; }
    }
}
