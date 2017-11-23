using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using iGEM_Enrollment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace iGEM_Enrollment.Controllers
{
    public class ApplyController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ApplyContext _context;

        public ApplyController(ApplyContext applyContext, IMemoryCache memoryCache)
        {
            _context = applyContext;
            _memoryCache = memoryCache;
        }

        public IActionResult ShowForm()
        {
            if (HttpContext.Session.GetString("eHashValue") != null)
            {
                ViewData["isExist"] = "Yes";
                ViewData["eHashValue"] = HttpContext.Session.GetString("eHashValue");
            }

            return View();
        }

        public IActionResult Form()
        {
            ViewData["inputName"] = HttpContext.Session.GetString("name");
            ViewData["inputId"] = HttpContext.Session.GetString("stuId");
            ViewData["isExistSaved"] = HttpContext.Session.GetString("savedHashValue") == null ? "No" : "Yes";

            if (HttpContext.Session.GetString("name") == null || HttpContext.Session.GetString("stuId") == null)
            {
                return Redirect("/");
            }

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
            if (HttpContext.Session.GetString("savedHashValue") != null)
            {
                return Redirect("/Apply/Form");
            }

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
            catch (Exception e)
            {
                ViewData["isExist"] = "No";
            }

            HttpContext.Session.SetString("name", name);
            HttpContext.Session.SetString("stuId", stuId);

            return View();
        }

        [HttpGet]
        public IActionResult Retrieve(String hashValue)
        {
            HttpContext.Session.SetString("eHashValue", hashValue);
            return Redirect("/Apply/ShowForm/");
        }

        public async Task<IActionResult> GetExistForm()
        {
            var inputName = HttpContext.Session.GetString("name");
            var inputId = HttpContext.Session.GetString("stuId");
            var hashValue = HttpContext.Session.GetString("eHashValue");

            List<User> Users = await _context.UserData
                            .AsNoTracking()
                            .OrderBy(u => u.stuId)
                            .ToListAsync();

            List<Applicant> Applicants = await _context.Applicant
                            .Include(a => a.appliForm)
                            .AsNoTracking()
                            .ToListAsync();

            try
            {
                Applicant theApplicant = Applicants.Single(u => (u.stuId == long.Parse(inputId) && u.name == inputName));
                User theUser = Users.Single(u => (u.stuId == long.Parse(inputId) && u.name == inputName && u.token ==
                                Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                    password: theApplicant.appliForm.appliFormId,
                                    salt: Encoding.UTF8.GetBytes(hashValue),
                                    prf: KeyDerivationPrf.HMACSHA256,
                                    iterationCount: 10000,
                                    numBytesRequested: 256 / 8))));
                return new ObjectResult(new FormString(theApplicant));
            }
            catch (Exception e)
            {
            }

            return new ObjectResult("");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody]FormString theFormString)
        {
            var holoForm = new HoloForm(theFormString);
            var appliForm = new AppliForm(holoForm);
            var applicant = new Applicant(holoForm, appliForm);

            var hashBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(holoForm.ToString()));
            var hashValue = Convert
                .ToString(BitConverter.ToInt64(hashBytes, 0), 16)
                .ToUpper();

            var user = new User
            {
                name = holoForm.name,
                stuId = holoForm.stuId,
                token = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: appliForm.appliFormId,
                    salt: Encoding.UTF8.GetBytes(hashValue),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8))
            };

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(appliForm);
                    _context.Add(applicant);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return new ObjectResult(hashValue);
                }
            }
            catch (DbUpdateException /* ex */)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return new ObjectResult(hashValue);
        }

        [HttpPost]
        public IActionResult SaveForm([FromBody]FormString theFormString)
        {
            var hashValue = Convert
                .ToString(BitConverter.ToInt64(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(
                theFormString.name + "__" + theFormString.stuId)), 0), 16)
                .ToUpper();

            var theSavedForm = new SavedForm
            {
                theForm = theFormString,
                savedTime = DateTime.Now
            };

            theSavedForm.token = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: theSavedForm.savedTime.ToString("o"),
                salt: Encoding.UTF8.GetBytes(hashValue),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var possibleRepeatForm = new SavedForm();

            if (!_memoryCache.TryGetValue(hashValue, out possibleRepeatForm))
            {
                _memoryCache.Set(hashValue, theSavedForm, TimeSpan.FromMinutes(30));
            }

            HttpContext.Session.SetString("name", theSavedForm.theForm.name);
            HttpContext.Session.SetString("stuId", theSavedForm.theForm.stuId);
            HttpContext.Session.SetString("savedHashValue", hashValue);

            return new ObjectResult(hashValue);
        }

        [HttpGet]
        public IActionResult GetSavedForm()
        {
            var inputName = HttpContext.Session.GetString("name");
            var inputId = HttpContext.Session.GetString("stuId");
            var inputSavedHashValue = HttpContext.Session.GetString("savedHashValue");
            var theSavedForm = new SavedForm();

            if (_memoryCache.TryGetValue(inputSavedHashValue, out theSavedForm))
            {
                if (theSavedForm.theForm.name == inputName &&
                    theSavedForm.theForm.stuId == inputId &&
                    theSavedForm.token == Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: theSavedForm.savedTime.ToString("o"),
                        salt: Encoding.UTF8.GetBytes(inputSavedHashValue),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8)))
                {
                    _memoryCache.Remove(inputSavedHashValue);
                    return new ObjectResult(theSavedForm.theForm);
                }
            }

            return new ObjectResult("");
        }
    }
}