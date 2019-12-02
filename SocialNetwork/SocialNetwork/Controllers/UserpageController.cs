using Microsoft.AspNet.Identity;
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
    }
}