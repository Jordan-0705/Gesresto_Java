using Data;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Impl;
using Microsoft.AspNetCore.Authentication.Cookies;
using Models;

var builder = WebApplication.CreateBuilder(args);

// ===== Ajouter DbContext avec Npgsql =====
builder.Services.AddDbContext<GesRestoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== Ajouter les services =====
builder.Services.AddScoped<IBurgerService, BurgerService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IComplementService, ComplementService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICommandeService, CommandeService>();
builder.Services.AddScoped<IPaiementService, PaiementService>();
builder.Services.AddScoped<IZoneService, ZoneService>();

// ===== Ajouter MVC =====
builder.Services.AddControllersWithViews();

// ===== Authentication & Authorization =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Client/Login";         // Page de login
        options.AccessDeniedPath = "/Client/AccessDenied"; // Page accès refusé
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ===== Middleware =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ===== Authentication / Authorization =====
app.UseAuthentication();
app.UseAuthorization();

// ===== Routes =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Client}/{action=Index}/{id?}"
);

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();