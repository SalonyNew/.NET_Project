using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Web.Controllers
{
    public class JobController : Controller
    {
        private readonly AppdbContext _context;

        public JobController(AppdbContext context)
        {
            _context = context;
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
                            JobDescription = model.JobDescription,
                            QualificationRequired = model.QualificationRequired,
                            Deadline = model.Deadline,
                            Location = model.Location,
                            Industry = model.Industry,
                            Type = model.Type,
                            UserId = userGuid 
                            
                        };

                        _context.JobPosts.Add(jobInfo);
                        _context.SaveChanges();
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

        public string GetUserIdFromLoggedInUser()
        {
            var UserData= User.FindFirst(ClaimTypes.Email)?.Value;
            var UserId = _context.UserCredentials.FirstOrDefault(u => u.Email == UserData)!.UserId.ToString();
            return UserId;
        }

     
        [Authorize(Roles = "Candidate")]
        public IActionResult JobList(JobInfo model)
        {
            
            var jobPosts = _context.JobPosts.Select(job => new JobInfo
            {
                JobPostId= job.JobPostId,
                JobTitle = job.JobTitle,
                JobDescription = job.JobDescription,
                QualificationRequired = job.QualificationRequired,
                Deadline = job.Deadline,
                Location = job.Location,
                Industry = job.Industry,
                Type = job.Type
            }).ToList();

            return View(jobPosts);           
        }


    }
}

