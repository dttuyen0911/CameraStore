using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext")));
builder.Services.AddNotyf(config => { 
    config.DurationInSeconds = 10; 
    config.IsDismissable = true; 
    config.Position = NotyfPosition.TopRight; 
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Authentication/Error";
    });
builder.Services.Configure<PaymentIntentCreateRequest>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddMvc().AddNewtonsoftJson();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".YourApp.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout after 30 minutes of inactivity
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });

    options.AddPolicy("MemberPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Member");
    });
    options.AddPolicy("EmployeePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Employee");
    });
    options.AddPolicy("OwnerPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Owner");
    });
    options.AddPolicy("OwnerOrEmployeePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Owner", "Employee");
    });
});

var app = builder.Build();

StripeConfiguration.ApiKey = "sk_test_51P3fJLF7FOWXnm57tBhVroVz0lg3TyEAyfIfUMj3b9q136FVxeEMtn31hmUvJsXZA6lBpbSp0ODQNdfF6vb1Bp9Q00XxTFtlwd";
// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseNotyf();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<String>();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

// Configure endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
