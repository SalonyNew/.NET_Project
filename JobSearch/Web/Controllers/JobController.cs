using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.ViewModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;


namespace Web.Controllers
{
    public class JobController : Controller
    {
        private readonly AppdbContext _dbcontext;

        public JobController(AppdbContext context)
        {
            _dbcontext = context;
        }
        [HttpGet]
        [NoCache]
        [Authorize(Roles = "Recruiter")]
        public IActionResult JobPost()
        {
            return View();
        }
        [HttpPost]
        [NoCache]
        [Authorize(Roles = "Admin, Recruiter")]
        public IActionResult JobPost(JobInfo model)
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

                string userId = GetUserIdFromLoggedInUser();

                if (Guid.TryParse(userId, out Guid userGuid))
                {

                    string userRole = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!;


                    if (userRole == "Recruiter")
                    {
                        var jobInfo = new JobPost
                        {
                            JobTitle = model.JobTitle,
                            CompanyName = model.CompanyName,
                            JobDescription = model.JobDescription,
                            QualificationRequired = model.QualificationRequired,
                            Deadline = model.Deadline,
                            Location = model.Location,
                            Type = model.Type,
                            UserId = userGuid

                        };
                        _dbcontext.JobPosts.Add(jobInfo);
                        _dbcontext.SaveChanges();
                        return LocalRedirect("/Account/Dashboard");
                    }
                    else
                    {

                        ModelState.AddModelError(string.Empty, "You do not have permission to post a job.");
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




        [Authorize(Roles = "Candidate")]
        public IActionResult JobList(JobInfo model)
        {
            var userId = GetUserIdFromLoggedInUser();

            var jobPosts = _dbcontext.JobPosts.Select(job => new JobInfo
            {
                JobPostId = job.JobPostId,
                JobTitle = job.JobTitle,
                JobDescription = job.JobDescription,
                QualificationRequired = job.QualificationRequired,
                Deadline = job.Deadline,
                Location = job.Location,
                CompanyName = job.CompanyName,
                Type = job.Type,
                ApplicationId = _dbcontext.Applications
                    .Where(application => application.JobPostId == job.JobPostId && application.UserId == Guid.Parse(userId))
                    .Select(application => application.ApplicationId)
                    .FirstOrDefault()
            }).ToList();

            return View(jobPosts);
        }

        [HttpGet]
        public IActionResult SearchJobs(string location, string type)
        {
            var userId = GetUserIdFromLoggedInUser();

            var query = _dbcontext.JobPosts.AsQueryable();


            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(job => job.Location.Contains(location));
            }


            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(job => job.Type.Contains(type));
            }


            var jobPosts = query.Select(job => new JobInfo
            {
                JobPostId = job.JobPostId,
                JobTitle = job.JobTitle,
                JobDescription = job.JobDescription,
                QualificationRequired = job.QualificationRequired,
                Deadline = job.Deadline,
                Location = job.Location,
                CompanyName = job.CompanyName,
                Type = job.Type,
                ApplicationId = _dbcontext.Applications
                    .Where(application => application.JobPostId == job.JobPostId && application.UserId == Guid.Parse(userId))
                    .Select(application => application.ApplicationId)
                    .FirstOrDefault()
            }).ToList();

            return View("JobList", jobPosts);
        }

        [HttpGet]
        [Authorize(Roles = "Recruiter")]
        public IActionResult EditJobPost(Guid JobPostId)
        {
            var userId = GetUserIdFromLoggedInUser();
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                var JobPost = _dbcontext.JobPosts.FirstOrDefault(a => a.JobPostId == JobPostId && a.UserId == userGuid);
                if (JobPost == null)
                {
                    return NotFound();
                }
                var model = new JobInfo
                {
                    JobPostId = JobPostId,
                    CompanyName = JobPost.CompanyName,
                    JobDescription = JobPost.JobDescription,
                    JobTitle = JobPost.JobTitle,
                    QualificationRequired = JobPost.QualificationRequired,
                    Location = JobPost.Location,
                    Deadline = JobPost.Deadline,
                    Type = JobPost.Type
                };
                ViewBag.UserId = JobPost.UserId;
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        [ValidateAntiForgeryToken]
        public IActionResult EditJobPost(JobInfo model, Guid JobPostId)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserIdFromLoggedInUser();
                if (Guid.TryParse(userId, out Guid userGuid))
                {
                    var JobPost = _dbcontext.JobPosts
                        .FirstOrDefault(a => a.JobPostId == JobPostId && a.UserId == userGuid);

                    if (JobPost == null)
                    {
                        return NotFound();
                    }

                    JobPost.CompanyName = model.CompanyName;
                    JobPost.JobTitle = model.JobTitle;
                    JobPost.JobDescription = model.JobDescription;
                    JobPost.QualificationRequired = model.QualificationRequired;
                    JobPost.Location = model.Location;
                    JobPost.Deadline = model.Deadline;
                    JobPost.Type = model.Type;

                    _dbcontext.JobPosts.Update(JobPost);
                    _dbcontext.SaveChanges();

                    return RedirectToAction("Dashboard", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid UserId format.");
                }
            }
            return View();
        }

        public string GetUserIdFromLoggedInUser()
        {
            var UserData = User.FindFirst(ClaimTypes.Email)?.Value;
            var UserId = _dbcontext.UserCredentials.FirstOrDefault(u => u.Email == UserData)?.UserId.ToString();
            return UserId;
        }

    }

}


