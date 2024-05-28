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


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            // For API requests, return the unauthorized message
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Please log in to access this resource.");
        }
        else
        {
            string loginUrl = $"/Account/LogIn?returnUrl={context.Request.Path.Value}&message=Please log in to access this resource.";
            context.Response.Redirect(loginUrl);
        }
    }
});

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
