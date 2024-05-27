using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using System.Linq.Expressions;
using Services.ViewModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppdbContext _context;

        public ProfileController(AppdbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Profile(UserInfo model)
        {
            if (ModelState.IsValid)
            {
                if (!User.Identity!.IsAuthenticated)
                {
                    ModelState.AddModelError(string.Empty, "User is not Authenticated.");
                    return View(model);
                }

                string jwtToken = HttpContext.Request.Cookies["jwtToken"]!;
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);

                var userId = GetUserIdFromLoggedInUser();


                if (Guid.TryParse(userId, out Guid userGuid))
                {

                    string userRole = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!;
                    {

                        if (userRole == "Candidate")
                        {
                            var profile = new Profile
                            {

                                Education = model.Education,
                                WorkExperience = model.WorkExperience,
                                Skills = model.Skills,
                                Resume = model.Resume,
                                UserId = Guid.Parse(userId)
                            };

                            _context.Profiles.Add(profile);
                            _context.SaveChanges();

                            return RedirectToAction("Dashboard");
                        }
                        else
                        {

                            ModelState.AddModelError(string.Empty, "Invalid UserId format.");
                            return View(model);
                        }
                    }
                }

                else
                {

                    ModelState.AddModelError(string.Empty, "UserId not available.");
                    return View(model);
                }
            }

            return View(model);
        }
        private string GetUserIdFromLoggedInUser()
        {
            var UserData = User.FindFirst(ClaimTypes.Email)?.Value;
            var UserId = _context.UserCredentials.FirstOrDefault(u => u.Email == UserData)!.UserId.ToString();
            return UserId;

        }
    }
}
