using System;
using MiniSocialNetwork.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Group
        public ActionResult Index()
        {
            var groups = from groupx in db.Messages
                         select groupx;
            ViewBag.Groups = groups;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [ActionName("View")]
        public ActionResult ViewGroup(int id)
        {
            Group group = db.Groups.Find(id);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(group);
        }

        [ActionName("New")]
        public ActionResult CreateGroup()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [ActionName("New")][HttpPost]
        public ActionResult CreateProfile(Profile profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loggedUser = User.Identity.GetUserId();
                    profile.UserId = loggedUser;
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "You successfully created the profile!";
                    return RedirectToAction("Index");
                } else
                {
                    return View(profile);
                }

            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(profile);
            }
        }
    }
}