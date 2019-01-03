using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;

// Comment controller

namespace TheWall
{
    // Route for Controller
    [Route("Comments")]    
    public class CommentsController : Controller
    {
        // <---------- Dependency Injection ---------->

        // Create DB Context object
        private WallContext dbContext;

        // Injects context service into the constructor
        public CommentsController(WallContext context)
        {
            dbContext = context;
        }
        // <---------- Comment POST routes ---------->

         // RESTful route for creating a Comment
        [HttpPost("Create")]
        public IActionResult Create(Comment comment)
        {
            // The added security measures on the post route is to ensure no
            // one adds a message with a different User ID than what the one 'logged in'

            string loggedIn = HttpContext.Session.GetString("LoggedIn");
            int? accountId = HttpContext.Session.GetInt32("AccountId");
            string email = HttpContext.Session.GetString("Email");

            // If loggedIn not present, proceed to default View
            if(loggedIn == null)
            {
                return View("Index", "Accounts");
            }

            // Else checks to see if the Account is in the DB
            else
            {
                // Creates a new account object using the session email
                Account accountInDb = dbContext.Accounts.FirstOrDefault(a => a.Email == email);
                
                // If the email is not in the DB, kills Session and returns the View
                if(accountInDb == null)
                {
                    HttpContext.Session.Clear();
                    return View("Index", "Acounts");
                }

                // Else checks to see if the session ID matches the queried Account ID for the 
                // ID in session
                else
                {
                    // If the ID's don't match, kills session and returns the view
                    if(accountInDb.AccountId != (int)accountId)
                    {
                        HttpContext.Session.Clear();
                        return View("Index", "Accounts");
                    }

                    // Else, understands that the account is 'logged in' and allows the
                    // account user to create a comment
                    else
                    {
                        // Validates model state
                        if(ModelState.IsValid)
                        {
                            // Updates the DateTime and UserId values for the DB entry
                            comment.CreatedAt = DateTime.Now;
                            comment.UpdatedAt = DateTime.Now;
                            comment.AccountId = (int)accountId;

                            // Queries the DB to add the Message object to the DB
                            dbContext.Comments.Add(comment);
                            dbContext.SaveChanges();

                            return RedirectToAction("Index", "Messages");
                        }
                        else
                        {
                            // Creates a ViewBag variable for Razor to read for the navBar
                            ViewBag.LoggedIn = true;

                            // Creates ViewBag AccountId for Razor to read for delete buttons
                            ViewBag.AccountId = accountId;

                            // Returns Index view with Errors
                            return View("Index", "Messages");
                        }
                    }
                }
            }
        }

        // RESTful route for deleting a Comment
        [HttpPost("Destroy/{commentId}")]
        public IActionResult Destroy(int commentId)
        {
            // The added security measures on the post route is to ensure no
            // one deletes a message with a different User ID than what the one 'logged in'

            string loggedIn = HttpContext.Session.GetString("LoggedIn");
            int? accountId = HttpContext.Session.GetInt32("AccountId");
            string email = HttpContext.Session.GetString("Email");

            // If loggedIn not present, proceed to default View
            if(loggedIn == null)
            {
                return View("Index", "Accounts");
            }

            // Else checks to see if the Account is in the DB
            else
            {
                // Creates a new account object using the session email
                Account accountInDb = dbContext.Accounts.FirstOrDefault(a => a.Email == email);
                
                // If the email is not in the DB, kills Session and returns the View
                if(accountInDb == null)
                {
                    HttpContext.Session.Clear();
                    return View("Index", "Acounts");
                }

                // Else checks to see if the session ID matches the queried Account ID for the 
                // ID in session
                else
                {
                    // If the ID's don't match, kills session and returns the view
                    if(accountInDb.AccountId != (int)accountId)
                    {
                        HttpContext.Session.Clear();
                        return View("Index", "Accounts");
                    }

                    // If all checks pass, Queries the DB for the comment
                    else
                    {
                        // Query the DB for the selected Comment
                        Comment deleteComment = dbContext.Comments.FirstOrDefault(c => c.CommentId == commentId);

                        // If query is null, redirect to the Index
                        if(deleteComment == null)
                        {
                            return RedirectToAction("Index");
                        }

                        // Check query AccountId against Session AccountId
                        else
                        {
                            if(deleteComment.AccountId != accountId)
                            {
                                
                                // If the user changes the message ID they are logged out
                                // redirects to the login page
                                HttpContext.Session.Clear();
                                return View("Index", "Accounts");
                            }
                            // Else query to delete the Message
                            else
                            {
                                // Query to delete the Message, then redirect to Index
                                dbContext.Comments.Remove(deleteComment);
                                dbContext.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }
        }

    }
}