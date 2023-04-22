using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.Controllers;

public class SomeController : ControllerBase
{

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    public async Task Get()
    { 
        
    }


    [Authorize]
    [RequiresClaim(IdentityData.AdminUserClaimName, "true")]
    public async Task Post()
    { 
    
    }

}
