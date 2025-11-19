using System;
using System.ComponentModel.DataAnnotations;

namespace ST10359128_PROG6212_POE_Part1.Models
{
    public class Claim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lecturer Name is required")]
        public string LecturerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hours Worked is required")]
        [Range(1, 200, ErrorMessage = "Hours must be greater than 0")]
        public int HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly Rate is required")]
        [Range(1, 2000, ErrorMessage = "Hourly rate must be greater than 0")]
        public decimal HourlyRate { get; set; }

        public decimal Amount => HoursWorked * HourlyRate;

        public string Status { get; set; } = "Pending";

        public string FileName { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;

        public string Notes { get; set; } = string.Empty;
    }
}
