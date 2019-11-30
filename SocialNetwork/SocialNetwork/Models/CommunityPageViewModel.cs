using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class CommunityPageViewModel
    {
        public string CommunityName { get; set; }

        public Guid CommunityId { get; set; }

        public List<PostInfo> News { get; set; }

        public List<ApplicationUser> Subs { get; set; }

        public bool isEditor { get; set; }

        public bool isAdmin { get; set; }

        public bool isSubscriber { get; set; }
    }
}