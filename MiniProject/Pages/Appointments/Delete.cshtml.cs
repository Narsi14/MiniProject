using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Threading.Tasks;

namespace MiniProject.Pages.Appointments
{
    public class DeleteModel : PageModel
    {
        private readonly IAppointmentService _service;

        public DeleteModel(IAppointmentService service)
        {
            _service = service;
        }

        [BindProperty]
        public AppointmentDTO Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var appt = await _service.GetByIdAsync(id.Value);

            if (appt == null) return NotFound();
            Appointment = appt;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                await _service.DeleteAsync(id.Value);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var appt = await _service.GetByIdAsync(id.Value);
                if (appt != null) Appointment = appt;
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
