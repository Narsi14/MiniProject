using System;
using System.ComponentModel.DataAnnotations;
using MiniProject.Models.Validation;

namespace MiniProject.Models.DTOs
{
    public class PatientDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        public string? Name { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "Phone must contain only digits (7-15 digits).")]
        public string? Phone { get; set; }

        [Required]
        public string? Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Date of birth must be a past date.")]
        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }
    }
}
