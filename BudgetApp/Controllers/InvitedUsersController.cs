﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BudgetApp.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Configuration;
using BudgetApp.HelperExtensions;

namespace BudgetApp.Controllers
{
    [RequireHttps]
    [Authorize]
    public class InvitedUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InvitedUsers
        public ActionResult Index()
        {
            var invitedUsers = db.InvitedUsers.Include(i => i.Household);
            return View(invitedUsers.ToList());
        }

        // GET: InvitedUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvitedUser invitedUser = db.InvitedUsers.Find(id);
            if (invitedUser == null)
            {
                return HttpNotFound();
            }
            return View(invitedUser);
        }

        // GET: InvitedUsers/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: InvitedUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Name,Email,AdminRights,InviteCode")] InvitedUser invitedUser)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                var user = db.Users.FirstOrDefault(u => u.Id.Equals(id));
                var hId = Convert.ToInt32(User.Identity.GetHouseholdId());
 
                invitedUser.HouseholdId = hId;
                invitedUser.InviteCode = Membership.GeneratePassword(10, 4);
                invitedUser.InvitedBy = user.FirstName + " " + user.LastName;

                //if (AdminRights==true)
                //{
                //    give Admin rights
                //}

                //How to force delete of entry after certain time period? Or leave entry but generate new code?
                var es = new EmailService();
                var msg = invitedUser.CreateJoinMessage();
                es.SendAsync(msg);

                db.InvitedUsers.Add(invitedUser);
                db.SaveChanges();
                return RedirectToAction("Details", "Households", (new { id = user.HouseholdId }));
            }

            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitedUser.HouseholdId);
            return View(invitedUser);
        }
        

        // GET: InvitedUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvitedUser invitedUser = db.InvitedUsers.Find(id);
            if (invitedUser == null)
            {
                return HttpNotFound();
            }
            return View(invitedUser);
        }

        // POST: InvitedUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvitedUser invitedUser = db.InvitedUsers.Find(id);
            db.InvitedUsers.Remove(invitedUser);
            db.SaveChanges();
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
