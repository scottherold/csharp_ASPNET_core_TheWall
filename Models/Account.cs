using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TheWall.Models
{
    public class Account
    {
        // Key for Account table entry
        [Key]
        public int AccountId { get; set; }

        // For First Name
        [Display(Name="First Name")]
        [Required(ErrorMessage="First name is required!")]
        [MinLength(2,ErrorMessage="First name must be at least two characters!")]
        public string FirstName { get; set; }

        // For Last Name
        [Display(Name="Last Name")]
        [Required(ErrorMessage="Last name is required!")]
        [MinLength(2,ErrorMessage="Last name must be at least two characters!")]
        public string LastName { get; set; }

        [Display(Name="Email")]
        [Required(ErrorMessage="Email is required!")]
        [EmailAddress(ErrorMessage="Email address must be in a valid format!")]
        public string Email { get; set; }

        [Display(Name="Birthday")]
        [Required(ErrorMessage="Birthday is required!")]
        [DataType(DataType.Date, ErrorMessage="Birthday must be a valid date!")]
        public DateTime? Birthday { get; set; }

        // For Password
        [Display(Name="Password")]
        [Required(ErrorMessage="Password is required!")]
        [MinLength(8,ErrorMessage="Password must be at least 8 characters!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // For Confirm
        // Not mapped due to being password confirm
        [NotMapped]
        [Compare("Password", ErrorMessage="Password confirmation must match Password field!")]
        [Required(ErrorMessage="Please confirm your password!")]
        public string Confirm { get; set;}

        // Timestamps for data entry
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // <---------- Entity Model References ---------->

        // Entity Message List
        public List<Message> Messages { get; set; }

        // Entity Comment List
        public List<Comment> Comments { get; set; }

        // <---------- Custom Properties ---------->

        // Age is created at time of creation to validate if the
        // chef is over 18.
        // Age is only established with a Getter so it will not push to the DB
        // this can be used for validation, or printing purposes without
        // adding extra information into the DB.
        public int Age 
        { 
            get
            {
                if(Birthday != null)
                {
                    // Grabs current local DateTime -- n = now
                    DateTime n = DateTime.Now;
                    DateTime birthday = (DateTime)Birthday;

                    // Grabs the age by taking the year from n, and subtracing the year from
                    // birthday
                    int age = n.Year - birthday.Year;

                    // Leap year handler
                    if (n.Month < birthday.Month || (n.Month == birthday.Month && n.Day < birthday.Day))
                    {
                        age--;
                    }
                    return age;
                }
                else
                {
                    return 0;
                }
            } 
        }
    }
    
    // Account Model class for Login
    public class LoginAccount
    {
        // Email validators
        [Display(Name="Email")]
        [Required(ErrorMessage="Please enter your Email address!")]
        [EmailAddress(ErrorMessage="Email address must be in a valid format!")]
        public string Email { get; set; }

        // Password validators
        [Display(Name="Password")]
        [Required(ErrorMessage="Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}