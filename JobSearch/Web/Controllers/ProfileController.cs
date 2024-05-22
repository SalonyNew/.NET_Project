using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using System.Linq.Expressions;
using Services.ViewModels;
using System.Security.Claims;


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
                // Retrieve the UserCredential for the current user
                var userId = GetUserIdFromLoggedInUser();


                if (!string.IsNullOrEmpty(userId))
                {
                    if (Guid.TryParse(userId, out Guid userGuid))
                    {
                        var profile = new Profile
                        {
                            // Set the UserId obtained from the logged-in user
                            
                            Education = model.Education,
                            WorkExperience = model.WorkExperience,
                            Skills = model.Skills,
                            Resume = model.Resume,
                            CompanyName = model.CompanyName,
                            CompanyDescription = model.CompanyDescription,
                            Mission = model.Mission,
                            UserId = Guid.Parse(userId)
                        };

                        _context.Profiles.Add(profile);
                        _context.SaveChanges();

                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        // Handle case where UserId cannot be parsed to Guid
                        ModelState.AddModelError(string.Empty, "Invalid UserId format.");
                        return View(model);
                    }
                }
                else
                {
                    // Handle case where UserId is null or empty
                    ModelState.AddModelError(string.Empty, "UserId not available.");
                    return View(model);
                }
            }

            return View(model);
        }
        private string GetUserIdFromLoggedInUser()
        {
             var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value; ;
        }

    }
}
