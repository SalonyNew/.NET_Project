using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
namespace Web.Controllers
{
    public class RecruiterController : Controller
    {
        private readonly AppdbContext _dbcontext;

        public RecruiterController(AppdbContext context)
        {
            
            _dbcontext = context;
        }


        [HttpGet]
        [Authorize(Roles = "Recruiter")]
        public IActionResult ViewCandidateApplicationForm(ApplicationViewModel model)
        {

            var application = _dbcontext.Applications.Select(app => new ApplicationViewModel
            {
               ApplicationId=app.ApplicationId,
               Name=app.Name,
               Email= app.Email,
               Education= app.Education,   
               PhoneNo= app.PhoneNo,
               Resume   = app.Resume,
               ApplicationDate= app.ApplicationDate,
            }).ToList();

            return View(application);
        }


    }
}

