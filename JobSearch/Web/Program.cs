using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.PasswordEncryption;
using Services.ViewModels;
using System.Net;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppdbContext>(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DBString")));
builder.Services.AddDbContext<AppdbContext>(options => options.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AESEncryptionUtility>();
builder.Services.AddScoped<JWTHelper>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["jwtissuer"],
        ValidAudience = builder.Configuration["jwtaudience"],

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtSecretKey"]))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RecruiterOnly", policy => policy.RequireRole("Recruiter"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var JWTokenCookie = context.Request.Cookies["jwtToken"];
    if (!string.IsNullOrEmpty(JWTokenCookie))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + JWTokenCookie);
    }
    await next();
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
