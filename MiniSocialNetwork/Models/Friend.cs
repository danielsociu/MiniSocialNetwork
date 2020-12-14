using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniSocialNetwork.Models
{
    public class Friend
    {
        [Key]
        public int Friends_id { get; set; }
        [Required(ErrorMessage = "Sender Id is required!")]
        public int Sender_id { get; set; }
        [Required(ErrorMessage = "Receiver id is required!")]
        public int Receiver_id { get; set; }
        public int State { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public int Created_at { get; set; }

        public virtual Profile Profile { get; set; }
        public ApplicationUser User { get; set; }
    }
}