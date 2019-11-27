using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Subscriptions
    {
        [Key]
        public Guid SubscriptionId { get; set; }

        [Required]
        public DateTime SubscriptionDate { get; set; }

        public DateTime? SubscriptionCancelationDate { get; set; }

        public Guid CommunityId { get; set; }

        public Guid UserId { get; set; }
    }
}