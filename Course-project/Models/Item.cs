using System;

namespace Course_project.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public string CollectionId { get; set; }

        public string NameNumField1 { get; set; }
        public string NameNumField2 { get; set; }
        public string NameNumField3 { get; set; }
        public double NumField1 { get; set; }
        public double NumField2 { get; set; }
        public double NumField3 { get; set; }

        public string NameStrField1 { get; set; }
        public string NameStrField2 { get; set; }
        public string NameStrField3 { get; set; }
        public string StrField1 { get; set; }
        public string StrField2 { get; set; }
        public string StrField3 { get; set; }

        public string NameTextField1 { get; set; }
        public string NameTextField2 { get; set; }
        public string NameTextField3 { get; set; }
        public string TextField1 { get; set; }
        public string TextField2 { get; set; }
        public string TextField3 { get; set; }

        public string NameDateField1 { get; set; }
        public string NameDateField2 { get; set; }
        public string NameDateField3 { get; set; }
        public DateTime DateField1 { get; set; }
        public DateTime DateField2 { get; set; }
        public DateTime DateField3 { get; set; }

        public string NameCheckField1 { get; set; }
        public string NameCheckField2 { get; set; }
        public string NameCheckField3 { get; set; }
        public bool CheckField1 { get; set; }
        public bool CheckField2 { get; set; }
        public bool CheckField3 { get; set; }

    }
}
