using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; set; }

        [Required]
        public DateTime CommentDate { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }
    }
}