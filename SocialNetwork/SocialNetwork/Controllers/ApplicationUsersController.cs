using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUsers/Edit/5
        public ActionResult Edit()
        {
            var id = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LastName,FirstName,RegistrationDate,BirthDate,Gender,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Userpage");
            }
            return View(applicationUser);
        }



        public ActionResult Image()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase uploadFile)
        {
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                string filePath = Path.Combine(Server.MapPath("/Temp"), User.Identity.GetUserId() + "." + "jpeg" /*Path.GetFileName(uploadFile.ContentType)*/);
                uploadFile.SaveAs(filePath);
            }
            return RedirectToAction("Index", "Userpage");
        }
    }
}
