using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Pages.Patients
{
    public class IndexModel : PageModel
    {
        private readonly IPatientService _service;

        public IndexModel(IPatientService service)
        {
            _service = service;
        }

        public IList<PatientDTO> Patients { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Patients = await _service.GetAllAsync();
        }
    }
}
