using System.ComponentModel.DataAnnotations;

namespace Course_project.ViewModels
{
    public class ItemViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Img")]
        public string Img { get; set; }
    }
}
