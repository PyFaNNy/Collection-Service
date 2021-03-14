using Course_project.СustomAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Course_project.ViewModels
{
    public class CollectionViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Theme")]
        public string Theme { get; set; }

        [Required]
        [Display(Name = "Summary")]
        public string Summary { get; set; }


        [Display(Name = "Img")]
        [MaxFileSize(1 * 1024 * 1024)]
        [PermittedExtensions(new string[] { ".jpg", ".png", ".gif", ".jpeg" })]
        public IFormFile Img { get; set; }

    }
}
