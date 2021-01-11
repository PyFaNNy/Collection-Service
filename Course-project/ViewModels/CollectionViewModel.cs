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


        [Required]
        [Display(Name = "Img")]
        public string Img { get; set; }

    }
}
