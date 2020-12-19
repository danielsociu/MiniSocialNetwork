using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MiniSocialNetwork.Models
{
    public class Friend
    {
        
        public int FriendId { get; set; }

        
        [ForeignKey("User1")]
        public string User1Id { get; set; }
        public virtual ApplicationUser User1 { get; set; }

        
        [ForeignKey("User2")]
        public string User2Id { get; set; }
        public virtual ApplicationUser User2 { get; set; }

        public bool Accepted { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

       
        
    }
}