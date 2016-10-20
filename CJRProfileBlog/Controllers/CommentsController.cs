using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CJRProfileBlog.Models;
using Microsoft.AspNet.Identity;

namespace CJRProfileBlog.Controllers
{
    [RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Post);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }

        // GET: Comments/Create
        [Authorize]
        public ActionResult Create(int id)
        {
            TempData["PostId"] = id;
            ViewBag.PostTitle = db.Posts.FirstOrDefault(p => p.Id == id);
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PostId,Body")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                var loguser = db.Users.Find(User.Identity.GetUserId());
                comments.PostId = (int)TempData["PostId"];
                comments.Created = DateTime.Now;
                comments.Updated = DateTime.Now;
                comments.AuthorId = loguser.Id;
                db.Comments.Add(comments);
                db.SaveChanges();

                var blog = db.Posts.FirstOrDefault(b => b.Id == comments.PostId);
                if (blog != null)
                {
                    return RedirectToAction("Details", "BlogPosts", new { slug = blog.Slug });
                }
                return RedirectToAction("Index","BlogPosts");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comments.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comments.PostId);
            return View(comments);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Edit(int? id)
        {
            var loguser = db.Users.Find(User.Identity.GetUserId());
            var comment = db.Comments.FirstOrDefault(p => p.Id == id);
            ViewBag.PostTitle = comment.Post;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comments.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comments.PostId);
            return View(comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,Body")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                var loguser = db.Users.Find(User.Identity.GetUserId());
                db.Entry(comments).State = EntityState.Modified;
                comments.Updated = DateTime.Now;


                db.Comments.Attach(comments);
                db.Entry(comments).Property("Updated").IsModified = true;
                db.Entry(comments).Property("Body").IsModified = true;

                db.SaveChanges();

                var blog = db.Posts.FirstOrDefault(b => b.Id == comments.PostId);
                if (blog != null)
                {
                    return RedirectToAction("Details", "BlogPosts", new { slug = blog.Slug });
                }
                return RedirectToAction("Index", "BlogPosts");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comments.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comments.PostId);
            return View(comments);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
          
           var comment = db.Comments.FirstOrDefault(p => p.Id == id);
            ViewBag.PostTitle = comment.Post;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }

            return View(comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comments comments = db.Comments.Find(id);
            db.Comments.Remove(comments);
            db.SaveChanges();

            var blog = db.Posts.FirstOrDefault(b => b.Id == comments.PostId);
            if (blog != null)
            {
                return RedirectToAction("Details", "BlogPosts", new { slug = blog.Slug });
            }
            return RedirectToAction("Index", "BlogPosts");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
