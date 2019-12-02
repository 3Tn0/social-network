using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Friendship
    {
        [Key]
        [Column(Order = 1)]
        public Guid applicantId { get; set; }
        [Key]
        [Column(Order = 2)]
        public Guid aimPersonId { get; set; }
        [Required]
        public bool friendshipAccepted { get; set; }
        [Required]
        public DateTime applyDate { get; set; }
    }
}