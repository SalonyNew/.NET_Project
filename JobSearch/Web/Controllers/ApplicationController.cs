using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Services;
using Services.ViewModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Web.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly AppdbContext _dbcontext;

        public ApplicationController(AppdbContext context)
        {
            _dbcontext = context;
        }

        [HttpGet]
        [Authorize(Roles = "Candidate")]
        [NoCache]
        public IActionResult JobApplicationForm(Guid jobId)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var jobPost = _dbcontext.JobPosts.FirstOrDefault(j => j.JobPostId == jobId);
                if (jobPost == null)
                {
                    Console.WriteLine($"Invalid JobId: {jobId}");
                    return RedirectToAction("Index", "Home");
                }

                var existingApplication = _dbcontext.Applications
                    .FirstOrDefault(a => a.UserId == userGuid && a.JobPostId == jobId);

                var applicationViewModel = new ApplicationViewModel
                {
                    JobPostId = jobId,
                    CompanyName = jobPost.CompanyName,
                    JobTitle = jobPost.JobTitle
                };

                if (existingApplication != null)
                {
                    return RedirectToAction("EditJobApplicationForm", new { applicationId = existingApplication.ApplicationId });
                }

                ViewBag.JobId = jobId;
                return View(applicationViewModel);
            }

            return RedirectToAction("Login", "Account"); 
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        [NoCache]
        public IActionResult JobApplicationForm(ApplicationViewModel model, Guid jobId)
        {
            if (ModelState.IsValid)
            {
                if (!User.Identity!.IsAuthenticated)
                {
                    ModelState.AddModelError(string.Empty, "User is not authenticated.");
                    return View(model);
                }

                var jobPost = _dbcontext.JobPosts.Find(jobId);
                if (jobPost == null)
                {
                    ModelState.AddModelError(string.Empty, "Job post not found.");
                    return View(model);
                }

                string userId = GetUserIdFromLoggedInUser();
                if (Guid.TryParse(userId, out Guid userGuid))
                {
                    string jwtToken = HttpContext.Request.Cookies["jwtToken"];
                    if (jwtToken == null)
                    {
                        ModelState.AddModelError(string.Empty, "JWT token not found.");
                        return View(model);
                    }

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);

                    string userRole = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    if (userRole == "Candidate")
                    {
                        var application = new Application
                        {
                            Name = model.Name,
                            Email = model.Email,
                            PhoneNo = model.PhoneNo,
                            Education = model.Education,
                            Resume = model.Resume,
                            ApplicationDate = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            UpdatedOn = DateTime.Now,
                            UserId = userGuid,
                            JobPostId = jobId
                        };
                        ViewBag.JobId = jobId;
                        _dbcontext.Applications.Add(application);
                        _dbcontext.SaveChanges();
                        return LocalRedirect("/Account/Dashboard");
                    }
                    else
                    {
                        return Forbid("You do not have permission to apply for a job.");
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
        public IActionResult JobApplicationIndex(ApplicationViewModel model)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var applications = _dbcontext.Applications
                    .Where(a => a.UserId == userGuid ).Select(a=> new ApplicationViewModel
                    {
                        ApplicationId = a.ApplicationId,
                        ApplicationDate =a.ApplicationDate,
                        CompanyName=a.JobPost.CompanyName,
                        JobTitle=a.JobPost.JobTitle,
                    })
                    .ToList();
                return View(applications);
            }
            

            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Candidate")]
        public IActionResult EditJobApplicationForm(Guid applicationId)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var application = _dbcontext.Applications
                    .FirstOrDefault(a => a.ApplicationId == applicationId && a.UserId == userGuid);

                if (application == null)
                {
                    return NotFound();
                }

                var model = new ApplicationViewModel
                {
                    ApplicationId = application.ApplicationId,  
                    Name = application.Name,
                    PhoneNo = application.PhoneNo,                   
                    Resume = application.Resume,
                    
                };

                ViewBag.JobId = application.JobPostId;
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public IActionResult EditJobApplicationForm(ApplicationViewModel model, Guid applicationId)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserIdFromLoggedInUser();
                if (Guid.TryParse(userId, out Guid userGuid))
                {
                    var application = _dbcontext.Applications
                        .FirstOrDefault(a => a.ApplicationId == applicationId && a.UserId == userGuid);

                    if (application == null)
                    {
                        return NotFound();
                    }

                   
                    application.PhoneNo = model.PhoneNo;                   
                    application.Resume = model.Resume;
                    application.UpdatedOn = DateTime.Now;

                    _dbcontext.Applications.Update(application);
                    _dbcontext.SaveChanges();

                    return RedirectToAction("JobApplicationIndex", new { JobPostId = application.JobPostId });
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
        public IActionResult JobApplicationFormDetails(Guid applicationId)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var application = _dbcontext.Applications
                    .FirstOrDefault(a => a.ApplicationId == applicationId && a.UserId == userGuid);

                if (application == null)
                {
                    return NotFound();
                }

                return View(application);
            }

            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Candidate")]
        public IActionResult DeleteJobApplicationForm(Guid applicationId)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var application = _dbcontext.Applications
                    .FirstOrDefault(a => a.ApplicationId == applicationId && a.UserId == userGuid);

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

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Candidate")]
        public IActionResult DeleteJobApplicationFormConfirmed(Guid applicationId)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var application = _dbcontext.Applications
                    .FirstOrDefault(a => a.ApplicationId == applicationId && a.UserId == userGuid);

                if (application == null)
                {
                    return NotFound();
                }

                _dbcontext.Applications.Remove(application);
                _dbcontext.SaveChanges();

                return RedirectToAction("Dashboard", "Account");
            }

            return NotFound();
        }

        private string GetUserIdFromLoggedInUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = _dbcontext.UserCredentials.FirstOrDefault(u => u.Email == userEmail)?.UserId.ToString();
            return userId!;
        }
    }
}
