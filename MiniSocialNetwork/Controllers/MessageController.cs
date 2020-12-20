using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;
using MiniSocialNetwork.Models;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Message
        public ActionResult Index()
        {
            var messages = from message in db.Messages
                         select message;
            ViewBag.Messages = messages;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        
        public void New(int id, Message message)
        {
            var loggedUser = User.Identity.GetUserId();
            var groups = getGroups();
            string fullname = (from profile in db.Profiles
                              where profile.UserId == loggedUser
                              select profile.FullName).SingleOrDefault();
            if (!groups.Contains(id))
            {
                TempData["message"] = "You are not part of this group!";
            } else 
            {
                // Socket.io implementation
                if (ModelState.IsValid)
                {
                    message.CreatedAt = DateTime.Now;
                    message.GroupId = id;
                    message.UserId = loggedUser;
                    message.Nickname = fullname;
                } else
                {
                    TempData["message"] = "Couldn't send message";
                }

            }
        }

        [NonAction]
        private IEnumerable<int> getGroups()
        {
            var loggedUser = User.Identity.GetUserId();
            var joinedGroups = from groups in db.GroupUsers
                               where groups.UserId == loggedUser
                               select groups.GroupId;
            return joinedGroups.ToList();
        }
    }
}