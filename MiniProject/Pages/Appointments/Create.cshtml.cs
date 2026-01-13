using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniProject.Models;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Pages.Appointments
{
    public class CreateModel : PageModel
    {
        private readonly IAppointmentService _apptService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public CreateModel(IAppointmentService apptService, IPatientService patientService, IDoctorService doctorService)
        {
            _apptService = apptService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        public SelectList PatientList { get; set; } = default!;
        public SelectList DoctorList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? patientId)
        {
            var patients = await _patientService.GetAllAsync();
            PatientList = new SelectList(patients, "Id", "Name");

            var doctors = await _doctorService.GetAllAsync();
            DoctorList = new SelectList(doctors.Select(d => new { Id = d.Id, Display = d.Name + " - " + d.Specialization }), "Id", "Display");

            if (patientId.HasValue)
            {
                Appointment = new AppointmentDTO { PatientId = patientId.Value, AppointmentDate = DateTime.Now.AddDays(1) };
            }
            else
            {
                Appointment = new AppointmentDTO { AppointmentDate = DateTime.Now.AddDays(1) };
            }

            return Page();
        }

        [BindProperty]
        public AppointmentDTO Appointment { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var patients = await _patientService.GetAllAsync();
                PatientList = new SelectList(patients, "Id", "Name");

                var doctors = await _doctorService.GetAllAsync();
                DoctorList = new SelectList(doctors.Select(d => new { Id = d.Id, Display = d.Name + " - " + d.Specialization }), "Id", "Display");
                
                return Page();
            }

            // Lookup Doctor Name for the DTO (SP expects it)
            if (Appointment.DoctorId.HasValue)
            {
                var doctors = await _doctorService.GetAllAsync();
                var selectedDoctor = doctors.FirstOrDefault(d => d.Id == Appointment.DoctorId.Value);
                if (selectedDoctor != null)
                {
                    Appointment.DoctorName = selectedDoctor.Name;
                }
            }

            // Calls the Service which calls the SP
            await _apptService.CreateAsync(Appointment);

            return RedirectToPage("./Index");
        }
    }
}
