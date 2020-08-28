using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ABSUploadClient.Pages.Account
{
    using Microsoft.AspNetCore.Authorization;

    [AllowAnonymous]
    public class AccessDenied : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}