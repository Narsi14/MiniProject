using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniProject.Models.DTOs;
using MiniProject.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProject.Pages.LabOrders
{
    public class IndexModel : PageModel
    {
        private readonly ILabOrderService _service;

        public IndexModel(ILabOrderService service)
        {
            _service = service;
        }

        public IList<LabOrderDTO> LabOrders { get; set; } = default!;

        public async Task OnGetAsync()
        {
            LabOrders = await _service.GetAllAsync();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            await _service.CompleteAsync(id);
            return RedirectToPage();
        }
    }
}
