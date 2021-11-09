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

// добавление сервисов Idenity
builder.Services.AddDefaultIdentity<IdentityUser>(opts => {
    opts.SignIn.RequireConfirmedAccount = true;
    opts.Password.RequiredLength = 5;   // минимальная длина
    opts.Password.RequireNonAlphanumeric = false;   // требуются ли не алфавитно-цифровые символы
    opts.Password.RequireLowercase = false; // требуются ли символы в нижнем регистре
    opts.Password.RequireUppercase = false; // требуются ли символы в верхнем регистре
    opts.Password.RequireDigit = false; // требуются ли цифры

    opts.User.RequireUniqueEmail = true;    // уникальный email
    opts.User.AllowedUserNameCharacters = ".@abcdefghijklmnopqrstuvwxyz"; // допустимые символы
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
 Users: набор объектов IdentityUser, соответствует таблице пользователей

Roles: набор объектов IdentityRole, соответствует таблице ролей

RoleClaims: набор объектов IdentityRoleClaim, соответствует таблице связи ролей и объектов claims

UserLogins: набор объектов IdentityUserLogin, соответствует таблице связи пользователей с их логинами их внешних сервисов

UserClaims: набор объектов IdentityUserClaim, соответствует таблице связи пользователей и объектов claims

UserRoles: набор объектов IdentityUserRole, соответствует таблице, которая сопоставляет пользователей и их роли

UserTokens: набор объектов IdentityUserToken, соответствует таблице токенов пользователей
 */
