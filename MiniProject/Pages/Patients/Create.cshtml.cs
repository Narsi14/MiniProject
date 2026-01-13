using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Threading.Tasks;

namespace MiniProject.Pages.Patients
{
    public class CreateModel : PageModel
    {
        private readonly IPatientService _service;

        public CreateModel(IPatientService service)
        {
            _service = service;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PatientDTO Patient { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _service.InitialCreateAsync(Patient);

            return RedirectToPage("./Index");
        }
    }
}
