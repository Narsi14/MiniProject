using MiniProject.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDTO>> GetAllAsync();
        Task<AppointmentDTO?> GetByIdAsync(int id);
        Task<int> CreateAsync(AppointmentDTO appointmentDTO); // Returns new ID
        Task UpdateAsync(AppointmentDTO appointmentDTO);
        Task DeleteAsync(int id);
        Task<List<AppointmentDTO>> GetByPatientIdAsync(int patientId);
    }
}
