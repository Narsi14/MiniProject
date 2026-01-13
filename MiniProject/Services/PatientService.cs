using Microsoft.EntityFrameworkCore;
using MiniProject.Data;
using MiniProject.Models;
using MiniProject.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;

        public PatientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PatientDTO>> GetAllAsync()
        {
            return await _context.Patients
                .Select(p => new PatientDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    Phone = p.Phone,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    Age = DateTime.Now.Year - p.DateOfBirth.Year // heavily simplified age
                })
                .ToListAsync();
        }

        public async Task<PatientDTO?> GetByIdAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return null;

            return new PatientDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                Email = patient.Email,
                Phone = patient.Phone,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                Age = DateTime.Now.Year - patient.DateOfBirth.Year
            };
        }

        public async Task InitialCreateAsync(PatientDTO patientDTO)
        {
            var patient = new Patient
            {
                Name = patientDTO.Name,
                Email = patientDTO.Email,
                Phone = patientDTO.Phone,
                Gender = patientDTO.Gender,
                DateOfBirth = patientDTO.DateOfBirth
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PatientDTO patientDTO)
        {
            var patient = await _context.Patients.FindAsync(patientDTO.Id);
            if (patient != null)
            {
                patient.Name = patientDTO.Name;
                patient.Email = patientDTO.Email;
                patient.Phone = patientDTO.Phone;
                patient.Gender = patientDTO.Gender;
                patient.DateOfBirth = patientDTO.DateOfBirth;

                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.LabOrders)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient != null)
            {
                var hasPendingLabOrders = patient.Appointments
                    .SelectMany(a => a.LabOrders)
                    .Any(lo => lo.Status == "Pending");

                if (hasPendingLabOrders)
                {
                    throw new InvalidOperationException("Cannot delete Patient because they have appointments with Pending Lab Orders.");
                }

                _context.Patients.Remove(patient);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Delete failed: {ex.Message}", ex);
                }
            }
        }
    }
}
