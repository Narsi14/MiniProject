using MiniProject.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Services
{
    public interface ILabOrderService
    {
        Task<List<LabOrderDTO>> GetAllAsync();
        Task<LabOrderDTO?> GetByIdAsync(int id);
        Task CreateAsync(LabOrderDTO labOrderDTO);
        Task UpdateAsync(LabOrderDTO labOrderDTO);
        Task DeleteAsync(int id);
        Task CompleteAsync(int id);
        Task<List<LabOrderDTO>> GetByAppointmentIdAsync(int appointmentId);
    }
}
