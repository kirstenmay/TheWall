using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWall.Models
{
    public class User
        {
            // auto-implemented properties need to match the columns in your table
            // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
            [Key]
            public int UserId { get; set; }
            // MySQL VARCHAR and TEXT types can be represeted by a string
            [Required]
            [MinLength(2)]
            public string FirstName { get; set; }
            [Required]
            [MinLength(2)]
            public string LastName { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [MinLength(8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            // The MySQL DATETIME type can be represented by a DateTime
            [NotMapped]
            [Required]
            [Compare("Password", ErrorMessage = "Passwords do not match!")]
            [DataType(DataType.Password)]
            public string ConfirmPassword {get;set;}
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now;
            public List<Message> UserMessages {get;set;}
        }
        public class LoginUser
        {
            [Key]
            public int UserId { get; set; }
            [EmailAddress]
            [Required]
            public string LoginEmail {get;set;}
            [Required]
            public string LoginPassword {get;set;}
        }
}