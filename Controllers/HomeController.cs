using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWall.Models;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
     
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("userName");
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("userId", user.UserId);
                HttpContext.Session.SetString("userName", user.FirstName);
                ViewBag.Username = HttpContext.Session.GetString("userName");
                return RedirectToAction("Wall");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Username/password combination incorrect");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                HttpContext.Session.SetString("userName", userInDb.FirstName);
                ViewBag.Username = HttpContext.Session.GetString("userName");
                return RedirectToAction("Wall");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("Wall")]
        public IActionResult Wall()
        {
            if(HttpContext.Session.GetString("userName") == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                WrapperModel viewMod = new WrapperModel();
                viewMod.AllMessages = dbContext.Messages.Include(u => u.Creator).Include(c => c.MessageComments).ThenInclude(m => m.Creator).OrderByDescending(c => c.Created_at).ToList();
                ViewBag.Username = HttpContext.Session.GetString("userName");
                ViewBag.UserId = (int)HttpContext.Session.GetInt32("userId");
                return View(viewMod);
            }
        }

        [HttpPost("CreateMessage")]
        public IActionResult CreateMessage(WrapperModel message)
        {
            if(HttpContext.Session.GetString("userName") == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if(ModelState.IsValid)
            {
                int? ID = HttpContext.Session.GetInt32("userId");
                message.NewMessage.UserId = (int)ID;
                dbContext.Add(message.NewMessage);
                dbContext.SaveChanges();
                return RedirectToAction("Wall");
                }
                else
                {
                    WrapperModel viewMod = new WrapperModel();
                    viewMod.AllMessages = dbContext.Messages.Include(u => u.Creator).Include(c => c.MessageComments).OrderByDescending(c => c.Created_at).ToList();
                    ViewBag.Username = HttpContext.Session.GetString("userName");
                    ViewBag.UserId = (int)HttpContext.Session.GetInt32("userId");
                    return View("Wall", viewMod);
                }
            }
        }

        [HttpPost("CreateComment/{mId}")]
        public IActionResult CreateComment(WrapperModel comment, int mId)
        {
            if(HttpContext.Session.GetString("userName") == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if(ModelState.IsValid)
            {
                int? ID = HttpContext.Session.GetInt32("userId");
                comment.NewComment.UserId = (int)ID;
                // comment.NewComment.Creator = dbContext.Users.FirstOrDefault(u => u.UserId == ID);
                comment.NewComment.MessageId = mId;
                dbContext.Add(comment.NewComment);
                dbContext.SaveChanges();
                return RedirectToAction("Wall");
                }
                else
                {
                    WrapperModel viewMod = new WrapperModel();
                    viewMod.AllMessages = dbContext.Messages.Include(u => u.Creator).Include(c => c.MessageComments).OrderByDescending(c => c.Created_at).ToList();
                    ViewBag.Username = HttpContext.Session.GetString("userName");
                    ViewBag.UserId = (int)HttpContext.Session.GetInt32("userId");
                    return View("Wall", viewMod);
                }
            }
        }

        [HttpGet("DeleteCom/{cId}")]
        public IActionResult DeleteCom(int cId)
        {
            if(HttpContext.Session.GetString("userName") == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Message mes = dbContext.Messages.Include(u => u.Creator).Include(c => c.MessageComments).FirstOrDefault(m => m.MessageId == cId);
                Comment com = dbContext.Comments.Include(u => u.Creator).FirstOrDefault(c => c.CommentId == cId);
                int? id = HttpContext.Session.GetInt32("userId");
                if(com.Creator.UserId == id)
                {
                    if(DateTime.Now.Subtract(com.Created_at).TotalMinutes <= 30)
                    {
                        dbContext.Remove(com);
                        dbContext.SaveChanges();
                        return RedirectToAction("Wall");
                    }
                    else
                    {
                        return RedirectToAction("Wall");
                    }
                }
                else
                {
                    return RedirectToAction("Wall");
                }
            }
        }

        [HttpGet("DeleteMes/{mId}")]
        public IActionResult DeleteMes(int mId)
        {
            Message mes = dbContext.Messages.Include(u => u.Creator).Include(c => c.MessageComments).FirstOrDefault(m => m.MessageId == mId);
            Comment com = dbContext.Comments.Include(u => u.Creator).FirstOrDefault(c => c.CommentId == mId);
            int? id = HttpContext.Session.GetInt32("userId");
            if(mes.Creator.UserId == id)
            {
                if(DateTime.Now.Subtract(mes.Created_at).TotalMinutes <= 30)
                {
                    dbContext.Remove(mes);
                    dbContext.SaveChanges();
                    return RedirectToAction("Wall");
                }
                else
                {
                    return RedirectToAction("Wall");
                }
            }
            else
            {
                return RedirectToAction("Wall");
            }
        }

        [HttpGet("LogOut")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
