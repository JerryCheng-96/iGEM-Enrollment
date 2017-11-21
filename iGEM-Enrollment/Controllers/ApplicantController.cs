using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Microsoft.AspNetCore.Mvc;

namespace iGEM_Enrollment.Controllers
{
    public class ApplicantController : Controller
    {
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

            return View();
        }
    }
}