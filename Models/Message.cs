using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TheWall.Models
{
    public class Message
    {
        // Key for Account table entry
        [Key]
        public int MessageId { get; set; }

        // Message text renamed "MessagePost" to differentiate from enclosing
        // class name
        [Display(Name="Message")]
        [Required(ErrorMessage="Message cannot be blank!")]
        public string MessagePost { get; set; }

        // Timestamps for data entry
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // <---------- Entity Model References ---------->

        // PK for Many-to-One relationship
        public int AccountId { get; set; }
        
        // Entity Account Model reference
        public Account Creator { get; set; }

        // Entity Comment List
        public List<Comment> Comments { get; set; }

        // <---------- Custom Properties ---------->

        public Boolean Deleteable
        {
            get {

                // Changes the CreatedTime property from the model
                // into UTC for a comparison
                DateTime postTime = CreatedAt.ToUniversalTime();

                if(postTime.AddMinutes(30) > DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}