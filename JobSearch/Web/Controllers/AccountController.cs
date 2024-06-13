using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using System.Text;
using Services.PasswordEncryption;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Services;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppdbContext _dbcontext;
        private readonly IConfiguration _config;
        private readonly AESEncryptionUtility _encryption;
        private readonly JWTHelper _helper;

        public AccountController(AppdbContext context, IConfiguration config, AESEncryptionUtility encryption, JWTHelper helper)
        {
            _dbcontext = context;
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
        public IActionResult Signup(SignUp model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    if (_dbcontext.UserCredentials.Any(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "A user with this email already exists.");
                        return View(model);
                    }

                    
                    var passwordHash = _encryption.Encrypt(model.Password, _config["EncryptionKey"]!, out string passwordSalt);

                    
                    var user = new UserCredential
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Age = model.Age,
                        Gender = model.Gender,
                        RoleId = _dbcontext.Roles
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

                    
                    _dbcontext.UserCredentials.Add(user);
                    _dbcontext.SaveChanges();
                    TempData["SignupSuccess"] = true;

                    return RedirectToAction("Dashboard");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var user = _dbcontext.UserCredentials
                        .Where(u => u.Email == model.Email)
                        .FirstOrDefault();

                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid email or username.");
                        return View(model);
                    }

                    
                    var passwordHash = _encryption.Authencrypt(model.Password, _config["EncryptionKey"]!, user.PasswordSalt);

                    
                    if (user.PasswordHash == passwordHash)
                    {
                        
                        var roleName = _dbcontext.Roles
                            .Where(role => role.RoleId == user.RoleId)
                            .Select(role => role.Name)
                            .FirstOrDefault();

                        
                        var token = _helper.GenerateJwtToken(user.Email, roleName!.ToString());

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
                        ModelState.AddModelError(string.Empty, "Invalid password.");
                        return View(model);
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            
            Response.Cookies.Delete("jwtToken");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            
            string userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            
            ViewData["UserRole"] = userRole;
            return View();
        }
    }
}
