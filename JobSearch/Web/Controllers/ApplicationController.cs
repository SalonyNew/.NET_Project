using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.ViewModels;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Web.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly AppdbContext _context;

        public ApplicationController(AppdbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Candidate")]
        [NoCache]
        public IActionResult Application(Guid jobId)
        {
            ViewBag.JobPostId = jobId;
            return View(new ApplicationViewModel());
        }


        [HttpPost]
        [Authorize(Roles = "Candidate")]
        [NoCache]
        public IActionResult Application(ApplicationViewModel model, Guid jobId)
        {
            if (ModelState.IsValid)
            {
                if (!User.Identity!.IsAuthenticated)
                {
                    ModelState.AddModelError(string.Empty, "User is not authenticated.");
                    return View(model);
                }

                string jwtToken = HttpContext.Request.Cookies["jwtToken"]!;
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);

                string userId = GetUserIdFromLoggedInUser();

                if (Guid.TryParse(userId, out Guid userGuid))
                {
                    string userRole = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!;

                    if (userRole == "Candidate")
                    {
                        var application = new Application
                        {
                            Name = model.Name,
                            Email = model.Email,
                            PhoneNo = model.PhoneNo,
                            Education = model.Education,
                            Resume = model.Resume,
                            ApplicationDate = model.ApplicationDate,
                            CreatedAt = DateTime.Now,
                            UpdatedOn = DateTime.Now,
                            UserId = userGuid,
                            JobPostId = jobId  
                        };

                        _context.Applications.Add(application);
                        _context.SaveChanges();
                        return LocalRedirect("/Account/Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "You do not have permission to apply for a job.");
                        return Forbid();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid UserId format.");
                }
            }

            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Candidate")]
        public IActionResult EditApplication(Guid applicationId)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationId == applicationId);
            if (application == null)
            {
                return NotFound();
            }

            var model = new ApplicationViewModel
            {
                ApplicationId = application.ApplicationId,
                Name = application.Name,
                Email = application.Email,
                PhoneNo = application.PhoneNo,
                Education = application.Education,
                Resume = application.Resume,
                ApplicationDate = application.ApplicationDate
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public IActionResult EditApplication(ApplicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var application = _context.Applications.FirstOrDefault(a => a.ApplicationId == model.ApplicationId);
                if (application == null)
                {
                    return NotFound();
                }

                application.Name = model.Name;
                application.Email = model.Email;
                application.PhoneNo = model.PhoneNo;
                application.Education = model.Education;
                application.Resume = model.Resume;
                application.ApplicationDate = model.ApplicationDate;
                application.UpdatedOn = DateTime.Now;

                _context.Applications.Update(application);
                _context.SaveChanges();

                return RedirectToAction("Dashboard", "Account");
            }

            return View(model);
        }

        private string GetUserIdFromLoggedInUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = _context.UserCredentials.FirstOrDefault(u => u.Email == userEmail)?.UserId.ToString();
            return userId!;
        }
    }

}


