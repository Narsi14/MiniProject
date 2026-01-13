using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniProject.Models;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Threading.Tasks;

namespace MiniProject.Pages.Appointments
{
    public class EditModel : PageModel
    {
        private readonly IAppointmentService _apptService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public EditModel(IAppointmentService apptService, IPatientService patientService, IDoctorService doctorService)
        {
            _apptService = apptService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        [BindProperty]
        public AppointmentDTO Appointment { get; set; } = default!;

        public SelectList PatientList { get; set; } = default!;
        public SelectList DoctorList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var appt = await _apptService.GetByIdAsync(id.Value);
            if (appt == null) return NotFound();
            Appointment = appt;

            var patients = await _patientService.GetAllAsync();
            PatientList = new SelectList(patients, "Id", "Name");

            var doctors = await _doctorService.GetAllAsync();
            DoctorList = new SelectList(doctors.Select(d => new { Id = d.Id, Display = d.Name + " - " + d.Specialization }), "Id", "Display");

            return Page();
        }

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

            try
            {
                // Lookup Doctor Name (SP/Service might expect it to stay in sync)
                if (Appointment.DoctorId.HasValue)
                {
                    var doctors = await _doctorService.GetAllAsync();
                    var selectedDoctor = doctors.FirstOrDefault(d => d.Id == Appointment.DoctorId.Value);
                    if (selectedDoctor != null)
                    {
                        Appointment.DoctorName = selectedDoctor.Name;
                    }
                }

                await _apptService.UpdateAsync(Appointment);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                
                // Reload lists
                var patients = await _patientService.GetAllAsync();
                PatientList = new SelectList(patients, "Id", "Name");

                var doctors = await _doctorService.GetAllAsync();
                DoctorList = new SelectList(doctors.Select(d => new { Id = d.Id, Display = d.Name + " - " + d.Specialization }), "Id", "Display");

                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
