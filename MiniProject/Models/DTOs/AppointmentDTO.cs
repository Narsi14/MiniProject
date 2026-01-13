using System;
using System.ComponentModel.DataAnnotations;
using MiniProject.Models;

namespace MiniProject.Models.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        [FutureDate]
        public DateTime AppointmentDate { get; set; }
        
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Reason must contain only letters and spaces")]
        public string? Reason { get; set; }
        public string? Status { get; set; }
    }
}
