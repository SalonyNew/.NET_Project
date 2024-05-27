using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    public class InterviewController : Controller
    {
        private readonly AppdbContext _context;

        public InterviewController(AppdbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Authorize(Roles = "Recruiter")]
        public IActionResult ScheduleInterview(Guid ApplicationId)
        {

            ViewBag.ApplicationId = ApplicationId;
            return View(new InterviewViewModel());
        }


        [HttpPost]
        [NoCache]
        [Authorize(Roles = "Recruiter")]
        public IActionResult ScheduleInterview(InterviewViewModel model, Guid ApplicationId)
        {
            if (ModelState.IsValid)
            {
                
               
                    var interview = new Interview
                    {
                        ApplicationId = ApplicationId,
                        InterviewDate = model.InterviewDate,
                        Time = model.Time,
                        Location = model.Location,
                       
                    };
                    _context.Interviews.Add(interview);
                    _context.SaveChanges();
                return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Application not found.");
                }
            
           
            return View(model);
        }
    }

 }
