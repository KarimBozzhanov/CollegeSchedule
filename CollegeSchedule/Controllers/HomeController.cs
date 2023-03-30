using CollegeSchedule.Models;
using CollegeSchedule.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CollegeSchedule.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db;

        public HomeController(ApplicationDbContext context)
        {
            db = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GroupList(int cource)
        {
            ViewData["cource"] = cource;
            return View(await db.Groups.Where(g => g.GroupCource == cource).ToListAsync());
        }


        [HttpGet]
        public async Task<IActionResult> ScheduleTable(int groupId)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["group"] = group.GroupName;
            ViewData["cource"] = group.GroupCource;
            return View(await db.Schedules.Where(s => s.Group.Id == groupId).ToListAsync());
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.userName.Equals(loginModel.userName) && u.password.Equals(loginModel.password));
                if (user != null)
                {
                    await Authenticate(user.userName);
                    return RedirectToAction("Index", "Home");
                } else
                {
                    ModelState.AddModelError("", "Некорректные логин или пароль");
                }
            }
            return View(loginModel);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> CreateGroup(string groupName, int groupCource)
        {
            await db.Groups.AddAsync(new Group { GroupName = groupName, GroupCource = groupCource });
            await db.SaveChangesAsync();
            for (int i = 1; i < 6; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    Group group = await db.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
                    await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id });
                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("GroupList", new {cource = groupCource});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int? groupId, int groupCource)
        {
            if (groupId != null)
            {
                foreach (var item in db.Schedules.Where(s => s.Group.Id == groupId))
                {
                    Schedule schedule = await db.Schedules.FindAsync(groupId);
                    db.Entry(schedule).State = EntityState.Deleted;
                }
                Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                db.Entry(group).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("GroupList", new { cource = groupCource });
            }
            return NotFound();
        }

        [HttpGet]
        public JsonResult getGroupInfo(int? id)
        {
            if (id != null)
            {
                Group group = db.Groups.FirstOrDefault(g => g.Id == id);
                if (group != null)
                {
                    var groupJson = JsonConvert.SerializeObject(group);
                    return Json(groupJson);
                }
            }
            return Json(NotFound());
        }


        [HttpPost]
        public async Task<IActionResult> EditGroup(int groupId, string groupName, int groupCource, int currentCource)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group != null)
            {
                var groupUpdate = db.Groups.Where(g => g.Id == groupId);
                groupUpdate.FirstOrDefault().GroupName = groupName;
                groupUpdate.FirstOrDefault().GroupCource = groupCource;
                await db.SaveChangesAsync();
                return RedirectToAction("GroupList", new {cource = currentCource });
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> EditSchedule(int groupId)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["group"] = group.GroupName;
            ViewData["cource"] = group.GroupCource;
            return View(await db.Schedules.Where(g => g.GroupId == groupId).ToListAsync());
        }

        [HttpPost]
        public JsonResult EditLessonTime(int? id, string time)
        {
            if(id != null)
            {
                Schedule schedule = db.Schedules.FirstOrDefault(s => s.Id == id);
                var updateSchedule = db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefault();
                updateSchedule.LessonTime = time;
                db.Schedules.UpdateRange(updateSchedule);
                db.SaveChanges();
                return Json("Время изменено");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public JsonResult EditRoom(int? id, string room)
        {
            if (id != null)
            {
                Schedule schedule = db.Schedules.FirstOrDefault(s => s.Id == id);
                var updateSchedule = db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefault();
                updateSchedule.Room = room;
                db.Schedules.UpdateRange(updateSchedule);
                db.SaveChanges();
                return Json("Кабинет изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public JsonResult EditDenominator(int? id, string denominator)
        {
            if (id != null)
            {
                Schedule schedule = db.Schedules.FirstOrDefault(s => s.Id == id);
                var updateSchedule = db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefault();
                updateSchedule.SubjectDenominator = denominator;
                db.Schedules.UpdateRange(updateSchedule);
                db.SaveChanges();
                return Json("Знаменатель изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public JsonResult EditNumerator(int? id, string numerator)
        {
            if (id != null)
            {
                Schedule schedule = db.Schedules.FirstOrDefault(s => s.Id == id);
                var updateSchedule = db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefault();
                updateSchedule.SubjectNumerator = numerator;
                db.Schedules.UpdateRange(updateSchedule);
                db.SaveChanges();
                return Json("Числитель изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public JsonResult EditTeacher(int? id, string teacher)
        {
            if (id != null)
            {
                Schedule schedule = db.Schedules.FirstOrDefault(s => s.Id == id);
                var updateSchedule = db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefault();
                updateSchedule.Teacher = teacher;
                db.Schedules.UpdateRange(updateSchedule);
                db.SaveChanges();
                return Json("Преподаватель изменен");
            }
            return Json(NotFound());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
