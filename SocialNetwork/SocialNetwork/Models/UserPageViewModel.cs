using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class UserPageViewModel
    {
        public ApplicationUser User { get; set; }

        public List<PostInfo> News { get; set; }
    }
}