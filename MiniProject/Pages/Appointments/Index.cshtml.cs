using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniProject.Models;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Pages.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly IAppointmentService _service;

        public IndexModel(IAppointmentService service)
        {
            _service = service;
        }

        public IList<AppointmentDTO> Appointments { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Appointments = await _service.GetAllAsync();
        }
    }
}
