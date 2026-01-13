using MiniProject.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Services
{
    public interface IDoctorService
    {
        Task<List<DoctorDTO>> GetAllAsync();
        Task<DoctorDTO?> GetByIdAsync(int id);
    }
}
