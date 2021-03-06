﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChatterApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ChatterApp.Controllers
{
    public class ChatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Follow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Request.IsAuthenticated)
            {

                Chat chat = db.Chats.Find(id);
                if (chat == null)
                {
                    return HttpNotFound();
                }
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                ApplicationUser authorUser = chat.ApplicationUser;
                if (currentUser.Following.Contains(authorUser))
                {
                    ViewBag.content = "You are already following this user";                
                }
                else
                {
                    currentUser.Following.Add(authorUser);
                    ViewBag.content = "You are now following this user";
                    db.SaveChanges();
                }
            }
            return View();
        }

        public ActionResult Browse()
        {
            List<Chat> chats = db.Chats.OrderByDescending(c => c.ChatID).ToList();
            ViewBag.chats = chats;
            return View();
        }

        public ActionResult Feed()
        {
            if (Request.IsAuthenticated)
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                List<string> following = new List<string>();
                foreach (ApplicationUser followed in currentUser.Following)
                {
                    following.Add(followed.Id);
                }
                IEnumerable<Chat> chatEnumerable = db.Chats.Where(c => following.Contains(c.Id)).Select(c => c).OrderByDescending(c => c.ChatID).AsEnumerable();
                List<Chat> chats = chatEnumerable.ToList();
                ViewBag.authentication = "yes";
                ViewBag.chats = chats;
                return View();
            }
            ViewBag.authentication = "no";
            return View();
        }

        // GET: Chats
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());

                List<Chat> chats = db.Chats.Where(c => c.ApplicationUser.Id.Equals(currentUser.Id)).OrderByDescending(c=>c.ChatID).ToList();
                //var chats = db.Chats.Include(c => c.ApplicationUser);
                //ApplicationUser erica = UserManager.FindById("4049e150-4441-47e1-be11-035b8ae3d1ce");
                //ApplicationUser valerie = UserManager.FindById("6d14c31b-b392-4632-b515-f95184596e11");

                //if (erica.Following == null)
                //{
                //    erica.Following = new ApplicationUser[] { valerie };
                //}
                db.SaveChanges();
                return View(chats);
            }
            List<Chat> unauthenticated = new List<Chat>();
            return View(unauthenticated);
        }

        // GET: Chats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = db.Chats.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.chatID = id;

            return View(chat);
        }

        // GET: Chats/Create
        public ActionResult Create()
        {
            if (Request.IsAuthenticated)
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                List<ApplicationUser> userList = new List<ApplicationUser>();
                userList.Add(currentUser);
                ViewBag.Id = new SelectList(userList, "Id", "Email");
            }
            else
            {
                List<ApplicationUser> userList = new List<ApplicationUser>();
                ViewBag.Id = new SelectList(userList, "Id", "Email");
            }
            return View();
        }

        // POST: Chats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChatID,Id,Content")] Chat chat)
        {
            
            if (ModelState.IsValid)
            {
                db.Chats.Add(chat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.ApplicationUsers, "Id", "Email", chat.Id);
            return View(chat);
        }

        // GET: Chats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = db.Chats.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAuthenticated)
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                if (currentUser == chat.ApplicationUser)
                {
                    List<ApplicationUser> userList = new List<ApplicationUser>();
                    userList.Add(currentUser);
                    ViewBag.Id = new SelectList(userList, "Id", "Email");
                    //ViewBag.Id = new SelectList(db.ApplicationUsers, "Id", "Email", chat.Id);
                    return View(chat);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");

            }
          
        }

        // POST: Chats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChatID,Id,Content")] Chat chat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chat).State = EntityState.Modified;
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                if (chat.ApplicationUser == currentUser)
                {
                    ViewBag.Id = new SelectList(db.ApplicationUsers, "Id", "Email", chat.Id);
                    db.SaveChanges();
                    return View(chat);
                   
                    
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Chats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chat chat = db.Chats.Find(id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAuthenticated)
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
                if (currentUser == chat.ApplicationUser)
                {
                    return View(chat);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        // POST: Chats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chat chat = db.Chats.Find(id);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            if (chat.ApplicationUser == currentUser)
            {
                db.Chats.Remove(chat);
                db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
