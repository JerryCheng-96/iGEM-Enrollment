using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iGEM_Enrollment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iGEM_Enrollment.Controllers
{
    public class ManageController : Controller
    {

        private readonly ApplyContext _context;
        private readonly List<Applicant> Applicants;

        public ManageController(ApplyContext applyContext)
        {
            _context = applyContext;

            Applicants = _context.Applicant
                .Include(a => a.appliForm)
                .AsNoTracking()
                .OrderBy(i => i.grade)
                .ThenBy(i => i.college)
                .ThenBy(i => i.major)
                .ThenBy(i => i.stuId)
                .ToList();
        }

        public IActionResult ApplicantList()
        {
            ViewData["List"] = Applicants;

            return View();
        }

        public IActionResult ShowForm(String stuId, String inEnum)
        {
            if (inEnum == "Yes")
            {
                var index = -1;
                index = Applicants.FindIndex(i => i.stuId.ToString() == stuId);
                ViewData["stuId"] = Applicants[index + 1].stuId;
                return View();
            }

            ViewData["stuId"] = stuId;
            ViewData["nextIndex"] = 0;

            return View();
        }

        [HttpGet]
        public IActionResult GetApplicant(String stuId)
        {
            try
            {
                Applicant theApplicant = Applicants.Single(u => (u.stuId == long.Parse(stuId)));
                return new ObjectResult(new FormString(theApplicant));
            }
            catch (Exception e)
            {
            }

            return null;
        }

        [HttpGet]
        public IActionResult GetApplicantByIndex(String index)
        {
            return new ObjectResult(new FormString(Applicants[int.Parse(index)]));
        }
    }
}