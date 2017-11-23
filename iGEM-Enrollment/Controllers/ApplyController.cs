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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;

namespace iGEM_Enrollment.Controllers
{
    public class ApplyController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ApplyContext _context;
        private readonly IHostingEnvironment _hosting;

        public ApplyController(ApplyContext applyContext, IMemoryCache memoryCache, IHostingEnvironment env)
        {
            _context = applyContext;
            _memoryCache = memoryCache;
            _hosting = env;
        }

        [HttpGet]
        public IActionResult Error(int errCode)
        {
            return View();
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        public async Task<IActionResult> Delete()
        {
            if (HttpContext.Session.GetString("eHashValue") != null &&
                HttpContext.Session.GetString("name") != null &&
                HttpContext.Session.GetString("stuId") != null)
            {
                var theApplicant = await GetExistApplicant();

                List<User> Users = _context.UserData
                                                .AsNoTracking()
                                                .OrderBy(u => u.stuId)
                                                .ToList();

                try
                {
                    User theUser = Users.Single(u => (u.stuId == theApplicant.stuId && u.name == theApplicant.name));
                    _context.UserData.Remove(theUser);
                }
                catch (Exception e)
                {
                }
                _context.AppliForm.Remove(theApplicant.appliForm);
                _context.Applicant.Remove(theApplicant);
                _context.SaveChanges();

                FileInfo photoFile = new FileInfo(_hosting.WebRootPath + $@"/uploads/photos/{theApplicant.photoFileName}");

                if (photoFile.Exists)
                {
                    photoFile.Delete();
                }

                try
                {
                    FileInfo appendixFile = new FileInfo(_hosting.WebRootPath + $@"/uploads/appendices/{theApplicant.appliForm.appendixFileName}");
                    if (appendixFile.Exists)
                    {
                        appendixFile.Delete();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            HttpContext.Session.Clear();
            return View();
        }

        [HttpGet]
        public IActionResult ShowFormByHash(String hashValue)
        {
            HttpContext.Session.SetString("isExist", "Yes");
            HttpContext.Session.SetString("eHashValue", hashValue);

            return RedirectToAction("ShowForm");

        }

        [HttpPost]
        public IActionResult UploadFileToCache()
        {
            try
            {
                var file = Request.Form.Files.Single();
                var hashValue = DateTime.Now.ToFileTimeUtc() + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                Console.WriteLine(_hosting.WebRootPath + $@"/cached/{hashValue}");
                FileStream fs = new FileStream(_hosting.WebRootPath + $@"/cached/{hashValue}", FileMode.Create);
                file.CopyTo(fs);
                fs.Flush();
                return new ObjectResult(hashValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(_hosting.WebRootPath);
            }
            return new ObjectResult("");
        }

        public IActionResult ShowForm(String hashValue)
        {
            if (hashValue != null)
            {
                ViewData["isExist"] = "Yes";
                HttpContext.Session.SetString("eHashValue", hashValue);
                return View();
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
            Applicant theApplicant = await GetExistApplicant();
            if (theApplicant != null)
            {
                return new ObjectResult(new FormString(theApplicant));
            }
            return new ObjectResult("");
        }


        public async Task<Applicant> GetExistApplicant()
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
                return theApplicant;
            }
            catch (Exception e)
            {
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm([FromBody]FormString theFormString)
        {
            try
            {
                FileInfo photoFile = new FileInfo(_hosting.WebRootPath + $@"/cached/{theFormString.photoFileName}");
                FileInfo appendixFile = new FileInfo(_hosting.WebRootPath + $@"/cached/{theFormString.appendixFileName}");

                if (photoFile.Exists)
                {
                    photoFile.MoveTo(_hosting.WebRootPath + @"/uploads/photos/" + theFormString.photoFileName);
                }
                else
                {
                    theFormString.photoFileName = "";
                }

                if (appendixFile.Exists)
                {
                    appendixFile.MoveTo(_hosting.WebRootPath + @"/uploads/appendices/" + theFormString.appendixFileName);
                }
                else
                {
                    theFormString.appendixFileName = "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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
                    _context.SaveChanges();
                    HttpContext.Session.SetString("hashValue", hashValue);
                    return new ObjectResult("");
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

        public IActionResult SubmitFormSucceeded()
        {
            ViewData["hashValue"] = HttpContext.Session.GetString("hashValue");
            HttpContext.Session.Remove("hashValue");
            return View();
        }

        [HttpPost]
        public IActionResult SaveForm([FromBody]FormString theFormString)
        {
            var hashValue = Convert
                .ToString(BitConverter.ToInt64(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(
                theFormString.name + "__" + theFormString.stuId + "__" + DateTime.Now.ToString("o"))), 0), 16)
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

            HttpContext.Session.Clear();

            HttpContext.Session.SetString("name", theSavedForm.theForm.name);
            HttpContext.Session.SetString("stuId", theSavedForm.theForm.stuId);
            HttpContext.Session.SetString("savedHashValue", hashValue);

            return new ObjectResult(DateTime.Now.ToString("G"));
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
                    //_memoryCache.Remove(inputSavedHashValue);
                    return new ObjectResult(theSavedForm.theForm);
                }
            }

            return new ObjectResult("");
        }
    }
}