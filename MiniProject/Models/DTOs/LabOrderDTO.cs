using System;
using System.ComponentModel.DataAnnotations;

namespace MiniProject.Models.DTOs
{
    public class LabOrderDTO
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string? TestName { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }
        public string? Result { get; set; }
        public string? PatientName { get; set; }
    }
}
