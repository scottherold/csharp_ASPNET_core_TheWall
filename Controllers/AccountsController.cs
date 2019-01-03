using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

// Account Controller

namespace TheWall.Controllers
{
    public class AccountsController : Controller
    {
        // <---------- Dependency Injection ---------->

        // Create DB Context object
        private WallContext dbContext;

        // Injects context service into the constructor
        public AccountsController(WallContext context)
        {
            dbContext = context;
        }

        // <---------- Users GET routes ---------->
        
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
                return View();
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
                    return View();
                }

                // Else checks to see if the session ID matches the queried Account ID for the 
                // ID in session
                else
                {
                    // If the ID's don't match, kills session and returns the view
                    if(accountInDb.AccountId != (int)accountId)
                    {
                        HttpContext.Session.Clear();
                        return View();
                    }

                    // Else, understands that the user is 'logged in' and moves them to the
                    // Index in the Messages controller
                    else
                    {
                        // Creates the viewbag variable for Razor to read for the NavBar
                        ViewBag.LoggedIn = true;
                        return RedirectToAction("Index", "Messages");
                    }
                }
            }
        }

        // RESTful route for registration form.
        [HttpGet("New")]
        public IActionResult New()
        {
            // Creates a 'loggedIn' variable using session to check to see if the user
            // is logged in.
            string loggedIn = HttpContext.Session.GetString("LoggedIn");

            // If loggedIn is not null, then proceed
            if(loggedIn == null)
            {
                return View();
            }

            // Else, kill Session and proceed
            else
            {
                ViewBag.LoggedIn = null;
                HttpContext.Session.Clear();
                return View();
            }
        }

        // <---------- Users POST routes ---------->

        // RESTful route for processing a request to create an account
        [HttpPost("Create")]
        public IActionResult Create(Account account)
        {
            // Checks validations
            if(ModelState.IsValid)
            {
                // Checks to see if age is over 18
                if(account.Age >= 18)
                {
                    // If account user over 18, checks for duplicate email address in the DB
                    if(dbContext.Accounts.Any(a => a.Email == account.Email))
                    {
                        // If email exists, throws an email error and returns the Registration form
                        ModelState.AddModelError("Email", "Email is already in use!");
                        return View("New");
                    }
                    // Else, creates the Account
                    else
                    {
                        // Sets DateTime values for account creations
                        account.CreatedAt = DateTime.Now;
                        account.UpdatedAt = DateTime.Now;

                        // Creates a password hasher object using Identity
                        PasswordHasher<Account> Hasher = new PasswordHasher<Account>();

                        // Uses the PW 'Hasher' object to hash the userform password
                        account.Password = Hasher.HashPassword(account, account.Password);

                        // Adds an Account to the DB
                        dbContext.Add(account);
                        dbContext.SaveChanges();

                        // Creates 'Logged In' status, with security validation.
                        // Each route can now check to see if the User is logged in using
                        // session data to validate a query to the DB. If the email does 
                        // not match the email for the user id, then session will be cleared.
                        // *NOTE* this can eventually replaced with Indentity in future projects!
                        HttpContext.Session.SetString("LoggedIn", "true");
                        HttpContext.Session.SetInt32("AccountId", account.AccountId);
                        HttpContext.Session.SetString("Email", account.Email);

                        return RedirectToAction("Index", "Messages");
                    }
                }
                else
                {
                    // If account user is under 18, throws a birthday error and returns the Registration form
                    ModelState.AddModelError("Birthday", "A user must be over the age of 18 to register an account!");
                    return View("New");
                }
            }
            // If the Modelstate is invalid, also checks for age
            else
            {
                if(account.Age < 18)
                {
                    // If there are other errors and the registration birthday is under 18, throws a birthday error and
                    // returns the registration form
                    ModelState.AddModelError("Birthday", "A user must be over the age of 18 to register an account!");
                    return View("New");
                }
                else
                {
                    // If the modelstate fails and the user is over 18, returns the registration form with Data Annotation errors
                    return View("New");
                }
            }
        }

        // RESTful route for Login
        [HttpPost("Login")]
        public IActionResult Login(LoginAccount submission)
        {
            // Checks validations
            if(ModelState.IsValid)
            {
                // If there are no form errors, query the DB for the User's
                // Email in the DB to see if it exists
                var loginAccount = dbContext.Accounts.FirstOrDefault(a => a.Email == submission.Email);

                // If user doesn't exist, throw a form field error
                // and return the Login view
                if(loginAccount == null)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password!");
                    return View("Index");
                }
                // Else, attempt to verify password
                else
                {
                    // Creates a password hasher object using Identity
                    var hasher = new PasswordHasher<LoginAccount>();

                    // Verify provided password against has stored in db
                    var result = hasher.VerifyHashedPassword(submission, loginAccount.Password, submission.Password);

                    // Result compared to 0 for failure
                    if(result == 0)
                    {
                        ModelState.AddModelError("Password", "Invalid Email/Password!");
                        return View("Index");
                    }

                    // Else creates 'Logged In' status, with security validation.
                    // Each route can now check to see if the User is logged in using
                    // session data to validate a query to the DB. If the email does 
                    // not match the email for the user id, then session will be cleared.
                    // *NOTE* this can eventually replaced with Indentity in future projects!
                    else
                    {
                        HttpContext.Session.SetString("LoggedIn", "true");
                        HttpContext.Session.SetInt32("AccountId", loginAccount.AccountId);
                        HttpContext.Session.SetString("Email", loginAccount.Email);

                        // Redirects to the Messages Index, now "Logged In"
                        return RedirectToAction("Index", "Messages");
                    }
                }
            }
            // Else display the field errors
            else
            {
                return View("Index");
            }
        }

        // RESTful route for Login
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            // Kills Session and returns to the Index
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}