using IdentityMetanit.Data;
using IdentityMetanit.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddTransient<IUserValidator<User>, CustomUserValidator>();

// ���������� �������� Idenity
builder.Services.AddDefaultIdentity<IdentityUser>(opts => {
    opts.SignIn.RequireConfirmedAccount = true;
    opts.Password.RequiredLength = 5;   // ����������� �����
    opts.Password.RequireNonAlphanumeric = false;   // ��������� �� �� ���������-�������� �������
    opts.Password.RequireLowercase = false; // ��������� �� ������� � ������ ��������
    opts.Password.RequireUppercase = false; // ��������� �� ������� � ������� ��������
    opts.Password.RequireDigit = false; // ��������� �� �����

    opts.User.RequireUniqueEmail = true;    // ���������� email
    opts.User.AllowedUserNameCharacters = ".@abcdefghijklmnopqrstuvwxyz"; // ���������� �������
})
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Idenity
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

/*
 Users: ����� �������� IdentityUser, ������������� ������� �������������

Roles: ����� �������� IdentityRole, ������������� ������� �����

RoleClaims: ����� �������� IdentityRoleClaim, ������������� ������� ����� ����� � �������� claims

UserLogins: ����� �������� IdentityUserLogin, ������������� ������� ����� ������������� � �� �������� �� ������� ��������

UserClaims: ����� �������� IdentityUserClaim, ������������� ������� ����� ������������� � �������� claims

UserRoles: ����� �������� IdentityUserRole, ������������� �������, ������� ������������ ������������� � �� ����

UserTokens: ����� �������� IdentityUserToken, ������������� ������� ������� �������������
 */
