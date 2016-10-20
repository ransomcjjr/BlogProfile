using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CJRProfileBlog.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace CJRProfileBlog.Controllers
   
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogIndex
        public ActionResult blogindex(int? page)
        {
            return View(db.Posts.ToList());
        }

        // GET: blogmain
        public ActionResult blogmain()
        {
            return View();
        }

        // GET: BlogPosts
        public ActionResult Index(int? page, string query)
        {
            var result = db.Posts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            { 
                result = result.Where(p => p.Body.Contains(query))
                .Union(db.Posts.Where(p => p.Title.Contains(query)))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Body.Contains(query))));
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var qpost = result.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize);
            return View(qpost);
            
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()

        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL")] BlogPost blogPost, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image == null)
                {
                    blogPost.MediaURL = "~/img/blog/default.jpg";
                }
                else
                {
                 //image validation
                    if (ImageUploadValidator.IsWebFriendlyImage(Image))
                 {
                    var filename = Path.GetFileName(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/img/blog/"), filename));
                    blogPost.MediaURL = "~/img/blog/" + filename;
                    }
                }
                //Slug Utility
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (string.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View("blogPost");
                }

                if(db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }
                blogPost.Updated = DateTime.Now;
                blogPost.Created = DateTime.Now;
                blogPost.Slug = Slug;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,Body,MediaURL,Slug")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (string.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View("blogPost");
                }

              // string val = Convert.ToString(Request.Params["Slug"]);

                if (blogPost.Slug != Slug)
                {
                    if (db.Posts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);
                    }
                }


                blogPost.Updated = DateTime.Now;
                blogPost.Slug = Slug;
                //db.Posts.Attach(blogPost);
                //db.Entry(blogPost).Property("Title").IsModified = true;
                //db.Entry(blogPost).Property("Updated").IsModified = true;
                //db.Entry(blogPost).Property("Body").IsModified = true;
                //db.Entry(blogPost).Property("MediaURL").IsModified = true;

                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
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
