using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Threading.Tasks;

namespace MiniProject.Pages.LabOrders
{
    public class CreateModel : PageModel
    {
        private readonly ILabOrderService _labService;
        private readonly IAppointmentService _apptService;

        public CreateModel(ILabOrderService labService, IAppointmentService apptService)
        {
            _labService = labService;
            _apptService = apptService;
        }

        public SelectList AppointmentList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? appointmentId)
        {
            var appts = await _apptService.GetAllAsync();
            // Just showing ID and Patient Name for simple selection
            var selectListItems = appts.ConvertAll(a => new { Id = a.Id, Text = $"#{a.Id} - {a.PatientName} ({a.AppointmentDate.ToShortDateString()})" });
            
            AppointmentList = new SelectList(selectListItems, "Id", "Text", appointmentId);
            
            LabOrder = new LabOrderDTO();
            if(appointmentId.HasValue)
            {
                LabOrder.AppointmentId = appointmentId.Value;
            }
            
            return Page();
        }

        [BindProperty]
        public LabOrderDTO LabOrder { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // ModelState valid check omitted for brevity in demo if nav props cause issues, but generally good.
            // Removing navigation property validation issues
            ModelState.Remove("LabOrder.Appointment");

            if (!ModelState.IsValid)
            {
                 var appts = await _apptService.GetAllAsync();
                 var selectListItems = appts.ConvertAll(a => new { Id = a.Id, Text = $"#{a.Id} - {a.PatientName}" });
                 AppointmentList = new SelectList(selectListItems, "Id", "Text");
                 return Page();
            }

            await _labService.CreateAsync(LabOrder);

            return RedirectToPage("./Index");
        }
    }
}
