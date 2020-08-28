using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;

namespace ABSUploadClient.MB
{
    public class CreateModel : PageModel
    {
        private readonly PaymentOrdersContext _context;

        public CreateModel(PaymentOrdersContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ModuleBrief ModuleBrief { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ModuleBriefs.Add(ModuleBrief);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
