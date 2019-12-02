using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class PostInfo
    {
        public Guid PostId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Text { get; set; }
        public Guid CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string AuthorFN { get; set; }
        public string AuthorLN { get; set; }

    }

    public class CommentInfo
    {
        public Guid CommentId { get; set; }

        public DateTime CommentDate { get; set; }

        public string CommentText { get; set; }

        public string UserFn { get; set; }

        public string UserLn { get; set; }

        public Guid UserId { get; set; }
    }
}