using System.Collections.Generic;
using System.Globalization;

namespace Course_project.Models
{
    public class CultureSwitcher
    {
        public CultureInfo CurrentUICulture { get; set; }
        public List<CultureInfo> SupportedCultures { get; set; }
    }
}
