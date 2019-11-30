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
    public class CommunitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Communities
        public ActionResult Index()
        {
            return View(db.Communities.ToList());
        }

        // GET: Communities/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Communities communities = db.Communities.Find(id);
            if (communities == null)
            {
                return HttpNotFound();
            }
            return View(communities);
        }

        // GET: Communities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Communities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommunityId,CreationDate,Name,UserId")] Communities communities)
        {
            if (ModelState.IsValid)
            {
                communities.CommunityId = Guid.NewGuid();
                db.Communities.Add(communities);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(communities);
        }


        // Страница пользователя

        public ActionResult Userpage()
        {   //Добавить
            //using ()
            var db = new ApplicationDbContext();

            var userId = User.Identity.GetUserId();


            UserPageViewModel Page = new UserPageViewModel();

            Page.User = db.Users
                        .Where(c => c.Id == userId)
                        .Select(c => c).First();


            Guid id = Guid.Parse(userId);

            var news = (from sub in db.Subscriptions
                        join p in db.Posts on sub.CommunityId equals p.CommunityId
                        join c in db.Communities on sub.CommunityId equals c.CommunityId
                        join r in db.Editors on p.RightsId equals r.EditorRightsId
                        join u in db.Users on userId equals u.Id
                        where sub.SubscriptionCancelationDate == null &&
                      sub.UserId == id
                        orderby p.CreationDate descending
                        select new
                        {
                            p.PostId,
                            p.CreationDate,
                            p.Text,
                            sub.CommunityId,
                            c.Name,
                            u.LastName,
                            u.FirstName
                        }).ToList();


            Page.News = new List<PostInfo>();

            foreach (var item in news)
            {
                PostInfo one = new PostInfo { PostId = item.PostId, AuthorFN = item.FirstName, AuthorLN = item.LastName, CommunityId = item.CommunityId, CommunityName = item.Name, CreationDate = item.CreationDate, Text = item.Text };
                Page.News.Add(one);
            }

            return View(Page);
        }


        //Подписки

        public ActionResult Subscriptions(int page = 1)
        {
            var db = new ApplicationDbContext();
            var userId = Guid.Parse(User.Identity.GetUserId());


            List<Communities> communities = new List<Communities>();


            var select = from Sub in db.Subscriptions
                         join Com in db.Communities on Sub.CommunityId equals Com.CommunityId
                         where Sub.SubscriptionCancelationDate == null &&
                         Sub.UserId == userId
                         select new
                         {
                             Id = Com.CommunityId,
                             Name = Com.Name,
                             Date = Com.CreationDate,
                             User = Com.UserId
                         };

            foreach (var item in select)
            {
                var comunity = new Communities { CommunityId = item.Id, UserId = item.User, CreationDate = item.Date, Name = item.Name };
                communities.Add(comunity);
            }

            int pageSize = 10; // количество объектов на страницу
            IEnumerable<Communities> phonesPerPages = communities.Skip((page - 1) * pageSize).Take(pageSize);
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = communities.Count };
            IndexViewModel1 ivm = new IndexViewModel1 { PageInfo = pageInfo, Communities = phonesPerPages };
            return View(ivm);
        }


        //Администрирование
        public ActionResult Administration(int page = 1)
        {
            var db = new ApplicationDbContext();
            var userId = Guid.Parse(User.Identity.GetUserId());


            //List<Communities> communities = (db.Communities
            //            .Where(c => c.UserId == userId)
            //            .Select(c => c)).ToList();

            List<Communities> communities = (from c in db.Communities
                                             join e in db.Editors on c.CommunityId equals e.CommunityId
                                             where (e.UserId == userId && e.CancellationDate == null) || (c.UserId == userId)
                                             select c).Distinct().ToList();


            int pageSize = 10; // количество объектов на страницу
            IEnumerable<Communities> phonesPerPages = communities.Skip((page - 1) * pageSize).Take(pageSize);
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = communities.Count };
            IndexViewModel1 ivm = new IndexViewModel1 { PageInfo = pageInfo, Communities = phonesPerPages };
            return View(ivm);
        }


        [HttpGet]
        public ActionResult All()
        {

            using (var db = new ApplicationDbContext())
            {



                List<Communities> Coms = new List<Communities>();


                Coms = (from c in db.Communities
                        select c).ToList();

                return View(Coms);
            }


        }


        [HttpPost]
        public ActionResult All(string Filter)
        {
            using (var db = new ApplicationDbContext())
            {

                //List<Communities> Coms = new List<Communities>();


                var Coms = (from c in db.Communities
                            select c);

                if (!String.IsNullOrEmpty(Filter))
                {
                    Coms = Coms.Where(s => s.Name.Contains(Filter));
                }

                return View(Coms.ToList());


            }
        }


        // GET: Communities/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Communities communities = db.Communities.Find(id);
            if (communities == null)
            {
                return HttpNotFound();
            }
            return View(communities);
        }

        // POST: Communities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommunityId,CreationDate,Name,UserId")] Communities communities)
        {
            if (ModelState.IsValid)
            {
                db.Entry(communities).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(communities);
        }

        // GET: Communities/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Communities communities = db.Communities.Find(id);
            if (communities == null)
            {
                return HttpNotFound();
            }
            return View(communities);
        }

        // POST: Communities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Communities communities = db.Communities.Find(id);
            db.Communities.Remove(communities);
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
