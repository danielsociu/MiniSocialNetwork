using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiniSocialNetwork.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public string Content { get; set; }
        public string Created_at { get; set; }
        [Required]
        public int UserId { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }


}