using System;
using System.Collections.Generic;

namespace attendance.Entities
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string Employeeid { get; set; }
        public string Timein { get; set; }
        public string Timeout { get; set; }
        public string Date { get; set; }
        public string Dateout { get; set; }
    }
}
