using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ABSUploadClient.Entity;
using ABSUploadClient.Entity.EntityModel;

namespace ABSUploadClient.MB
{
    public class IndexModel : PageModel
    {
        private readonly PaymentOrdersContext _context;

        public IndexModel(PaymentOrdersContext context)
        {
            _context = context;
        }

        public IList<ModuleBrief> ModuleBrief { get;set; }

        public async Task OnGetAsync()
        {
            ModuleBrief = await _context.ModuleBriefs.ToListAsync();
        }
    }
}
