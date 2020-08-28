using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;

namespace ABSUploadClient.MB
{
    public class EditModel : PageModel
    {
        private readonly PaymentOrdersContext _context;

        public EditModel(PaymentOrdersContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ModuleBrief).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleBriefExists(ModuleBrief.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ModuleBriefExists(int id)
        {
            return _context.ModuleBriefs.Any(e => e.Id == id);
        }
    }
}
