using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class Editor
    {
        [Key]
        public Guid EditorRightsId { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }

        public DateTime? CancellationDate { get; set; }
        public Guid CommunityId { get; set; }
        public Guid UserId { get; set; }
    }
}