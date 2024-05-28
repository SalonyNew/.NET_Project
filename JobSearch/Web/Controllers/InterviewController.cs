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
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationId == ApplicationId);
            if (application == null)
            {
                return NotFound();
            }

            var model = new InterviewViewModel
            {
                ApplicationId = application.ApplicationId,
                ApplicantName = application.Name 
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public IActionResult ScheduleInterview(InterviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var interview = new Interview
                {
                    ApplicationId = model.ApplicationId,
                    InterviewDate = model.InterviewDate,
                    Time = model.Time,
                    Location = model.Location,
                };
                _context.Interviews.Add(interview);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            return View(model);
        }
    }

 }
