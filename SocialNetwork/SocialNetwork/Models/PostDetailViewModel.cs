using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class PostDetailViewModel
    {
        public PostInfo Post { get; set; }

        public List<CommentInfo> Comments { get; set; }

        public Comment comment { get; set; }

        public bool isAuthor { get; set; }
        public bool isAdmin { get; set; }

        public Guid AuthorId { get; set; }
    }
}