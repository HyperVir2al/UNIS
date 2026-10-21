using System;
using System.Collections.Generic;
using System.Text;

namespace Universities.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }        

       
        public int GroupId { get; set; }
        public Group? Group { get; set; }
    }
}