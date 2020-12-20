﻿using System;
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
        private int _perPage = 10;
        // GET: Group
        public ActionResult Index()
        {
            var loggedUser = User.Identity.GetUserId();
            var groups = db.Groups.OrderByDescending(g => g.CreatedAt);
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var totalItems = groups.Count();
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedGroups = groups.Skip(offset).Take(this._perPage);

            ViewBag.Total = totalItems;
            ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Groups = paginatedGroups;
            ViewBag.CurrentUserId = loggedUser;
            ViewBag.JoinedGroups = getGroups();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        [ActionName("View")]
        public ActionResult ViewGroup(int id)
        {
            string loggedUser = User.Identity.GetUserId();
            string fullName = (from profile in db.Profiles
                            where profile.UserId == loggedUser
                            select profile.FullName).FirstOrDefault();
            Group group = db.Groups.Find(id);
            IEnumerable<int> joinedGroups = getGroups();

            ViewBag.CurrentUserId = loggedUser;
            ViewBag.JoinedGroups = joinedGroups;
            ViewBag.CurrentGroup = id;
            ViewBag.JoinedUsers = getUsers(id);
            ViewBag.FullName = fullName;
            if (joinedGroups.Contains(id))
            {
                ViewBag.ShowMessages = 1;
                ViewBag.GroupMessages = (from message in db.Messages
                                        where message.GroupId == id
                                        orderby message.CreatedAt 
                                        select message);
            } else
            {
                ViewBag.ShowMessages = 0;
            }
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

        [ActionName("New")]
        [HttpPost]
        public ActionResult CreateGroup(Group group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loggedUser = User.Identity.GetUserId();
                    group.CreatorId = loggedUser;
                    group.CreatedAt = DateTime.Now;
                    db.Groups.Add(group);
                    GroupUsers joined = new GroupUsers();
                    joined.UserId = loggedUser;
                    joined.GroupId = group.GroupId;
                    db.GroupUsers.Add(joined);
                    db.SaveChanges();
                    TempData["message"] = "You successfully created the group!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(group);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(group);
            }
        }
        [ActionName("Edit")]
        public ActionResult EditGroup(int id)
        {
            var loggedUser = User.Identity.GetUserId();
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                TempData["message"] = "No such group!";
                return RedirectToAction("Index");
            }
            if (group.CreatorId != loggedUser)
            {
                TempData["message"] = "You are not the creator of the group!";
                return RedirectToAction("Index");
            }
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(group);
        }

        [ActionName("Edit")]
        [HttpPut]
        public ActionResult EditGroup(int id, Group requestGroup)
        {
            try
            {
                var loggedUser = User.Identity.GetUserId();
                Group group = db.Groups.Find(id);
                if (group == null)
                {
                    TempData["message"] = "No such group!";
                    return RedirectToAction("Index");
                }
                if (group.CreatorId != loggedUser)
                {
                    TempData["message"] = "You are not the creator of the group!";
                    return RedirectToAction("Index");
                }
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(group))
                    {
                        group.Name = requestGroup.Name;
                        group.Details = requestGroup.Details;
                        // group.CreatorId = loggedUser;
                        // group.CreatedAt = requestGroup.CreatedAt;
                        db.SaveChanges();
                        TempData["message"] = "You successfully updated the group!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot update your profile";
                    return View(requestGroup);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(requestGroup);
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
        [NonAction]
        private IEnumerable<Profile> getUsers(int groupId)
        {
            var loggedUser = User.Identity.GetUserId();
            var joinedUsers = from users in db.GroupUsers
                              join profile in db.Profiles on users.UserId equals profile.UserId
                              where users.GroupId == groupId
                              select profile;
            return joinedUsers.ToList();
        }
    }
}