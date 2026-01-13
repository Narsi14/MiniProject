using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Threading.Tasks;

namespace MiniProject.Pages.Patients
{
    public class DeleteModel : PageModel
    {
        private readonly IPatientService _service;

        public DeleteModel(IPatientService service)
        {
            _service = service;
        }

        [BindProperty]
        public PatientDTO Patient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _service.GetByIdAsync(id.Value);

            if (patient == null)
            {
                return NotFound();
            }
            Patient = patient;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                await _service.DeleteAsync(id.Value);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var patient = await _service.GetByIdAsync(id.Value);
                if (patient != null) Patient = patient;
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
