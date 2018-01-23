using System;
using System.Threading.Tasks;
using Aiursoft.Pylon.Services;
using Echeers.Mq.Data;
using Echeers.Mq.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Echeers.Mq.Controllers
{
    [Authorize]
    public class AppsController : Controller
    {
        private readonly MqDbContext _dbContext;
        private readonly UserManager<MqUser> _userManager;
        public AppsController(
            MqDbContext dbContext,
            UserManager<MqUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyApps()
        {
            var user = await GetCurrentUserAsync();
            return View(user.Apps);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(App model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var app = new App(user.Id, model.Name)
            {
                OwnerId = user.Id
            };
            _dbContext.Apps.Add(app);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(MyApps));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var app = await _dbContext.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();
            if (app.OwnerId != user.Id)
            {
                return Unauthorized();
            }
            return View(app);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(App model)
        {
            var app = await _dbContext.Apps.FindAsync(model.Id);
            if (app == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();
            if (app.OwnerId != user.Id)
            {
                return Unauthorized();
            }
            _dbContext.Channels.Delete(t => t.AppId == app.Id);
            _dbContext.Apps.Remove(app);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(MyApps));
        }

        public Task<MqUser> GetCurrentUserAsync()
        {
            return _dbContext.Users.Include(t => t.Apps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}