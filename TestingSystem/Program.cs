using Microsoft.AspNetCore.Authentication.Cookies;
using TestingSystem.Data;
using TestingSystem.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "TestingSystemAuth";
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<DatabaseHelper>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<DisciplineRepository>();
builder.Services.AddSingleton<TestRepository>();
builder.Services.AddSingleton<TestResultRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();