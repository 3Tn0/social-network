using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Chat
    {
        [Key]
        [Column(Order = 1)]
        public DateTime sendingDate { get; set; }
        [Key]
        [Column(Order = 2)]
        public Guid senderId { get; set; }
        [Key]
        [Column(Order = 3)]
        public Guid receiverId { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string messageText { get; set; }

    }
}