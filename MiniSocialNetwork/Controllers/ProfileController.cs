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
            //var profiles = (from user in db.Users
            //               join profile in db.Profiles on user.Id equals profile.UserId
            //               select new {
            //                   profile.ProfileId,
            //                   profile.ProfilePictureUrl,
            //                   profile.Status,
            //                   profile.FirstName,
            //                   profile.LastName,
            //                   profile.Location,
            //                   profile.Biography,
            //                   profile.Private,
            //                   profile.BirthDate,
            //                   user.UserName,
            //                   user.Email,
            //                   user.PhoneNumber,
            //               }).ToList();
            var profiles = from profile in db.Profiles
                           select profile;
            ViewBag.Profiles = profiles;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [ActionName("Search")]
        public ActionResult SearchProfile(string searchedString)
        {
            var query = (from profile in db.Profiles
                         where profile.FullName.ToUpper().Contains(searchedString.ToUpper())
                             && profile.Private == false
                         select profile);
                        // {
                        //     profile.ProfileId,
                        //     profile.FullName,
                        //     profile.ProfilePictureUrl,
                        // }).ToList();
            ViewBag.SearchedString = searchedString;
            ViewBag.SearchedProfiles = query;
            return View();
        }

        [ActionName("View")]
        public ActionResult ViewProfile(int id)
        {
            Profile profileUser = (from profile in db.Profiles
                               where profile.ProfileId == id
                               select profile).SingleOrDefault();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            if (profileUser == null)
            {
                TempData["message"] = "Profile doesn't exist";
                return RedirectToAction("Index");
            }
            return View(profileUser);
        }

        [ActionName("New")]
        public ActionResult CreateProfile()
        {
            var loggedUser = User.Identity.GetUserId();
            Profile profileUser = (from profile in db.Profiles
                               where profile.UserId == loggedUser
                               select profile).SingleOrDefault();
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
                    profile.FullName = profile.FirstName + ' ' + profile.LastName;
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
            Profile profileUser = (from profile in db.Profiles
                               where profile.UserId == loggedUser
                               select profile).SingleOrDefault();
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
                Profile profileUser = (from profilex in db.Profiles
                                   where profilex.UserId == loggedUser
                                   select profilex).SingleOrDefault();
                if (profileUser == null)
                {
                    TempData["message"] = "You need to first create a profile!";
                    return RedirectToAction("New");
                }
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(profileUser))
                    {
                        profileUser.FirstName = profile.FirstName;
                        profileUser.LastName = profile.LastName;
                        profileUser.FullName = profile.FirstName + ' ' + profile.LastName;
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