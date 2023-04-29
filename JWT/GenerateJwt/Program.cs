using System.Security.Claims;

//dotnet user-jwts create

//dotnet user-jwts create --role "admin"

//dotnet user-jwts create --role "admin" -n payer1

//dotnet user-jwts create --role "player" -n player1 --claim "subscription-gold"

//dotnet user-jwts print f09e4395

Dictionary<string, List<string>> gamesMap = new()
{
    {"player1", new List<string>(){"Street Fighter II", "Minecraft"}},
    {"player2", new List<string>(){"Forza Horizon 5", "Final Fantasy XIV", "FIFA 23"}}
};

Dictionary<string, List<string>> subscriptionMap = new()
{
    {"silver", new List<string>(){"Street Fighter II", "Minecraft"}},
    {"gold", new List<string>(){"Street Fighter II", "Minecraft", "Forza Horizon 5", "Final Fantasy XIV", "FIFA 23"}}
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/playergames", () => gamesMap)
   .RequireAuthorization(policy =>
   {
       policy.RequireRole("admin");
   });

app.MapGet("/mygames", (ClaimsPrincipal user) =>
{
    var hasClaim = user.HasClaim(claim => claim.Type == "subscription");

    if (hasClaim)
    {
        var subs = user.FindFirstValue("subscription") ?? throw new Exception("Claim has no value!");
        return Results.Ok(subscriptionMap[subs]);
    }

    ArgumentNullException.ThrowIfNull(user.Identity?.Name);
    var username = user.Identity.Name;

    if (!gamesMap.ContainsKey(username))
    {
        return Results.Empty;
    }

    return Results.Ok(gamesMap[username]);
})
.RequireAuthorization(policy =>
{
    policy.RequireRole("player");
});

app.Run();
