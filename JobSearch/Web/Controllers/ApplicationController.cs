﻿using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Services;
using Services.ViewModels;
using System;
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
        public IActionResult JobApplicationForm(Guid jobId)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                
                var jobPost = _context.JobPosts.FirstOrDefault(j => j.JobPostId == jobId);
                if (jobPost == null)
                {                  
                    Console.WriteLine($"Invalid JobId: {jobId}");
                    return RedirectToAction("Index", "Home"); 
                }

                var existingApplication = _context.Applications
                    .FirstOrDefault(a => a.UserId == userGuid && a.JobPostId == jobId);

                if (existingApplication != null)
                {
                    return RedirectToAction("EditApplication", new { applicationId = existingApplication.ApplicationId });
                }
            }

           
            ViewBag.JobPostId = jobId;
            return View(new ApplicationViewModel());
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
                var jobPost = _context.JobPosts.Find(jobId);
                if (jobPost == null)
                {
                    ModelState.AddModelError(string.Empty, "Job post not found.");
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
        public IActionResult JobApplicationIndex(Application model)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var applications = _context.Applications
                    .Where(a => a.UserId == userGuid)
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
                var application = _context.Applications
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

                ViewBag.JobPostId = application.JobPostId;
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
                    var application = _context.Applications
                        .FirstOrDefault(a => a.ApplicationId == applicationId && a.UserId == userGuid);

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
                var application = _context.Applications
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
                var application = _context.Applications
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
                var application = _context.Applications
                    .FirstOrDefault(a => a.ApplicationId == applicationId && a.UserId == userGuid);

                if (application == null)
                {
                    return NotFound();
                }

                _context.Applications.Remove(application);
                _context.SaveChanges();

                return RedirectToAction("Dashboard", "Account");
            }

            return NotFound();
        }



        private string GetUserIdFromLoggedInUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = _context.UserCredentials.FirstOrDefault(u => u.Email == userEmail)?.UserId.ToString();
            return userId!;
        }
    }
}
