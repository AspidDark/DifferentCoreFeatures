using IdentityMetanit.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMetanit.Controller
{
    public class AdminController : ControllerBase
    {
        ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.Users.ToList();

            return Ok();
        }
    }
}
