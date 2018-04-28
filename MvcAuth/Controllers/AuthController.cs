using MvcAuth.CustomLibraries;
using MvcAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MvcAuth.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var db = new MainDbContext();
            var model = db.Lists.Where(m => m.Public == "Y").ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new MainDbContext())
            {
                var user = db.Users.Where(u => u.Email == model.Email).ToList().FirstOrDefault();
                
                if (user != null)
                {
                    var password = Cryptography.Decrypt(user.Password);

                    if (password == model.Password)
                    {
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Country, user.Country)
                        },
                        "ApplicationCookie");

                        Request.GetOwinContext().Authentication.SignIn(identity);

                        return RedirectToAction("index", "home");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid email or password");

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(Users model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more fields have been");
                return View();
            }

            using (var db = new MainDbContext())
            {
                var encryptedPassword = Cryptography.Encrypt(model.Password);
                var user = db.Users.Create();

                user.Email = model.Email;
                user.Password = encryptedPassword;
                user.Name = model.Name;
                user.Country = model.Country;

                db.Users.Add(user);
                db.SaveChanges();
            }

            return View();
        }
    }
}