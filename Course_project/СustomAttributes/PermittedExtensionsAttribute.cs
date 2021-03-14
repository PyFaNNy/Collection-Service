using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Course_project.СustomAttributes
{
    public class PermittedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] permittedExtensions;
        public PermittedExtensionsAttribute(string[] permittedExtensions)
        {
            this.permittedExtensions = permittedExtensions;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (!(file == null))
            {
                var extension = Path.GetExtension(file.FileName);
                if (!permittedExtensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Files of this type are not allowed to be added. (Use formats .jpg, .png, .gif, .jpeg)";
        }
    }
}
