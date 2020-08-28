using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;

namespace ABSUploadClient.MB
{
    public class DeleteModel : PageModel
    {
        private readonly PaymentOrdersContext _context;

        public DeleteModel(PaymentOrdersContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ModuleBrief ModuleBrief { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ModuleBrief = await _context.ModuleBriefs.FirstOrDefaultAsync(m => m.Id == id);

            if (ModuleBrief == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ModuleBrief = await _context.ModuleBriefs.FindAsync(id);

            if (ModuleBrief != null)
            {
                _context.ModuleBriefs.Remove(ModuleBrief);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
