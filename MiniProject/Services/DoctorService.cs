using Microsoft.EntityFrameworkCore;
using MiniProject.Data;
using MiniProject.Models;
using MiniProject.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;

        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorDTO>> GetAllAsync()
        {
            return await _context.Doctors
                .Select(d => new DoctorDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Specialization = d.Specialization,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<DoctorDTO?> GetByIdAsync(int id)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
            if (doctor == null) return null;

            return new DoctorDTO
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber
            };
        }
    }
}
