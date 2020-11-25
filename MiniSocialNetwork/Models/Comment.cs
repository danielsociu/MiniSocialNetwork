using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniSocialNetwork.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public string Created_at { get; set; }

        public virtual Post Post { get; set; }
        public IEnumerable<SelectListItem> Posts { get; set; }
    }

    public class CommentDBContext : DbContext
    {
        public CommentDBContext() : base("DBConnectionString") { }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
    }

}