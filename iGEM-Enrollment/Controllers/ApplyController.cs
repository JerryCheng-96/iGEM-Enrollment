using System;
using System.Collections.Generic;
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

namespace iGEM_Enrollment.Controllers
{
    public class ApplyController : Controller
    {

        private List<SavedForm> savedForms = new List<SavedForm>();

        private readonly ApplyContext _context;

        public ApplyController(ApplyContext applyContext)
        {
            _context = applyContext;
        }

        public IActionResult Form()
        {
            if (HttpContext.Session.GetString("savedHashValue") != "")
            {
                
            }

            ViewData["inputName"] = HttpContext.Session.GetString("name");
            ViewData["inputId"] = HttpContext.Session.GetString("stuId");

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
            if (HttpContext.Session.GetString("savedHashValue") != "")
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
                HttpContext.Session.SetString("isExist", "Yes");
            }
            catch (InvalidOperationException e)
            {
                ViewData["isExist"] = "No";
                HttpContext.Session.SetString("isExist", "No");
            }

            HttpContext.Session.SetString("name", name);
            HttpContext.Session.SetString("stuId", stuId);

            return View();
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
                    salt: hashBytes,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8))
            };

            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(appliForm);
                    //_context.Add(applicant);
                    //_context.Add(user);
                    //await _context.SaveChangesAsync();
                    //return new ObjectResult("");
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

            savedForms.Add(theSavedForm);

            HttpContext.Session.SetString("name", theSavedForm.theForm.name);
            HttpContext.Session.SetString("stuId", theSavedForm.theForm.stuId);
            HttpContext.Session.SetString("savedHashValue", hashValue);
            
            return new ObjectResult(hashValue);
        }
    }
}