using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniProject.Models;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Linq;
using System.Collections.Generic;

namespace MiniProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly ILabOrderService _labOrderService;

        public IndexModel(IPatientService patientService, IAppointmentService appointmentService, ILabOrderService labOrderService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _labOrderService = labOrderService;
        }

        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalLabOrders { get; set; }
        public List<AppointmentDTO> RecentAppointments { get; set; } = new List<AppointmentDTO>();

        public async Task OnGetAsync()
        {
            var patients = await _patientService.GetAllAsync();
            TotalPatients = patients.Count;

            var appointments = await _appointmentService.GetAllAsync();
            TotalAppointments = appointments.Count;
            RecentAppointments = appointments.OrderByDescending(a => a.AppointmentDate).Take(5).ToList();

            var labOrders = await _labOrderService.GetAllAsync();
            TotalLabOrders = labOrders.Count;
        }
    }
}
