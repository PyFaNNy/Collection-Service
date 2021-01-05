using System.ComponentModel.DataAnnotations;

namespace Course_project.ViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Name { get; set; }

        public string ReturnUrl { get; set; }
    }
}
