using Microsoft.EntityFrameworkCore;
using MiniProject.Data;
using MiniProject.Models;
using MiniProject.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniProject.Services
{
    public class LabOrderService : ILabOrderService
    {
        private readonly ApplicationDbContext _context;

        public LabOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LabOrderDTO>> GetAllAsync()
        {
            return await _context.LabOrders
                .Include(l => l.Appointment)
                .ThenInclude(a => a.Patient)
                .Select(l => new LabOrderDTO
                {
                    Id = l.Id,
                    AppointmentId = l.AppointmentId,
                    TestName = l.TestName,
                    OrderDate = l.OrderDate,
                    Status = l.Status,
                    Result = l.Result,
                    PatientName = l.Appointment != null && l.Appointment.Patient != null ? l.Appointment.Patient.Name : "Unknown"
                })
                .ToListAsync();
        }

        public async Task<LabOrderDTO?> GetByIdAsync(int id)
        {
            var l = await _context.LabOrders
                .Include(l => l.Appointment)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (l == null) return null;

            return new LabOrderDTO
            {
                Id = l.Id,
                AppointmentId = l.AppointmentId,
                TestName = l.TestName,
                OrderDate = l.OrderDate,
                Status = l.Status,
                Result = l.Result,
                PatientName = l.Appointment != null && l.Appointment.Patient != null ? l.Appointment.Patient.Name : "Unknown"
            };
        }

        public async Task CreateAsync(LabOrderDTO labOrderDTO)
        {
            var labOrder = new LabOrder
            {
                AppointmentId = labOrderDTO.AppointmentId,
                TestName = labOrderDTO.TestName,
                OrderDate = labOrderDTO.OrderDate,
                Status = labOrderDTO.Status,
                Result = labOrderDTO.Result
            };

            _context.LabOrders.Add(labOrder);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LabOrderDTO labOrderDTO)
        {
            var labOrder = await _context.LabOrders.FindAsync(labOrderDTO.Id);
            if (labOrder != null)
            {
                labOrder.AppointmentId = labOrderDTO.AppointmentId;
                labOrder.TestName = labOrderDTO.TestName;
                labOrder.OrderDate = labOrderDTO.OrderDate;
                labOrder.Status = labOrderDTO.Status;
                labOrder.Result = labOrderDTO.Result;

                _context.LabOrders.Update(labOrder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
             var order = await _context.LabOrders.FindAsync(id);
             if (order != null)
             {
                 if (order.Status == "Pending")
                 {
                     throw new System.InvalidOperationException("Cannot delete a Lab Order with 'Pending' status.");
                 }
                 _context.LabOrders.Remove(order);
                 await _context.SaveChangesAsync();
             }
        }

        public async Task CompleteAsync(int id)
        {
            var order = await _context.LabOrders.FindAsync(id);
            if (order != null)
            {
                order.Status = "Completed";
                _context.LabOrders.Update(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LabOrderDTO>> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.LabOrders
                .Where(l => l.AppointmentId == appointmentId)
                .Select(l => new LabOrderDTO
                {
                    Id = l.Id,
                    AppointmentId = l.AppointmentId,
                    TestName = l.TestName,
                    OrderDate = l.OrderDate,
                    Status = l.Status,
                    Result = l.Result,
                    PatientName = l.Appointment != null && l.Appointment.Patient != null ? l.Appointment.Patient.Name : "Unknown"
                })
                .ToListAsync();
        }
    }
}
