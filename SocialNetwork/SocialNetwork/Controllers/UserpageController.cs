using Microsoft.AspNet.Identity;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class UserpageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Userpage
        public ActionResult Index()
        {
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
                        join u in db.Users on r.UserId.ToString() equals u.Id
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
                            u.FirstName,
                            u.Id
                        }).ToList();


            Page.News = new List<PostInfo>();

            foreach (var item in news)
            {
                PostInfo one = new PostInfo { PostId = item.PostId, AuthorFN = item.FirstName, AuthorLN = item.LastName, CommunityId = item.CommunityId, CommunityName = item.Name, CreationDate = item.CreationDate, Text = item.Text, AuthorId = Guid.Parse(item.Id) };
                Page.News.Add(one);
            }

            return View(Page);
        }


        //Страница другого пользователя
        public ActionResult Id(Guid id)
        {

            Guid currentUser = Guid.Parse(User.Identity.GetUserId());

            if (currentUser == id)
            {
                return RedirectToAction("Index", "Userpage");
            }


            var userId = id.ToString();


            UserPageViewModel Page = new UserPageViewModel();

            Page.User = db.Users
                        .Where(c => c.Id == userId)
                        .Select(c => c).First();


            var news = (from sub in db.Subscriptions
                        join p in db.Posts on sub.CommunityId equals p.CommunityId
                        join c in db.Communities on sub.CommunityId equals c.CommunityId
                        join r in db.Editors on p.RightsId equals r.EditorRightsId
                        join u in db.Users on r.UserId.ToString() equals u.Id
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
                            u.FirstName,
                            u.Id
                        }).ToList();


            Page.News = new List<PostInfo>();

            foreach (var item in news)
            {
                PostInfo one = new PostInfo { PostId = item.PostId, AuthorFN = item.FirstName, AuthorLN = item.LastName, CommunityId = item.CommunityId, CommunityName = item.Name, CreationDate = item.CreationDate, Text = item.Text, AuthorId = Guid.Parse(item.Id) };
                Page.News.Add(one);
            }


            var isFriend = db.Friendships
                        .Where(c => (c.applicantId == id && c.aimPersonId == currentUser) || (c.applicantId==currentUser && c.aimPersonId == id))
                        .Select(c => c);


            if (isFriend.Count() != 0)
            {
                if (isFriend.First().friendshipAccepted)
                {
                    ViewBag.isFriend = true;
                }
                else
                {
                    ViewBag.isFriend = false;

                    if (isFriend.First().applicantId == currentUser)
                    {
                        ViewBag.isApplicant = true;
                        ViewBag.isAimPerson = false;
                    }
                    else
                    {
                        ViewBag.isAimPerson = true;
                        ViewBag.isApplicant = false;
                    }
                }       
            }
            else
            {
                ViewBag.isFriend = false;
                ViewBag.isApplicant = false;
                ViewBag.isAimPerson = false;
            }
                

            ViewBag.UserId = id;

            return View(Page);
        }


        public ActionResult Addfriend(Guid id)
        {
            Friendship friendship = new Friendship();

            friendship.aimPersonId = id;
            friendship.applicantId = Guid.Parse(User.Identity.GetUserId());
            friendship.applyDate = DateTime.Now;
            friendship.friendshipAccepted = false;

            db.Friendships.Add(friendship);
            db.SaveChanges();

            return RedirectToAction("Id", "Userpage", new { id });
        }

        public ActionResult Confirmfriend(Guid id)
        {
            Guid currentUser = Guid.Parse(User.Identity.GetUserId());
            var Friendship = db.Friendships
                            .Where(c => (c.applicantId == id && c.aimPersonId == currentUser) || (c.applicantId == currentUser && c.aimPersonId == id))
                            .Select(c => c);
            Friendship.First().friendshipAccepted = true;

            db.Entry(Friendship.First()).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Id", "Userpage", new { id });
        }

        public ActionResult Confirmfriendapplications(Guid id)
        {
            Guid currentUser = Guid.Parse(User.Identity.GetUserId());
            var Friendship = db.Friendships
                            .Where(c => (c.applicantId == id && c.aimPersonId == currentUser) || (c.applicantId == currentUser && c.aimPersonId == id))
                            .Select(c => c);
            Friendship.First().friendshipAccepted = true;

            db.Entry(Friendship.First()).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Applications", "Userpage");
        }

        public ActionResult Deletefriend(Guid id)
        {
            Guid currentUser = Guid.Parse(User.Identity.GetUserId());


            var Friendship = db.Friendships
                            .Where(c => (c.applicantId == id && c.aimPersonId == currentUser) || (c.applicantId == currentUser && c.aimPersonId == id))
                            .Select(c => c);


            db.Friendships.Remove(Friendship.First());
            db.SaveChanges();

            return RedirectToAction("Id", "Userpage", new { id });
        }

        public ActionResult Deletefriendfriends(Guid id)
        {
            Guid currentUser = Guid.Parse(User.Identity.GetUserId());


            var Friendship = db.Friendships
                            .Where(c => (c.applicantId == id && c.aimPersonId == currentUser) || (c.applicantId == currentUser && c.aimPersonId == id))
                            .Select(c => c);


            db.Friendships.Remove(Friendship.First());
            db.SaveChanges();

            return RedirectToAction("Friends", "Userpage");
        }

        public ActionResult Deletefriendapplications(Guid id)
        {
            Guid currentUser = Guid.Parse(User.Identity.GetUserId());


            var Friendship = db.Friendships
                            .Where(c => (c.applicantId == id && c.aimPersonId == currentUser) || (c.applicantId == currentUser && c.aimPersonId == id))
                            .Select(c => c);


            db.Friendships.Remove(Friendship.First());
            db.SaveChanges();

            return RedirectToAction("Applications", "Userpage");
        }


        public ActionResult Friends(string Filter)
        {
            using (var db = new ApplicationDbContext())
            {

                List<ApplicationUser> Friends1 = new List<ApplicationUser>();
                List<ApplicationUser> Friends2 = new List<ApplicationUser>();

                var currentUser = User.Identity.GetUserId();


                Friends1 = (from u in db.Users
                            join f1 in db.Friendships on u.Id equals f1.aimPersonId.ToString()
                            where f1.applicantId.ToString() == currentUser && f1.friendshipAccepted == true
                            select u).ToList();

                Friends2 = (from u in db.Users
                            join f1 in db.Friendships on u.Id equals f1.applicantId.ToString()
                            where f1.aimPersonId.ToString() == currentUser && f1.friendshipAccepted == true
                            select u).ToList();

                var Friends = Friends2.Union(Friends1);

                if (!String.IsNullOrEmpty(Filter))
                {
                    Friends = Friends.Where(s => s.FirstName.Contains(Filter) || s.LastName.Contains(Filter));
                }

                return View(Friends.ToList());
            }

        }



        public ActionResult Applications (string Filter)
        {
            using (var db = new ApplicationDbContext())
            {

                List<ApplicationUser> Friends1 = new List<ApplicationUser>();
                List<ApplicationUser> Friends2 = new List<ApplicationUser>();

                var currentUser = User.Identity.GetUserId();

                Friends2 = (from u in db.Users
                            join f1 in db.Friendships on u.Id equals f1.applicantId.ToString()
                            where f1.aimPersonId.ToString() == currentUser && f1.friendshipAccepted == false
                            select u).ToList();

                var Friends = Friends2.Union(Friends1);

                if (!String.IsNullOrEmpty(Filter))
                {
                    Friends = Friends.Where(s => s.FirstName.Contains(Filter) || s.LastName.Contains(Filter));
                }

                return View(Friends.ToList());
            }

        }
    }
}