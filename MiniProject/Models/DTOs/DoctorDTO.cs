using System.ComponentModel.DataAnnotations;

namespace MiniProject.Models.DTOs
{
    public class DoctorDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Specialization { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
