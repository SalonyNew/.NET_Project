using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.ViewModels;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    public class InterviewController : Controller
    {
        private readonly AppdbContext _dbcontext;

        public InterviewController(AppdbContext context)
        {
            _dbcontext = context;
        }


        [HttpGet]
        [NoCache]
        [Authorize(Roles = "Recruiter")]
        public IActionResult ScheduleInterview(Guid applicationid)
        {
            var applicationDetail = _dbcontext.Applications.Where(application => application.ApplicationId == applicationid).FirstOrDefault();
                var interviewModel = new InterviewViewModel
                {
                    ApplicationId = applicationid,
                    ApplicantName = applicationDetail.Name,
                    
                };
           
           return View(interviewModel);
        }

        [HttpPost]
        [NoCache]
        [Authorize(Roles = "Recruiter")]
        public IActionResult ScheduleInterview(InterviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var interview = new Interview
                {
                    ApplicationId=model.ApplicationId,
                    InterviewDate = model.InterviewDate,
                    Time = model.Time,
                    Location = model.Location,
                    InterviewId = model.InterviewId
                    

                };
                _dbcontext.Interviews.Add(interview);
                _dbcontext.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            return View(model);
        }
    }

 }
