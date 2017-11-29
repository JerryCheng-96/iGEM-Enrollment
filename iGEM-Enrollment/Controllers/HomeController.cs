﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iGEM_Enrollment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace iGEM_Enrollment.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("savedHashValue") != null)
            {
                return Redirect("/Apply/Form");
            }

            ViewData["nameSession"] = HttpContext.Session.GetString("name");
            ViewData["idSession"] = HttpContext.Session.GetString("stuId");
            ViewData["isExistSession"] = HttpContext.Session.GetString("isExist");

            return Redirect("/Manage/ApplicantList/");
        }

    }
}
