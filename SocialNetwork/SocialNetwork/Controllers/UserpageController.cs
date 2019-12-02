﻿using Microsoft.AspNet.Identity;
using SocialNetwork.Models;
using System;
using System.Collections.Generic;
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


            var isFriend = db.Friendships
                        .Where(c => (c.applicantId == id && c.aimPersonId == currentUser) || (c.applicantId==currentUser && c.aimPersonId == id))
                        .Select(c => c);


            if (isFriend.Count() != 0) 
                ViewBag.isFriend = true;
            else
                ViewBag.isFriend = false;

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
    }
}