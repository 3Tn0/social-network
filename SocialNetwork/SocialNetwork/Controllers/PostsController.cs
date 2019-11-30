using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);

            PostDetailViewModel FullPost = new PostDetailViewModel();

            var postinf = (from p in db.Posts
                           join e in db.Editors on p.RightsId equals e.EditorRightsId
                           join u in db.Users on e.UserId.ToString() equals u.Id
                           join c in db.Communities on p.CommunityId equals c.CommunityId
                           where p.PostId == id
                           select new
                           {
                               u.FirstName,
                               u.LastName,
                               c.Name,
                               u.Id
                           }).First();

            FullPost.AuthorId = Guid.Parse(postinf.Id);

            FullPost.Post = new PostInfo { PostId = post.PostId, Text = post.Text, CommunityId = post.CommunityId, CreationDate = post.CreationDate, AuthorFN = postinf.FirstName, AuthorLN = postinf.LastName, CommunityName = postinf.Name };


            var comments = (from p in db.Posts
                            join com in db.Comments on p.PostId equals com.PostId
                            join u in db.Users on com.UserId.ToString() equals u.Id
                            where p.PostId == id
                            orderby com.CommentDate descending
                            select new
                            {
                                com.CommentId,
                                com.CommentDate,
                                com.Text,
                                u.LastName,
                                u.FirstName,
                                u.Id
                            }).ToList();

            FullPost.Comments = new List<CommentInfo>();

            foreach (var item in comments)
            {
                CommentInfo comment = new CommentInfo { CommentDate = item.CommentDate, CommentId = item.CommentId, CommentText = item.Text, UserFn = item.FirstName, UserLn = item.LastName, UserId = Guid.Parse(item.Id) };
                FullPost.Comments.Add(comment);
            }


            var userId = User.Identity.GetUserId();

            FullPost.isAuthor = false;

            if (postinf.Id == userId)
            {
                FullPost.isAuthor = true;
            }

            FullPost.isAdmin = false;

            var Admin = (from c in db.Communities
                         where c.CommunityId == post.CommunityId
                         select new { id = c.UserId }).First();

            if (userId == Admin.id.ToString())
            {
                FullPost.isAdmin = true;
            }


            if (post == null)
            {
                return HttpNotFound();
            }
            return View(FullPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "CommentId,CommentDate,Text")] Comment comment, Guid id)
        {
            if (ModelState.IsValid)
            {
                comment.CommentId = Guid.NewGuid();
                comment.CommentDate = DateTime.Now;
                comment.PostId = id;
                comment.UserId = Guid.Parse(User.Identity.GetUserId());

                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", new { id });
            }

            return RedirectToAction("Details", "Posts", new { id });
        }

        // GET: Posts/Create
        public ActionResult Create(Guid ComId)
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,CreationDate,Text")] Post post, Guid ComId)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                post.PostId = Guid.NewGuid();
                post.CommunityId = ComId;
                post.CreationDate = DateTime.Now;
                post.RightsId = (from R in db.Editors
                                 where R.CommunityId == ComId && R.UserId == userId && R.CancellationDate == null
                                 select new
                                 {
                                     RId = R.EditorRightsId
                                 }).First().RId;
                db.Posts.Add(post);
                db.SaveChanges();

                return RedirectToAction("Community", "Communities", new { ComId });
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,CreationDate,Text,RightsId,CommunityId")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
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
