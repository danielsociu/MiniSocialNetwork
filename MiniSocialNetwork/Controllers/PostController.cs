using MiniSocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniSocialNetwork.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var posts = from post in db.Posts
                        select post;
            ViewBag.Posts = posts;

            return View();
        }

        public ActionResult Show(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Post pst)
        {
            try
            {
                db.Posts.Add(pst);
                db.SaveChanges();
                TempData["message"] = "Post added!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(pst);
            }
        }

        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
     

            return View(post);
        }

        [HttpPut]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                Post post = db.Posts.Find(id);
                if (TryUpdateModel(post))
                {
                    post.Content = requestPost.Content;
                    db.SaveChanges();
                    TempData["message"] = "Post edited!";
                    return RedirectToAction("Index");
                }

                return View(requestPost);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(requestPost);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            TempData["message"] = "Post deleted!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}