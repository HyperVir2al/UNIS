using System;
using System.Collections.Generic;
using System.Text;

namespace Universities.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;   
        public int Year { get; set; }                       
        public int MaxStudents { get; set; }                 
        public string Specialty { get; set; } = string.Empty;

        
        public int UniId { get; set; }
        public University? University { get; set; }

        
        public List<Student> Students { get; set; } = new();
    }
}