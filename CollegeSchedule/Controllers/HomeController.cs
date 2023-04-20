using CollegeSchedule.Models;
using CollegeSchedule.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            return View(await db.Schedules.Include(s => s.TeacherDenominator).Include(s => s.TeacherNumerator).Include(s => s.Group).Where(s => s.Group.Id == groupId).ToListAsync());
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
        public async Task<JsonResult> getGroupInfo(int? id)
        {
            if (id != null)
            {
                Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == id);
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
            SelectList teachers = new SelectList(db.Teachers, "Id", "fullTeacherName");
            ViewBag.teachers = teachers;
            var allModels = new AllModels();
            allModels.Schedules = await db.Schedules.Where(g => g.GroupId == groupId).ToListAsync();
            allModels.Teachers = await db.Teachers.ToListAsync();
            ViewData["group"] = group.GroupName;
            ViewData["cource"] = group.GroupCource;
            return View(allModels);
        }

        [HttpPost]
        public async Task<JsonResult> EditLessonTime(int? id, string time)
        {
            if(id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.LessonTime = time;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Время изменено");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditRoomDenominator(int? id, string roomDenominator)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.RoomDenominator = roomDenominator;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Числитель кабинета изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditRoomNumerator(int? id, string roomNumerator)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.RoomNumerator = roomNumerator;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Знаменатель кабинета изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditDenominator(int? id, string denominator)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.SubjectDenominator = denominator;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Знаменатель изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditNumerator(int? id, string numerator)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.SubjectNumerator = numerator;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Числитель изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditTeacherDenominator(int? id, int teacherDenominatorId)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.TeacherDenominatorId = teacherDenominatorId;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Числитель преподавателя изменен");
            }
            return Json(NotFound());
        }


        public async Task<JsonResult> EditTeacherNumerator(int? id, int teacherNumeratorId)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.TeacherNumeratorId = teacherNumeratorId;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Знаменатель преподавателя изменен");
            }
            return Json(NotFound());
        }

        public async Task<IActionResult> TeachersList()
        {
            return View(await db.Teachers.ToListAsync());
        }

        public async Task<IActionResult> AddTeacher(string teacherName)
        {
            if (ModelState.IsValid)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName == teacherName);
                if (teacher == null)
                {
                    await db.Teachers.AddAsync(new Teacher { teacherFullName = teacherName});
                    await db.SaveChangesAsync();
                    return RedirectToAction("TeachersList");
                } else
                {
                    ModelState.AddModelError("", "Преподаватель с таким ФИО уже добавлен");
                }
            }
            return View(teacherName);
        }


        public async Task<JsonResult> GetTeacherInfo(int? id)
        {
            Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == id);
            if (teacher != null)
            {
                var teacherJson = JsonConvert.SerializeObject(teacher);
                return Json(teacherJson);
            }
            return Json(NotFound());
        }

        public async Task<IActionResult> EditTeacher(int? id, string teacherName)
        {
            if (id != null)
            {
                if (ModelState.IsValid)
                {
                    Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName == teacherName);
                    if (teacher == null)
                    {
                        var teacherUpdate = db.Teachers.Where(t => t.Id == id).AsQueryable().FirstOrDefault();
                        teacherUpdate.teacherFullName = teacherName;
                        db.Teachers.UpdateRange(teacherUpdate);
                        await db.SaveChangesAsync();
                        return RedirectToAction("TeachersList");
                    }
                    ModelState.AddModelError("", "Преподаватель с таким ФИО уже добавлен");
                } else
                {
                    return View(teacherName);
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> TeachersSchedule(int? teacherId)
        {
            if (teacherId != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
                ViewData["teacherName"] = teacher.teacherFullName;
                ViewData["MondayLessons"] = db.Schedules.Count(s => s.DayOfTheWeek == 1 && s.TeacherDenominator.Id == teacherId) + 1;
                ViewData["TuesdayLessons"] = db.Schedules.Count(s => s.DayOfTheWeek == 2 && s.TeacherDenominator.Id == teacherId) + 1;
                ViewData["WednesdayLessons"] = db.Schedules.Count(s => s.DayOfTheWeek == 3 && s.TeacherDenominator.Id == teacherId) + 1;
                ViewData["ThursdayLessons"] = db.Schedules.Count(s => s.DayOfTheWeek == 4 && s.TeacherDenominator.Id == teacherId) + 1;
                ViewData["FridayLessons"] = db.Schedules.Count(s => s.DayOfTheWeek == 5 && s.TeacherDenominator.Id == teacherId) + 1;
                Console.WriteLine(db.Schedules.Count(s => s.DayOfTheWeek == 1 && s.TeacherDenominator.Id == teacherId));
                return View(await db.Schedules.Include(s => s.TeacherDenominator).Include(s => s.TeacherNumerator).Include(s => s.Group).Where(s => s.TeacherDenominatorId == teacherId || s.TeacherNumeratorId == teacherId ).ToListAsync());
            }
            return NotFound();
        }

        public async Task<IActionResult> DeleteTeacher(int? teacherId)
        {
            if (teacherId != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
                foreach(var item in db.Schedules.Where(s => s.TeacherDenominatorId == teacherId))
                {
                    Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == item.Id);
                    if (schedule != null)
                    {
                        var updateSchedule = await db.Schedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                        updateSchedule.TeacherDenominatorId = null;
                        db.Schedules.UpdateRange(updateSchedule);
                        await db.SaveChangesAsync();
                    }
                }
                foreach(var item in db.Schedules.Where(s => s.TeacherNumeratorId == teacherId))
                {
                    Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == item.Id);
                    if (schedule != null)
                    {
                        var updateSchedule = await db.Schedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                        updateSchedule.TeacherNumeratorId = null;
                        db.Schedules.UpdateRange(updateSchedule);
                        await db.SaveChangesAsync();
                    }
                }
                db.Entry(teacher).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("TeachersList");
            }
            return NotFound();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
