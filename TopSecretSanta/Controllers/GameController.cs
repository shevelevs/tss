using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopSecretSanta.Models;
using WebMatrix.WebData;
using System.Web.Security;

namespace TopSecretSanta.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private SecretSantaContext db = new SecretSantaContext();

        private bool EditAllowed(Game g)
        {
            return g.OwnerId == WebSecurity.CurrentUserId;
        }

        //
        // GET: /Game/
        public ActionResult Index()
        {
            if (Roles.IsUserInRole("Administrator")) {
                ViewBag.PageTitle = "All games";

                return View(db.Games.ToList());
            }
            else {
                ViewBag.PageTitle = "My games";

                    var result = (  from g in db.Games
                                   where g.OwnerId == WebSecurity.CurrentUserId
                                  select g).
                             Union( from g in db.Games
                                    join p in db.Players
                                      on g.GameId equals p.GameId
                                   where p.UserId == WebSecurity.CurrentUserId
                                  select g);
                
                return View(result.ToList());
            }
        }

        //
        // GET: /Game/Details/5

        public ActionResult Details(int id = 0)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        //
        // GET: /Game/Create

        public ActionResult Create()
        {
            return View(new Game() { DateCreated = DateTime.Now, OwnerId = WebSecurity.CurrentUserId });
        }

        //
        // POST: /Game/Create

        [HttpPost]
        public ActionResult Create(Game game)
        {
            if (ModelState.IsValid)
            {
                game.OwnerId = WebSecurity.CurrentUserId;
                game.DateCreated = DateTime.Now;
                db.Games.Add(game);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(game);
        }

        //
        // GET: /Game/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        //
        // POST: /Game/Edit/5

        [HttpPost]
        public ActionResult Edit(Game game)
        {
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        //
        // GET: /Game/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        //
        // POST: /Game/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Game game = db.Games.Find(id);
            db.Games.Remove(game);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Game/DeletePlayer/5
        public ActionResult DeletePlayer(int id)
        {
            var p = db.Players.Find(id);
            if (p == null) {
                return HttpNotFound();
            }

            var game = p.Game;
            if (EditAllowed(game)) {
                ViewBag.StatusMessage = "You have successfully removed player " + p.UserProfile.FullName + " from the game";
                db.Players.Remove(p);
                db.SaveChanges();
            }
            return View("Edit", game);
        }

        //
        // GET: /Game/DeletePlayer/5
        public ActionResult DeleteInvitation(Guid id)
        {
            var inv = db.Invitations.Find(id);
            if (inv == null) {
                return HttpNotFound();
            }

            var game = inv.Game;
            if (EditAllowed(game)) {
                ViewBag.StatusMessage = "You have successfully removed invitation to " + inv.Email + " from the game";
                db.Invitations.Remove(inv);
                db.SaveChanges();
            }
            return View("Edit", game);
        }

        //
        // POST: /Game/AddInvitation
        [HttpPost]
        public ActionResult AddInvitation(Invitation invitation)
        {
            var g = db.Games.Find(invitation.GameId);
            if (g == null) {
                return HttpNotFound();
            }

            if (EditAllowed(g)) {
                ViewBag.StatusMessage = "You have successfully added invitation to " + invitation.Email + " to the game";
                db.Invitations.Add(invitation);
                db.SaveChanges();
            }
            return View("Edit", g);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}