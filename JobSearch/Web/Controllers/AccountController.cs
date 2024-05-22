using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services.ViewModels;
using System.Text;
using Services.PasswordEncryption;
using System.Security.Cryptography.Xml;
using Azure.Identity;
using Services;
using System.Diagnostics.Eventing.Reader;



namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppdbContext _context;
        private readonly IConfiguration _config;
        private readonly AESEncryptionUtility _encryption;
        private readonly JWTHelper _helper;

        public AccountController(AppdbContext context, IConfiguration config, AESEncryptionUtility encryption,JWTHelper helper)
        {
            _context = context;
            _config = config;
            _encryption = encryption;
            _helper = helper;
        }

        

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(Signup model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.UserCredentials.Any(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "A user with this email already exists.");
                        return View(model);
                    }

                    var roleId = _context.Roles.FirstOrDefault(r => r.Name == model.Role)?.RoleId;
                     
                    var passwordHash = _encryption.Encrypt(model.Password, _config["EncryptionKey"]!, out string passwordSalt);
                    var user = new UserCredential
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Age = model.Age,
                        Gender = model.Gender,
                        RoleId = _context.Roles
                        .Where(role => role.Name == model.Role)
                        .Select(role => role.RoleId)
                        .FirstOrDefault(),
                        PasswordHash = passwordHash, 
                        PasswordSalt = passwordSalt,
                        CreatedAt = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                        PhoneNo = model.PhoneNo,
                        Dob = model.Dob
                    };

                    
                    _context.UserCredentials.Add(user);
                    _context.SaveChanges();

                    return RedirectToAction("SignupSuccess");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult SignupSuccess()
        {
            return RedirectToAction("Dashboard");
        }


        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn( Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _context.UserCredentials
                        .Where(u => u.Email == model.Email).FirstOrDefault();

                        var passwordHash = _encryption.Authencrypt(model.Password, _config["EncryptionKey"]!, user!.PasswordSalt);
                    if (user != null && user.PasswordHash == passwordHash)
                    {
                        var RoleName = _context.Roles.Where(role => role.RoleId == user.RoleId).Select(role => role.Name).FirstOrDefault();
                        var token = _helper.GenerateJwtToken(user.Email, RoleName!.ToString());

                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        };

                        HttpContext.Response.Cookies.Append("jwtToken", token, cookieOptions);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid email or password.");
                        return View(model);
                    }
                }

            }
            catch (Exception )
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");               
            }
            return View();
        }
        public IActionResult Logout()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

    }
}
