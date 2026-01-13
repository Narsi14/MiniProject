using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniProject.Models
{
    [Table("Appointment", Schema = "Healthcare")]
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [StringLength(255)]
        public string? Reason { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";

        [StringLength(100)]
        public string? DoctorName { get; set; }

        public int? DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        // Navigation Property
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
    }
}
