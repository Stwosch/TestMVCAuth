using MvcAuth.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MvcAuth.Controllers
{
    public class HomeController : Controller
    {
        private static string datePosted;
        private static string timePosted;

        [HttpGet]
        public ActionResult Index()
        {
            var db = new MainDbContext();
            return View(db.Lists.ToList());
        }

        [HttpPost]
        public ActionResult Index(Lists model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MainDbContext())
                {
                    var email = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Email).Value;
                    var user = db.Users.Where(u => u.Email == email).ToList().FirstOrDefault();
                    var dateToday = DateTime.Now.ToString("M/dd/yyyy");
                    var timeToday = DateTime.Now.ToString("h:mm:ss tt");
                    var check_public = Request.Form["check_public"];
                    var details = Request.Form["new_item"];
                    var list = db.Lists.Create();

                    list.Details = details;
                    list.Date_Posted = dateToday;
                    list.Time_Posted = timeToday;
                    list.User_Id = user.Id;
                    if (check_public != null)
                    {
                        list.Public = "Y";
                    }
                    else
                    {
                        list.Public = "N";
                    }

                    db.Lists.Add(list);
                    db.SaveChanges();
                }
            }
            else
            {
                ModelState.AddModelError("", "Incorrect format has been placed");
            }

            var db2 = new MainDbContext();
            return View(db2.Lists.ToList());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new MainDbContext();
            var model = db.Lists.Find(id);
            if (model != null)
            {
                datePosted = model.Date_Posted;
                timePosted = model.Time_Posted;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(Lists model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MainDbContext())
                {
                    var dateToday = DateTime.Now.ToString("M/dd/yyyy");
                    var timeToday = DateTime.Now.ToString("h:mm:ss tt");
                    var check_public = Request.Form["check_public"];
                    var details = Request.Form["new_item"];

                    model.Details = details;
                    model.Date_Edited = dateToday;
                    model.Time_Edited = timeToday;
                    model.Date_Posted = datePosted;
                    model.Time_Posted = timePosted;
                    if (check_public != null)
                    {
                        model.Public = "Y";
                    }
                    else
                    {
                        model.Public = "N";
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Incorrect format has been placed");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var db = new MainDbContext();
            var model = db.Lists.Find(id);
            if (model != null)
            {
                db.Lists.Remove(model);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}