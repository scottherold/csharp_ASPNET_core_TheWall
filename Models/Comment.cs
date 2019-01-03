using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWall.Models
{
    public class Comment
    {
        // Key for Comment table entry
        [Key]
        public int CommentId { get; set; }

        // Message text renamed "Post" to differentiate from enclosing
        // class name
        [Display(Name="Comment")]
        [Required(ErrorMessage="Comment cannot be blank!")]
        public string CommentPost { get; set; }

        // Timestamps for data entry
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // <---------- Entity Model References ---------->

        // PK for Many-to-One relationship w/ Account
        public int AccountId { get; set; }
        
        // Entity Account Model reference -- Account
        public Account Creator { get; set; }

        // PK for Many-to-One relationship w/ Message
        public int MessageId { get; set; }
        
        // Entity Account Model reference -- Message
        public Message CommentedOn { get; set; }
    }
}