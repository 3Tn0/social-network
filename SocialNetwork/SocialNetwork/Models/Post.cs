using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Post
    {
        [Key]
        public Guid PostId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        public Guid RightsId { get; set; }
        public Guid CommunityId { get; set; }

    }
}