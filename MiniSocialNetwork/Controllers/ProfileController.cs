using MiniSocialNetwork.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profile
        public ActionResult Index()
        {
            var profiles = (from user in db.Users
                           join profile in db.Profiles on user.Id equals profile.UserId
                           select new {
                               profile.ProfileId,
                               profile.ProfilePictureUrl,
                               profile.Status,
                               profile.FirstName,
                               profile.LastName,
                               profile.Location,
                               profile.Biography,
                               profile.Private,
                               profile.BirthDate,
                               user.UserName,
                               user.Email,
                               user.PhoneNumber,
                           }).ToList();
            ViewBag.Profiles = profiles;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [ActionName("View")]
        public ActionResult ViewProfile(int id)
        {
            var profileUser = (from profile in db.Profiles
                              where profile.ProfileId == id
                              select new {
                                   profile.ProfileId,
                                   profile.ProfilePictureUrl,
                                   profile.Status,
                                   profile.FirstName,
                                   profile.LastName,
                                   profile.Location,
                                   profile.Biography,
                                   profile.Private,
                                   profile.BirthDate,
                              }).ToList();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            if (profileUser.Count < 1)
            {
                TempData["message"] = "Profile doesn't exist";
                return RedirectToAction("Index");
            }
            ViewBag.Profile = profileUser[0];
            return View();
        }

        [ActionName("New")]
        public ActionResult CreateProfile()
        {
            var loggedUser = User.Identity.GetUserId();
            var profileUser = db.Profiles.Find(loggedUser);
            if ( profileUser != null)
            {
                TempData["message"] = "You already have a profile!";
                return RedirectToAction("Edit");
            }
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

        [ActionName("Edit")]
        public ActionResult EditProfile()
        {
            var loggedUser = User.Identity.GetUserId();
            var profileUser = db.Profiles.Find(loggedUser);
            if (profileUser == null)
            {
                TempData["message"] = "You need to first create a profile!";
                return RedirectToAction("New");
            }
            ViewBag.Profile = profileUser;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(profileUser);
        }

        [ActionName("Edit")][HttpPut]
        public ActionResult EditProfile(Profile profile)
        {
            try
            {
                var loggedUser = User.Identity.GetUserId();
                Profile profileUser = db.Profiles.Find(loggedUser);
                if (profileUser == null)
                {
                    TempData["message"] = "You need to first create a profile!";
                    return RedirectToAction("New");
                }
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(profileUser))
                    {
                        profileUser.ProfilePictureUrl = profile.ProfilePictureUrl;
                        profileUser.Status = profile.Status;
                        profileUser.Location = profile.Location;
                        profileUser.Biography = profile.Biography;
                        profileUser.Private = profile.Private;
                        profileUser.BirthDate = profile.BirthDate;
                        db.SaveChanges();
                        TempData["message"] = "You successfully updated your profile!";
                    }
                    return RedirectToAction("Index");
                } else
                {
                    TempData["message"] = "Cannot update your profile";
                    return View(profile);
                }

            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(profile);
            }
        }
    }
}