namespace ABSUploadClient.Pages.Account
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using ABSUploadClient.Models;
    using ABSUploadClient.Options;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Options;

    public class Login : PageModel
    {
        private readonly LdapConfig ldapConfig;
        
        public Login(IOptions<LdapConfig> ldapConfig)
        {
            this.ldapConfig = ldapConfig.Value;
        }

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }
        
        public IActionResult OnGet()
        {
            if (this.User.Identity.IsAuthenticated)  
            {  
                return this.RedirectToPage("/Index");  
            }
            
            return this.Page(); 
        }
      

        public async Task<IActionResult> OnPostLogIn(string returnUrl)  
        {             
            if (!ModelState.IsValid) return this.Page(); 

            //using var root = new DirectoryEntry("LDAP://mmk.local/DC=mmk,DC=local", LoginViewModel.UserName, LoginViewModel.Password);
            using var root = new DirectoryEntry(this.ldapConfig.ConnectionString, LoginViewModel.UserName, LoginViewModel.Password);
            using var userSearcher = new DirectorySearcher(root, $"(&(objectClass=user)(samaccountname={LoginViewModel.UserName}))");            
            
            try
            {
                var user = userSearcher.FindAll().Cast<SearchResult>().Single();            
                var roles = user.Properties["memberof"]?.Cast<string>().Select(x => Regex.Match(x,"^CN=(?<cn>.*?)(,|$)").Groups["cn"].Value) ?? new List<string>();
                
                var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, LoginViewModel.UserName) };
                claims.AddRange(roles.Select(roleName => new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName)));
                
                var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                return string.IsNullOrEmpty(returnUrl) ? (IActionResult)RedirectToPage("/Index") : Redirect(returnUrl);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                return this.Page();             
            }              
        }  

    }
}