using Learning.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Learning.Controllers
{
    public class AccountController : Controller
    {
        LearningDbEntities db = new LearningDbEntities();

        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserModel user)
        {
            try
            {
                User dbUser = new User() { Full_Name = user.Full_Name, Email = user.Email, Password = user.Password };

                if(db.Users.Where(x => x.Email.Equals(user.Email)).Count() > 0)
                {
                    ViewBag.msg = "User already exist";
                }
                else
                {
                    db.Users.Add(dbUser);
                    db.SaveChanges();

                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel user)
        {
            try
            {
                if(db.Users.Where(x=>x.Email.Equals(user.Email) && x.Password.Equals(user.Password)).Count() > 0)
                {
                    ViewBag.msg = "Login Successfull";
                }
                else
                {
                    ViewBag.msg = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.StackTrace;
            }
            return View();
        }
    }
}