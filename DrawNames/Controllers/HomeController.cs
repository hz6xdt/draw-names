using DrawNames.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrawNames.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public DrawNamesDbContext db { get; set; }
        private UserManager<AppUser> userManager;

        public HomeController(DrawNamesDbContext db, UserManager<AppUser> userMgr)
        {
            this.db = db;
            userManager = userMgr;
        }

        public async Task<IActionResult> Index()
        {
            List<DrawingListModel> drawingsList = await GetDrawings();
            return View(drawingsList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                Drawing drawing = new Drawing { Name = name };
                db.Drawings.Add(drawing);
                await db.SaveChangesAsync();
            }
            List<DrawingListModel> drawingsList = await GetDrawings();
            return View("Index", drawingsList);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Drawing drawing = db.Drawings.Where(d => d.Id == id).SingleOrDefault();
            if (drawing != null)
            {
                db.Drawings.Remove(drawing);
                await db.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"No drawing found for id: {id}.");
            }
            List<DrawingListModel> drawingsList = await GetDrawings();
            return View("Index", drawingsList);
        }

        [ActionName("DrawNames")]
        [HttpGet]
        public async Task<IActionResult> DrawNamesReady(int id)
        {
            DrawingListModel drawingList = await GetDrawing(id);
            return View(drawingList);
        }

        [HttpPost]
        public async Task<IActionResult> DrawNames(int id)
        {
            if (ModelState.IsValid)
            {
                DrawingListModel drawing = await GetDrawing(id);
                List<AppUser> drawingUsers = new List<AppUser>();
                Dictionary<string, string> drawnNames = new Dictionary<string, string>();

                foreach (string name in drawing.Names)
                {
                    AppUser user = await userManager.FindByNameAsync(name);

                    if (user != null)
                    {
                        drawingUsers.Add(user);
                    }
                }


                Random random = new Random();


                bool sameName = false;
                do
                {
                    sameName = false;
                    List<AppUser> cupOfNames = new List<AppUser>(drawingUsers);

                    foreach (AppUser user in drawingUsers)
                    {
                        int index = random.Next(cupOfNames.Count);
                        AppUser drawnName = cupOfNames[index];

                        if (drawnName == user)
                        {
                            sameName = true;
                            drawnNames.Clear();
                            break;
                        }
                        else
                        {
                            drawnNames.Add(user.Id, drawnName.Id);
                        }
                    }

                } while (sameName);


                foreach (string key in drawnNames.Keys)
                {
                    DrawingUsers drawingUser = db.DrawingUsers.Where(du => du.DrawingId == id && du.UserId == key).SingleOrDefault();

                    if (drawingUser != null)
                    {
                        drawingUser.DrawnUserId = drawnNames[key];
                    }
                }

                drawing.Drawing.Drawn = true;
                await db.SaveChangesAsync();
            }

            List<DrawingListModel> drawingsList = await GetDrawings();
            return View("Index", drawingsList);
        }
        public async Task<IActionResult> Edit(int id)
        {
            Drawing drawing = await db.Drawings.FindAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            foreach (AppUser user in userManager.Users)
            {
                DrawingUsers drawingUser = db.DrawingUsers.Where(du => du.DrawingId == id && du.UserId == user.Id).SingleOrDefault();

                List<AppUser> list = drawingUser == null
                    ? nonMembers
                    : members;
                list.Add(user);
            }
            return View(new DrawingEditModel
            {
                Drawing = drawing,
                Members = members,
                NonMembers = nonMembers
            });
        }

        public async Task<ActionResult> View(int id)
        {
            Drawing drawing = db.Drawings.Where(d => d.Id == id).SingleOrDefault();
            if (drawing != null)
            {
                var currentUser = await userManager.GetUserAsync(HttpContext.User);
                DrawingUsers drawingUser = db.DrawingUsers.Where(du => du.DrawingId == id && du.UserId == currentUser.Id).SingleOrDefault();

                if (drawingUser != null)
                {
                    var drawnUser = await userManager.FindByIdAsync(drawingUser.DrawnUserId);

                    if (drawnUser != null)
                    {
                        return View("ViewDrawnName", drawnUser.UserName);
                    }
                }
            }

            List<DrawingListModel> drawingsList = await GetDrawings();
            return View("Index", drawingsList);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DrawingModificationModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    db.DrawingUsers.Add(new DrawingUsers { DrawingId = model.DrawingId, UserId = userId });
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    List<DrawingUsers> drawingUsers = await db.DrawingUsers.Where(du => du.DrawingId == model.DrawingId && du.UserId == userId).ToListAsync<DrawingUsers>();
                    foreach (DrawingUsers drawingUser in drawingUsers)
                    {
                        db.DrawingUsers.Remove(drawingUser);
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return await Edit(model.DrawingId);
            }
        }

        private async Task<DrawingListModel> GetDrawing(int id)
        {
            Drawing drawing = await db.Drawings.FindAsync(id);

            List<DrawingUsers> drawingUsers = await db.DrawingUsers.Where(du => du.DrawingId == drawing.Id).ToListAsync<DrawingUsers>();
            List<string> names = new List<string>();

            foreach (DrawingUsers drawingUser in drawingUsers)
            {
                AppUser user = await userManager.FindByIdAsync(drawingUser.UserId);
                if (user != null)
                {
                    names.Add(user.UserName);
                }
            }

            DrawingListModel drawingList = new DrawingListModel { Drawing = drawing, Names = names };

            return drawingList;
        }

        private async Task<List<DrawingListModel>> GetDrawings()
        {
            List<Drawing> drawings = db.Drawings.ToList<Drawing>();
            List<DrawingListModel> drawingsList = new List<DrawingListModel>();

            foreach (Drawing drawing in drawings)
            {
                List<DrawingUsers> drawingUsers = await db.DrawingUsers.Where(du => du.DrawingId == drawing.Id).ToListAsync<DrawingUsers>();
                List<string> names = new List<string>();

                foreach (DrawingUsers drawingUser in drawingUsers)
                {
                    AppUser user = await userManager.FindByIdAsync(drawingUser.UserId);
                    if (user != null)
                    {
                        names.Add(user.UserName);
                    }
                }

                drawingsList.Add(new DrawingListModel { Drawing = drawing, Names = names });
            }

            return drawingsList;
        }
    }
}
