using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;

// Message Controller

namespace TheWall.Controllers
{
    // Route for Controller
    [Route("Messages")]
    public class MessagesController : Controller
    {
        // <---------- Dependency Injection ---------->

        // Create DB Context object
        private WallContext dbContext;

        // Injects context service into the constructor
        public MessagesController(WallContext context)
        {
            dbContext = context;
        }

        // <---------- Message GET routes ---------->
        
        // Catch-all Http route! AccountController is the 'home' controller
        // due to being the login and registration module.
        [HttpGet("")]
        public IActionResult Index()
        {
            // Retrieves data from session to query as an event handler
            // checks to see if the session data is present to prevent 
            // penetration.
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

                    // Else, understands that the user is 'logged in' and moves them to the
                    // Index in the Messages controller
                    else
                    {
                        // Creates the viewbag variable for Razor to read for the NavBar
                        ViewBag.LoggedIn = true;
                        ViewBag.AccountId = accountId;

                        // Create Message Bundle for the messages Index
                        var messageBundle = new MessageBundle();
                        messageBundle.messageList = dbContext.Messages
                            .Include(m => m.Creator)
                            .Include(m => m.Comments)
                            .OrderByDescending(m => m.CreatedAt)
                            .ToList();

                        return View(messageBundle);
                    }
                }
            }
        }

        // <---------- Message POST routes ---------->

        // RESTful route for creating a post
        [HttpPost("Create")]
        public IActionResult Create(Message message)
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
                    // account user to create a message
                    else
                    {
                        // Validates model state
                        if(ModelState.IsValid)
                        {
                            // Updates the DateTime and UserId values for the DB entry
                            message.CreatedAt = DateTime.Now;
                            message.UpdatedAt = DateTime.Now;
                            message.AccountId = (int)accountId;

                            // Queries the DB to add the Message object to the DB
                            dbContext.Messages.Add(message);
                            dbContext.SaveChanges();

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Creates a ViewBag variable for Razor to read for the navBar
                            ViewBag.LoggedIn = true;

                            // Creates ViewBag AccountId for Razor to read for delete buttons
                            ViewBag.AccountId = accountId;

                            // Returns Index view with Errors
                            return View("Index");
                        }
                    }
                }
            }
        }

        // RESTful route for deleting a Message
        [HttpPost("Destroy/{messageId}")]
        public IActionResult Destroy(int messageId)
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

                    // If all checks pass, deletes the Message
                    else
                    {
                        // Query the DB for the selected Message
                        Message deleteMessage = dbContext.Messages.FirstOrDefault(m => m.MessageId == messageId);

                        // If query is null, redirect to the Index
                        if(deleteMessage == null)
                        {
                            return RedirectToAction("Index");
                        }

                        // Check query AccountId against Session AccountId
                        else
                        {
                            if(deleteMessage.AccountId != accountId)
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
                                dbContext.Messages.Remove(deleteMessage);
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