using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniProject.Models.Validation;

namespace MiniProject.Models
{
    [Table("Patient", Schema = "Healthcare")]
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Date of birth must be a past date.")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "Phone must contain only digits (7-15 digits).")]
        public string? Phone { get; set; }

        // Navigation Property
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
