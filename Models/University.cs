using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Universities.Models
{
    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;   
        public string Address { get; set; } = string.Empty; 

        public List<Group> Group { get; set; } = new();
    }
}