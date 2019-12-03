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

        //Страница создания сообщества
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(MyCommunitiesViewModels model)
        {
            if (ModelState.IsValid)
            {

                var community = new Communities { CommunityId = Guid.NewGuid(), CreationDate = DateTime.Now, Name = model.Name, UserId = Guid.Parse(User.Identity.GetUserId()) };

                var editor = new Editor { EditorRightsId = Guid.NewGuid(), AppointmentDate = DateTime.Now, CommunityId = community.CommunityId, UserId = Guid.Parse(User.Identity.GetUserId()) };

                var db = new ApplicationDbContext();

                db.Communities.Add(community);
                db.Editors.Add(editor);

                db.SaveChanges();


                return Redirect("/Communities/Administration");
            }
            return Redirect("/Communities/Administration");
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


        public ActionResult Community(Guid ComId)
        {
            var db = new ApplicationDbContext();
            var userId = Guid.Parse(User.Identity.GetUserId());

            CommunityPageViewModel CommunityPage = new CommunityPageViewModel();

            var q = from Com in db.Communities
                    where Com.CommunityId == ComId
                    select new
                    {
                        Name = Com.Name
                    };

            CommunityPage.CommunityName = q.First().Name;

            CommunityPage.CommunityId = ComId;


            var news = (from p in db.Posts
                        join c in db.Communities on p.CommunityId equals c.CommunityId
                        join ed in db.Editors on p.RightsId equals ed.EditorRightsId
                        join u in db.Users on ed.UserId.ToString() equals u.Id
                        where p.CommunityId == ComId
                        select new
                        {
                            p.PostId,
                            p.CreationDate,
                            p.Text,
                            p.CommunityId,
                            c.Name,
                            u.LastName,
                            u.FirstName,
                            u.Id
                        }).ToList();

            CommunityPage.News = new List<PostInfo>();

            foreach (var item in news)
            {
                PostInfo one = new PostInfo { PostId = item.PostId, AuthorFN = item.FirstName, AuthorLN = item.LastName, CommunityId = item.CommunityId, CommunityName = item.Name, CreationDate = item.CreationDate, Text = item.Text, AuthorId = Guid.Parse(item.Id) };
                CommunityPage.News.Add(one);
            }


            var e = from Ed in db.Editors
                    where Ed.CommunityId == ComId &&
                    Ed.CancellationDate == null
                    select new
                    {
                        EdId = Ed.UserId
                    };

            CommunityPage.isEditor = false;

            foreach (var item in e)
            {
                if (item.EdId == Guid.Parse(User.Identity.GetUserId()))
                    CommunityPage.isEditor = true;
            }

            var s = from Sub in db.Subscriptions
                    where Sub.CommunityId == ComId &&
                    Sub.UserId == userId &&
                    Sub.SubscriptionCancelationDate == null
                    select Sub;

            CommunityPage.isSubscriber = false;

            if (s.Count() != 0)
                CommunityPage.isSubscriber = true;


            var isA = from c in db.Communities
                      where c.CommunityId == ComId &&
                      c.UserId == userId
                      select c;

            CommunityPage.isAdmin = false;

            if (isA.Count() != 0)
                CommunityPage.isAdmin = true;


            return View(CommunityPage);
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


        public ActionResult Subscribers(Guid ComId)
        {
            using (var db = new ApplicationDbContext())
            {



                //*************************
                //var Subs = from s in db.Subscriptions
                //           join u in db.Users on s.UserId.ToString() equals u.Id
                //           where s.CommunityId == ComId &&
                //           s.SubscriptionCancelationDate == null
                //           select u;
                //           return View(Subs.ToList());
                //************************




                //var Subs = from s in db.Subscriptions
                //           join u in db.Users on s.UserId.ToString() equals u.Id
                //           join e in db.Editors on u.Id equals e.UserId.ToString() into gj
                //           from sube in gj.DefaultIfEmpty()
                //           where s.CommunityId == ComId &&
                //           s.SubscriptionCancelationDate == null &&
                //           sube.CancellationDate == null
                //           select new
                //           {
                //               u.Id,
                //               u.FirstName,
                //               u.LastName,
                //               sube.
                //           };


                var Subs = (from s in db.Subscriptions
                            join u in db.Users on s.UserId.ToString() equals u.Id
                            where s.CommunityId == ComId &&
                            s.SubscriptionCancelationDate == null
                            select u).ToList();

                List<Subscriber> Subscribers = new List<Subscriber>();

                foreach (var item in Subs)
                {

                    var isE = from e in db.Editors
                              where e.CommunityId == ComId &&
                              e.UserId.ToString() == item.Id &&
                              e.CancellationDate == null
                              select e;
                    if (isE.Count() == 0)
                    {
                        Subscriber Sub = new Subscriber { FirstName = item.FirstName, LastName = item.LastName, isEditor = false, UserId = Guid.Parse(item.Id) };
                        Subscribers.Add(Sub);
                    }
                    else
                    {
                        Subscriber Sub = new Subscriber { FirstName = item.FirstName, LastName = item.LastName, isEditor = true, UserId = Guid.Parse(item.Id) };
                        Subscribers.Add(Sub);
                    }
                }

                Guid Uid = Guid.Parse(User.Identity.GetUserId());

                var isA = from c in db.Communities
                          where c.CommunityId == ComId &&
                          c.UserId == Uid
                          select c;

                if (isA.Count() == 0)
                { ViewBag.IsAdmin = false; }
                else
                { ViewBag.IsAdmin = true; }

                ViewBag.ComId = ComId;


                return View(Subscribers);

            }

        }


        public ActionResult Editors(Guid ComId)
        {
            using (var db = new ApplicationDbContext())
            {

                var Ed = (from e in db.Editors
                          join u in db.Users on e.UserId.ToString() equals u.Id
                          where e.CommunityId == ComId &&
                          e.CancellationDate == null
                          select u).ToList();

                List<Subscriber> Editors = new List<Subscriber>();

                foreach (var item in Ed)
                {

                    Subscriber Sub = new Subscriber { FirstName = item.FirstName, LastName = item.LastName, isEditor = true, UserId = Guid.Parse(item.Id) };
                    Editors.Add(Sub);
                }

                Guid Uid = Guid.Parse(User.Identity.GetUserId());

                ViewBag.ComId = ComId;


                return View(Editors);
            }
        }


        public ActionResult Addeditor(Guid ComId, Guid UserId)
        {
            using (var db = new ApplicationDbContext())
            {

                Editor Ed = new Editor();

                Ed.UserId = UserId;
                Ed.CommunityId = ComId;
                Ed.AppointmentDate = DateTime.Now;
                Ed.EditorRightsId = Guid.NewGuid();

                db.Editors.Add(Ed);
                db.SaveChanges();

                return RedirectToAction("Editors", "Communities", new { ComId });

            }
        }

        public ActionResult Deleteeditor(Guid ComId, Guid UserId)
        {

            using (var db = new ApplicationDbContext())
            {
                Editor Ed = (from e in db.Editors
                             where e.UserId == UserId &&
                             e.CommunityId == ComId &&
                             e.CancellationDate == null
                             select e).First();

                db.Entry(Ed).Entity.CancellationDate = DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("Editors", "Communities", new { ComId });
            }
        }

        public ActionResult Edit(Guid? id)
        {
            var db = new ApplicationDbContext();

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

        // POST: Communities1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommunityId,CreationDate,Name,UserId")] Communities communities)
        {
            var db = new ApplicationDbContext();

            if (ModelState.IsValid)
            {
                db.Entry(communities).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Community", "Communities", new { ComId = communities.CommunityId });
            }
            return View(communities);
        }




        public ActionResult Delete(Guid? id)
        {
            var db = new ApplicationDbContext();

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

        // POST: Communities1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var db = new ApplicationDbContext();

            Communities communities = db.Communities.Find(id);
            db.Communities.Remove(communities);
            db.SaveChanges();
            return RedirectToAction("Administration", "Communities");

        }


        public ActionResult Subscribe(Guid ComId)
        {
            Subscriptions Sub = new Subscriptions();

            Sub.CommunityId = ComId;
            Sub.SubscriptionDate = DateTime.Now;
            Sub.SubscriptionId = Guid.NewGuid();
            Sub.UserId = Guid.Parse(User.Identity.GetUserId());

            db.Subscriptions.Add(Sub);
            db.SaveChanges();

            return RedirectToAction("Community", "Communities", new { ComId });
        }

        public ActionResult Unsubscribe(Guid ComId)
        {
            Guid UserId = Guid.Parse(User.Identity.GetUserId());


            Subscriptions Sub = (from s in db.Subscriptions
                                 where s.CommunityId == ComId &&
                                 UserId == s.UserId &&
                                 s.SubscriptionCancelationDate == null
                                 select s).First();

            db.Entry(Sub).Entity.SubscriptionCancelationDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Community", "Communities", new { ComId });
        }

    }
}
