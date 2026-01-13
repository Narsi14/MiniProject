using MiniProject.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Services
{
    public interface IPatientService
    {
        Task<List<PatientDTO>> GetAllAsync();
        Task<PatientDTO?> GetByIdAsync(int id);
        Task InitialCreateAsync(PatientDTO patientDTO);
        Task UpdateAsync(PatientDTO patientDTO);
        Task DeleteAsync(int id);
    }
}
