using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Communities
    {
        [Key]
        public Guid CommunityId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
}