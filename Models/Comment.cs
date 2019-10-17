using System;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Comment
    {
        [Key]
        public int CommentId {get;set;}
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Text {get;set;}
        public DateTime Created_at {get;set;} = DateTime.Now;
        public DateTime Updated_at {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        public User Creator {get;set;}
        public int MessageId {get;set;}
        public Message Message {get;set;}
    }
}