using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using iGEM_Enrollment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iGEM_Enrollment.Controllers
{
    public class ApplyController : Controller
    {

        private readonly ApplyContext _context;

        public ApplyController(ApplyContext applyContext)
        {
            _context = applyContext;
        }

        public IActionResult Form()
        {
            return View();
        }


        //[HttpPost]
        //public IActionResult Welcome([FromBody] InitDataString initDataString)
        //{
        //    ViewData["initName"] = initDataString.name;
        //    ViewData["initId"] = initDataString.stuId;

        //    return View();
        //}

        [HttpGet]
        public IActionResult Welcome(String name, String stuId)
        {
            ViewData["initName"] = name;
            ViewData["initId"] = stuId;

            List<User> Users = _context.UserData
                .AsNoTracking()
                .OrderBy(u => u.stuId)
                .ToList();

            try
            {
                User theUser = Users.Single(u => (u.stuId == long.Parse(stuId) && u.name == name));
                ViewData["isExist"] = "Yes";
            }
            catch (InvalidOperationException e)
            {
                ViewData["isExist"] = "No";
            }

            return View();
        }

        [HttpPost]
        public IActionResult SubmitForm([FromBody]AppliFormString theFormString)
        {


            return null;
        }
    }
}