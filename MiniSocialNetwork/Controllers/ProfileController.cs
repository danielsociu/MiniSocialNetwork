using MiniSocialNetwork.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int _perPage = 10;

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
                           orderby profile.FullName
                           select profile;

            var friends = from friend in db.Friends
                          orderby friend.CreatedAt
                          select friend;

            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var totalItems = profiles.Count();

            var currentProfile = Convert.ToInt32(Request.Params.Get("profile"));
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProfiles = profiles.Skip(offset).Take(this._perPage);

            ViewBag.Total = totalItems;
            ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Profiles = paginatedProfiles;
            ViewBag.Friends = friends;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            
            return View();
        }

        [ActionName("Search")]
        public ActionResult SearchProfile()
        {
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var searchedString = "";
            if (Request.Params.Get("search") != null)
            {
                searchedString = Request.Params.Get("search").Trim();
                //System.Diagnostics.Debug.WriteLine(searchedString);
            }
            var query = (from profile in db.Profiles
                         join user in db.Users on profile.UserId equals user.Id
                         where (profile.FullName.ToLower().Contains(searchedString)
                                 || user.Email.ToLower().Contains(searchedString))
                             && profile.Private == false
                         orderby profile.FullName
                         select profile);

            var totalItems = query.Count();

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProfiles = query.Skip(offset).Take(this._perPage);

            ViewBag.Total = totalItems;
            ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.SearchedString = searchedString;
            ViewBag.SearchedProfiles = paginatedProfiles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
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
            if (profileUser != null)
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

        [ActionName("New")]
        [HttpPost]
        public ActionResult CreateProfile(Profile profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loggedUser = User.Identity.GetUserId();
                    profile.UserId = loggedUser;
                    profile.FullName = profile.FirstName + ' ' + profile.LastName;
                    if (profile.ProfilePictureUrl == null)
                    {
                        profile.ProfilePictureUrl = "https://icon-library.com/images/default-user-icon/default-user-icon-13.jpg";
                    }
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "You successfully created the profile!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(profile);
                }

            }
            catch (Exception e)
            {
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

        [ActionName("Edit")]
        [HttpPut]
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
                        if (profile.ProfilePictureUrl == null)
                        {
                            profileUser.ProfilePictureUrl = "https://icon-library.com/images/default-user-icon/default-user-icon-13.jpg";
                        }
                        db.SaveChanges();
                        TempData["message"] = "You successfully updated your profile!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot update your profile";
                    return View(profile);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(profile);
            }
        }
      
        [ActionName("AddFriend")]
        [HttpPost]
        public ActionResult AddFriend(FormCollection formData)
        {
            string currentUser = User.Identity.GetUserId();
            string friendToAdd = formData.Get("UserId");
            System.Diagnostics.Debug.WriteLine(friendToAdd);

            try
            {
                Friend friendship = new Friend();
                friendship.User1Id = currentUser;
                friendship.User2Id = friendToAdd;
                friendship.Accepted = false;
                friendship.CreatedAt = DateTime.Now;

                db.Friends.Add(friendship);
                db.SaveChanges();
                TempData["message"] = "You successfully add your friend!";
                    
                return RedirectToAction("Index");
                 
            }
            catch (Exception e)
            {
                TempData["message"] = "Cannot add your friend!";
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return RedirectToAction("Index");
            }
            
        }

    }
}