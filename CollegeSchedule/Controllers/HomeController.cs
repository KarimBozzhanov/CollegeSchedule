using CollegeSchedule.Models;
using CollegeSchedule.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XSystem.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using System.IO;
using Aspose.Cells;
using XAct;

namespace CollegeSchedule.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db;
        IWebHostEnvironment app;
 
        public HomeController(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;
            app = appEnvironment;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.userName.Equals(loginModel.userName) && u.password.Equals(loginModel.password));
                if (user != null)
                {
                    await Authenticate(user.userName);
                    return RedirectToAction("Index", "Home");
                }
                else
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

        public async Task<IActionResult> GroupList(int course)
        {
            ViewData["course"] = course;
            return View(await db.Groups.Where(g => g.GroupCourse == course).OrderBy(g => g.GroupName).ToListAsync());
        }

        [HttpPost]
        public async Task<JsonResult> CreateGroup(string groupName, int groupCourse, int groupShift)
        {
            Console.WriteLine("GroupName - " + groupName + "\nCourse - " + groupCourse + "\nShift - " + groupShift);
            Group collegeGroup = await db.Groups.FirstOrDefaultAsync(g => g.GroupName.ToLower() == groupName.ToLower());
            if (collegeGroup == null)
            {
                await db.Groups.AddAsync(new Group() { GroupName = groupName, GroupCourse = groupCourse, GroupShift = groupShift });
                await db.SaveChangesAsync();
                Group group = await db.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
                if (group != null)
                {
                    for (int i = 1; i < 6; i++)
                    {
                        for (int j = 1; j < 5; j++)
                        {
                            if (groupShift == 1)
                            {
                                switch (j)
                                {
                                    case 1:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "07:50", EndOfLesson = "09:19" });
                                        break;
                                    case 2:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "09:20", EndOfLesson = "10:49" });
                                        break;
                                    case 3:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "10:50", EndOfLesson = "12:19" });
                                        break;
                                    case 4:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "12:20", EndOfLesson = "13:50" });
                                        break;
                                }
                            }
                            else if (groupShift == 2)
                            {
                                switch (j)
                                {
                                    case 1:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "12:50", EndOfLesson = "14:19" });
                                        break;
                                    case 2:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "14:20", EndOfLesson = "15:49" });
                                        break;
                                    case 3:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "15:50", EndOfLesson = "17:19" });
                                        break;
                                    case 4:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "17:20", EndOfLesson = "18:50" });
                                        break;
                                }
                            }
                        }
                    }

                    await db.SaveChangesAsync();
                    return Json("Группа сохранена");
                }
                else
                {
                    return Json("Не удалось сохранить группу");
                }
            }
            else
            {
                return Json("Группа с таким именем уже создана");
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteGroup(int? groupId)
        {
            if (groupId != null)
            {
                foreach (var item in db.Schedules.Where(s => s.Group.Id == groupId))
                {
                    foreach(var teachersSchedule in db.TeachersSchedules.Where(t => t.ScheduleDenominatorId == item.Id || t.ScheduleNumeratorId == item.Id))
                    {
                        var updateTeachersSchedule = await db.TeachersSchedules.Where(t => t.Id == teachersSchedule.Id).AsQueryable().FirstOrDefaultAsync();
                        if (teachersSchedule.ScheduleDenominatorId == item.Id)
                        {
                            updateTeachersSchedule.ScheduleDenominatorId = null;
                        } else if(teachersSchedule.ScheduleNumeratorId == item.Id)
                        {
                            updateTeachersSchedule.ScheduleNumeratorId = null;
                        }
                    }
                    Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == item.Id);
                    db.Entry(schedule).State = EntityState.Deleted;
                }
                foreach (var consultation in db.ConsultationsSchedules.Where(c => c.GroupId == groupId))
                {
                    ConsultationSchedule consultationSchedule = await db.ConsultationsSchedules.FirstOrDefaultAsync(s => s.Id == consultation.Id);
                    db.Entry(consultationSchedule).State = EntityState.Deleted;
                }
                foreach (var exam in db.ExamsSchedules.Where(c => c.GroupId == groupId))
                {
                    ExamsSchedule examsSchedule = await db.ExamsSchedules.FirstOrDefaultAsync(s => s.Id == exam.Id);
                    db.Entry(examsSchedule).State = EntityState.Deleted;
                }
                foreach (var practice in db.PracticeSchedules.Where(c => c.GroupId == groupId))
                {
                    PracticeSchedule practiceSchedule = await db.PracticeSchedules.FirstOrDefaultAsync(s => s.Id == practice.Id);
                    db.Entry(practiceSchedule).State = EntityState.Deleted;
                }
                Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                db.Entry(group).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return Json("Группа удалена");
            }
            return Json("Не удалось удалить группу");
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
        public async Task<IActionResult> EditGroup(int groupId, string groupName, int groupCourse, int currentCourse)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group != null)
            {
                var groupUpdate = db.Groups.Where(g => g.Id == groupId);
                groupUpdate.FirstOrDefault().GroupName = groupName;
                groupUpdate.FirstOrDefault().GroupCourse = groupCourse;
                await db.SaveChangesAsync();
                return RedirectToAction("GroupList", new { course = currentCourse });
            }
            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> ScheduleTable(int groupId)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["groupId"] = group.Id;
            ViewData["group"] = group.GroupName;
            ViewData["course"] = group.GroupCourse;
            return View(await db.Schedules
                .Include(s => s.SubjectDenominator)
                .Include(s => s.SubjectNumerator)
                .Include(s => s.TeacherDenominator)
                .Include(s => s.TeacherNumerator)
                .Include(s => s.Group)
                .Where(s => s.Group.Id == groupId)
                .OrderBy(s => s.SubjectNumber)
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> EditSchedule(int groupId)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            var allModels = new AllModels();
            allModels.Schedules = await db.Schedules
                .Include(s => s.TeacherDenominator)
                .Include(s => s.TeacherNumerator)
                .Include(s => s.SubjectDenominator)
                .Include(s => s.SubjectNumerator)
                .Where(g => g.GroupId == groupId)
                .OrderBy(s => s.SubjectNumber)
                .ToListAsync();
            allModels.Teachers = await db.Teachers.OrderBy(t => t.teacherFullName).ToListAsync();
            allModels.Subjects = await db.Subjects.ToListAsync();
            ViewData["group"] = group.GroupName;
            ViewData["course"] = group.GroupCourse;
            return View(allModels);
        }

        [HttpPost]
        public async Task<JsonResult> EditStartOfLesson(int? id, string time)
        {
            if(id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.StartOfLesson = time;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Время начала пары изменено");
            }
            return Json(NotFound());
        }


        [HttpPost]
        public async Task<JsonResult> EditEndOfLesson(int? id, string time)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateSchedule.EndOfLesson = time;
                db.Schedules.UpdateRange(updateSchedule);
                await db.SaveChangesAsync();
                return Json("Время окончания пары изменено");
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
                if(schedule != null)
                {
                    if(denominator != null)
                    {
                        Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.SubjectName.Equals(denominator));
                        if(subject != null)
                        {
                            var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                            updateSchedule.SubjectDenominatorId = subject.Id;
                            db.Schedules.UpdateRange(updateSchedule);
                        } else
                        {
                            return Json("Предмет не найден");
                        }
                    } else
                    {
                        var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                        updateSchedule.SubjectDenominatorId = null;
                        db.Schedules.UpdateRange(updateSchedule);
                    }
                    await db.SaveChangesAsync();
                    return Json("Знаменатель изменен");
                } else
                {
                    return Json("Расписание не найдено");
                }
            } else
            {
                return Json(NotFound());
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditNumerator(int? id, string numerator)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
                if (schedule != null)
                {
                    if (numerator != null)
                    {
                        Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.SubjectName.Equals(numerator));
                        if (subject != null)
                        {
                            var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                            updateSchedule.SubjectNumeratorId = subject.Id;
                            db.Schedules.UpdateRange(updateSchedule);
                        }
                        else
                        {
                            return Json("Предмет не найден");
                        }
                    }
                    else
                    {
                        var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                        updateSchedule.SubjectNumeratorId = null;
                        db.Schedules.UpdateRange(updateSchedule);
                    }
                    await db.SaveChangesAsync();
                    return Json("Числитель изменен");
                }
                else
                {
                    return Json("Расписание не найдено");
                }
            }
            else
            {
                return Json(NotFound());
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditTeacherDenominator(int? id, string teacherName)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
                if (schedule != null)
                {
                    if (teacherName != null)
                    {
                        Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName.Equals(teacherName));
                        if (teacher != null)
                        {
                            var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                            updateSchedule.TeacherDenominatorId = teacher.Id;
                            db.Schedules.UpdateRange(updateSchedule);
                            int? groupShift = schedule.Group?.GroupShift;
                            var updateTeachersSchedule = await db.TeachersSchedules.Where(t => t.TeacherId == teacher.Id && t.DayOfTheWeek == schedule.DayOfTheWeek && t.SubjectNumber == schedule.SubjectNumber && t.Shift == groupShift).AsQueryable().FirstOrDefaultAsync();
                            updateTeachersSchedule.ScheduleDenominatorId = schedule.Id;
                            db.TeachersSchedules.UpdateRange(updateTeachersSchedule);
                        }
                        else
                        {
                            return Json("Такого преподавателя нет");
                        }
                    }
                    else
                    {
                        var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                        updateSchedule.TeacherDenominatorId = null;
                        db.Schedules.UpdateRange(updateSchedule);
                        var updateTeacherSchedule = await db.TeachersSchedules.Where(t => t.ScheduleDenominatorId == schedule.Id).AsQueryable().FirstOrDefaultAsync();
                        updateTeacherSchedule.ScheduleDenominatorId = null;
                        db.TeachersSchedules.UpdateRange(updateTeacherSchedule);
                    }
                    await db.SaveChangesAsync();
                    return Json("Числитель преподавателя изменен");
                } else
                {
                    return Json("Такой пары нет");
                }
            }
            return Json(NotFound());
        }


        public async Task<JsonResult> EditTeacherNumerator(int? id, string teacherName)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
                if (schedule != null)
                {
                    if (teacherName != null)
                    {
                        Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName.Equals(teacherName));
                        if (teacher != null)
                        {
                            var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                            updateSchedule.TeacherNumeratorId = teacher.Id;
                            db.Schedules.UpdateRange(updateSchedule);
                            int? groupShift = schedule.Group?.GroupShift;
                            var updateTeachersSchedule = await db.TeachersSchedules.Where(t => t.TeacherId == teacher.Id && t.DayOfTheWeek == schedule.DayOfTheWeek && t.SubjectNumber == schedule.SubjectNumber && t.Shift == groupShift).AsQueryable().FirstOrDefaultAsync();
                            updateTeachersSchedule.ScheduleNumeratorId = schedule.Id;
                            db.TeachersSchedules.UpdateRange(updateTeachersSchedule);
                        }
                        else
                        {
                            return Json("Такого преподавателя нет");
                        }
                    }
                    else
                    {
                        var updateSchedule = await db.Schedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                        updateSchedule.TeacherNumeratorId = null;
                        db.Schedules.UpdateRange(updateSchedule);
                        var updateTeacherSchedule = await db.TeachersSchedules.Where(t => t.ScheduleNumeratorId == schedule.Id).AsQueryable().FirstOrDefaultAsync();
                        updateTeacherSchedule.ScheduleNumeratorId = null;
                        db.TeachersSchedules.UpdateRange(updateTeacherSchedule);
                    }
                    await db.SaveChangesAsync();
                    return Json("Знаменатель преподавателя изменен");
                }
                else
                {
                    return Json("Такой пары нет");
                }
            }
            return Json(NotFound());
        }

        public async Task<IActionResult> TeachersList(string searchText)
        {
            if(!String.IsNullOrEmpty(searchText))
            {
                return View(await db.Teachers.Where(t => t.teacherFullName.Contains(searchText)).OrderBy(t => t.teacherFullName).ToListAsync());
            } else
            {
                return View(await db.Teachers.OrderBy(t => t.teacherFullName).ToListAsync());
            }
        }

        public async Task<JsonResult> AddTeacher(string teacherName)
        {
            Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName == teacherName);
            if (teacher == null)
            {
                string password = GetHash(teacherName);
                await db.Teachers.AddAsync(new Teacher { teacherFullName = teacherName, TeacherPassword = password });
                await db.SaveChangesAsync();
                Teacher newTeacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName.Equals(teacherName));
                Console.WriteLine(newTeacher);
                for (int i = 1; i <= 5; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        await db.TeachersSchedules.AddAsync(new TeachersSchedule { DayOfTheWeek = i, SubjectNumber = j, Shift = 1, TeacherId = newTeacher.Id });
                        await db.TeachersSchedules.AddAsync(new TeachersSchedule { DayOfTheWeek = i, SubjectNumber = j, Shift = 2, TeacherId = newTeacher.Id });
                    }
                }
                await db.SaveChangesAsync();
                return Json("Преподаватель добавлен");

            }
            else
            {
                return Json("Преподаватель с таким именем уже добавлен");
            }
        }

        public string GetHash(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();
            byte[] byteHash = CSP.ComputeHash(bytes);
            string hash = string.Empty;
            foreach (byte b in byteHash)
            {
                hash += string.Format("{0:x2}", b);
            }
            return hash;
        }


        public async Task<JsonResult> GetTeacherInfo(int? id)
        {
            if(id != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == id);
                if (teacher != null)
                {
                    var teacherJson = JsonConvert.SerializeObject(teacher);
                    return Json(teacherJson);
                }
                return Json(NotFound());
            } else
            {
                return Json(NotFound());
            }
        }

        public async Task<JsonResult> CheckTeacherPass(int? teacherId, string teacherPass)
        {
            if(teacherId != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
                if(teacher != null)
                {
                    if(teacherPass == teacher.TeacherPassword)
                    {
                        return Json(new { success = true, teacherId = teacherId, teacherPass = teacherPass });
                    } else
                    {
                        return Json(new {success = false, Message = "Неверный пароль"});
                    }
                } else
                {
                    return Json("Преподаватель не найден");
                }
            } else
            {
                return Json("Преподаватель не найден");
            }
        }

        public async Task<IActionResult> EditTeacher(int? teacherId, string teacherName)
        {
            if (teacherId != null)
            {
                if (ModelState.IsValid)
                {
                    Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName == teacherName);
                    if (teacher == null)
                    {
                        var teacherUpdate = db.Teachers.Where(t => t.Id == teacherId).AsQueryable().FirstOrDefault();
                        teacherUpdate.teacherFullName = teacherName;
                        db.Teachers.UpdateRange(teacherUpdate);
                        await db.SaveChangesAsync();
                        return RedirectToAction("TeachersList");
                    }
                    ModelState.AddModelError("", "Преподаватель с таким ФИО уже добавлен");
                }
                else
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
                if (teacher != null)
                {
                    ViewData["teacherName"] = teacher.teacherFullName;
                    ViewData["teacherId"] = teacher.Id;
                    ViewBag.Pass = teacher.TeacherPassword;
                    ViewBag.teacherId = teacher.Id;
                    return View(await db.TeachersSchedules
                        .Where(t => t.TeacherId == teacherId)
                        .Include(t => t.ScheduleDenominator)
                        .Include(t => t.ScheduleNumerator)
                        .Include(t => t.ScheduleDenominator.Group)
                        .Include(t => t.ScheduleDenominator.SubjectDenominator)
                        .Include(t => t.ScheduleNumerator.Group)
                        .Include(t => t.ScheduleNumerator.SubjectNumerator)
                        .OrderBy(t => t.SubjectNumber)
                        .ToListAsync());
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> DeleteTeacher(int? teacherId)
        {
            if (teacherId != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
                foreach (var item in db.Schedules.Where(s => s.TeacherDenominatorId == teacher.Id || s.TeacherNumeratorId == teacher.Id))
                {
                    var updateSchedule = await db.Schedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                    if (item.TeacherNumeratorId == teacher.Id)
                    {
                        updateSchedule.TeacherNumeratorId = null;
                    }
                    else if (item.TeacherDenominatorId == teacher.Id)
                    {
                        updateSchedule.TeacherDenominatorId = null;
                    }
                }
                foreach (var item in db.ExamsSchedules.Where(s => s.TeacherId == teacher.Id))
                {
                    var updateExamSchedule = await db.ExamsSchedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                    updateExamSchedule.TeacherId = null;
                }
                foreach (var item in db.ConsultationsSchedules.Where(s => s.TeacherId == teacher.Id))
                {
                    var updateConsultationSchedule = await db.ConsultationsSchedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                    updateConsultationSchedule.TeacherId = null;
                }
                foreach (var item in db.PracticeSchedules.Where(s => s.TeacherId == teacher.Id))
                {
                    var updatePracticeSchedule = await db.PracticeSchedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                    updatePracticeSchedule.TeacherId = null;
                }
                foreach (var item in db.TeachersSchedules.Where(t => t.TeacherId == teacher.Id))
                {
                    db.Entry(item).State = EntityState.Deleted;
                }
                db.Entry(teacher).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("TeachersList");
            }
            return NotFound();
        }

        public async Task<JsonResult> ChangeTeacherPass(int? teacherId, string newTeacherPass)
        {
            if (teacherId != null)
            {
                var teacher = await db.Teachers.Where(t => t.Id == teacherId).AsQueryable().FirstOrDefaultAsync();
                if (teacher != null)
                {
                    if (newTeacherPass.Length > 2 && newTeacherPass.Length < 10)
                    {
                        teacher.TeacherPassword = newTeacherPass;
                        db.Teachers.UpdateRange(teacher);
                        await db.SaveChangesAsync();
                        return Json("Пароль изменен");
                    }
                    else
                    {
                        return Json("Пароль должен быть не меньше 2 и не больше 10 символов");
                    }
                }
                else
                {
                    return Json("Преподаватель не найден");
                }
            }
            else
            {
                return Json("Преподаватель не найден");
            }
        }


        public async Task<IActionResult> ExamsListOfGroups(string searchText, int course)
        {
            ViewData["course"] = course;
            if (!String.IsNullOrEmpty(searchText))
            {
                Console.WriteLine(searchText);
                return View(await db.Groups.Where(g => g.GroupName.Contains(searchText) && g.GroupCourse == course).OrderBy(g => g.GroupName).ToListAsync());
            }
            else
            {
                return View(await db.Groups.Where(g => g.GroupCourse == course).OrderBy(g => g.GroupName).ToListAsync());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExamsSchedule(int groupId)
        {
            int examsCount = db.ExamsSchedules.Count(e => e.GroupId == groupId);
            Console.WriteLine(examsCount);
            ViewBag.examsCount = examsCount;
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["course"] = group.GroupCourse;
            ViewData["groupName"] = group.GroupName;
            var allModels = new AllModels();
            allModels.ExamsSchedules = await db.ExamsSchedules
                .Include(e => e.Teacher)
                .Include(e => e.ExamSubject)
                .Include(e => e.Group)
                .Where(e => e.GroupId == groupId)
                .ToListAsync();
            allModels.ConsultationSchedules = await db.ConsultationsSchedules.Include(e => e.Teacher).Where(e => e.GroupId == groupId).ToListAsync();
            return View(allModels);
        }


        [HttpGet]
        public async Task<IActionResult> EditExamsSchedule(int groupId)
        {
            int examsCount = db.ExamsSchedules.Count(e => e.GroupId == groupId);
            int consultationsCount = db.ConsultationsSchedules.Count(c => c.GroupId == groupId);
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["course"] = group.GroupCourse;
            ViewData["groupName"] = group.GroupName;
            Console.WriteLine(examsCount);
            Console.WriteLine(consultationsCount);
            ViewBag.examsCount = examsCount;
            ViewBag.consultationsCount = consultationsCount;
            ViewData["groupId"] = groupId;
            var allModels = new AllModels();
            allModels.Teachers = await db.Teachers.OrderBy(t => t.teacherFullName).ToListAsync();
            allModels.ExamsSchedules = await db.ExamsSchedules
                .Include(e => e.ExamSubject)
                .Include(e => e.Teacher)
                .Include(e => e.Group)
                .Where(e => e.GroupId == groupId)
                .ToListAsync();
            allModels.ConsultationSchedules = await db.ConsultationsSchedules.Where(c => c.GroupId == groupId).ToListAsync();
            allModels.Subjects = await db.Subjects.ToListAsync();
            return View(allModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddExam(int groupId, int examsCount)
        {
            await db.ExamsSchedules.AddAsync(new ExamsSchedule { GroupId = groupId, ExamNumber = examsCount + 1});
            await db.SaveChangesAsync();
            return RedirectToAction("EditExamsSchedule", new { groupId = groupId});
        }

        public async Task<IActionResult> DeleteExam(int examId, int groupId)
        {
            ExamsSchedule exam = await db.ExamsSchedules.FirstOrDefaultAsync(e => e.Id == examId);
            if (exam != null)
            {
                db.Entry(exam).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("EditExamsSchedule", new { groupId = groupId});
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<JsonResult> EditExamDate(int? id, string date)
        {
            if (id != null)
            {
                ExamsSchedule exam = await db.ExamsSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updateExam = await db.ExamsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateExam.Date = date;
                db.ExamsSchedules.UpdateRange(updateExam);
                await db.SaveChangesAsync();
                return Json("Дата экзамена изменена");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditExamTime(int? id, string time)
        {
            if (id != null)
            {
                ExamsSchedule exam = await db.ExamsSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updateExam = await db.ExamsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateExam.Time = time;
                db.ExamsSchedules.UpdateRange(updateExam);
                await db.SaveChangesAsync();
                return Json("Время экзамена изменено");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditExamRoom(int? id, string room)
        {
            if (id != null)
            {
                ExamsSchedule exam = await db.ExamsSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updateExam = await db.ExamsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateExam.Room = room;
                db.ExamsSchedules.UpdateRange(updateExam);
                await db.SaveChangesAsync();
                return Json("Кабинет экзамена изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditExamSubject(int? id, string subjectName)
        {
            if (id != null)
            {
                var updateExam = await db.ExamsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                if(subjectName != null)
                {
                    Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.SubjectName.Equals(subjectName));
                    if (subject != null)
                    {
                        updateExam.ExamSubjectId = subject.Id;
                    } else
                    {
                        return Json("Предмет не найден");
                    }
                } else
                {
                    updateExam.ExamSubject = null;
                }
                db.ExamsSchedules.UpdateRange(updateExam);
                await db.SaveChangesAsync();
                return Json("Предмет экзамена изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditExamTeacher(int? id, string teacherName)
        {
            if (id != null)
            {
                var updateExam = await db.ExamsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                if (teacherName != null)
                {
                    Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName.Equals(teacherName));
                    if(teacher != null)
                    {
                        updateExam.TeacherId = teacher.Id;
                    } else
                    {
                        return Json("Преподаватель не найден");
                    }
                } else
                {
                    updateExam.TeacherId = null;
                }
                db.ExamsSchedules.UpdateRange(updateExam);
                await db.SaveChangesAsync();
                return Json("Преподаватель экзамена изменен");
            }
            return Json(NotFound());
        }

        public async Task<IActionResult> PracticeGroupsList(string searchText, int course)
        {
            ViewData["course"] = course;
            if (!String.IsNullOrEmpty(searchText))
            {
                Console.WriteLine(searchText);
                return View(await db.Groups.Where(g => g.GroupName.Contains(searchText) && g.GroupCourse == course).OrderBy(g => g.GroupName).ToListAsync());
            }
            else
            {
                return View(await db.Groups.Where(g => g.GroupCourse == course).OrderBy(g => g.GroupName).ToListAsync());
            }
        }

        public async Task<IActionResult> PracticeSchedule(int groupId)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["course"] = group.GroupCourse;
            ViewData["groupName"] = group.GroupName;
            return View(await db.PracticeSchedules.Include(p => p.Teacher).Where(p => p.GroupId == groupId).ToListAsync());
        }

        public async Task<IActionResult> EditPracticeSchedule(int groupId)
        {
            int practiceCount = db.PracticeSchedules.Count(p => p.GroupId == groupId);
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["course"] = group.GroupCourse;
            Console.WriteLine(practiceCount);
            ViewBag.practiceCount = practiceCount;
            ViewData["groupId"] = groupId;
            ViewData["groupName"] = group.GroupName;
            var allModels = new AllModels();
            allModels.Teachers = await db.Teachers.OrderBy(t => t.teacherFullName).ToListAsync();
            allModels.PracticesSchedule = await db.PracticeSchedules.Where(p => p.GroupId == groupId).ToListAsync();
            return View(allModels);
        }

        [HttpPost]
        public async Task<JsonResult> EditPracticeStartDate(int? id, string startDate)
        {
            if (id != null)
            {
                PracticeSchedule practice = await db.PracticeSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updatePractice = await db.PracticeSchedules.Where(p => p.Id == id).AsQueryable().FirstOrDefaultAsync();
                updatePractice.StartDate = startDate;
                db.PracticeSchedules.UpdateRange(updatePractice);
                await db.SaveChangesAsync();
                return Json("Дата начала практики изменена");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditPracticeEndDate(int? id, string endDate)
        {
            if (id != null)
            {
                PracticeSchedule practice = await db.PracticeSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updatePractice = await db.PracticeSchedules.Where(p => p.Id == id).AsQueryable().FirstOrDefaultAsync();
                updatePractice.EndDate = endDate;
                db.PracticeSchedules.UpdateRange(updatePractice);
                await db.SaveChangesAsync();
                return Json("Дата окончания практики изменена");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditPracticeName(int? id, string name)
        {
            if (id != null)
            {
                PracticeSchedule practice = await db.PracticeSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updatePractice = await db.PracticeSchedules.Where(p => p.Id == id).AsQueryable().FirstOrDefaultAsync();
                updatePractice.PracticeName = name;
                db.PracticeSchedules.UpdateRange(updatePractice);
                await db.SaveChangesAsync();
                return Json("Название практики изменено");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditPracticeTeacher(int? id, string teacherName)
        {
            if (id != null)
            {
                PracticeSchedule practice = await db.PracticeSchedules.FirstOrDefaultAsync(e => e.Id == id);
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName.Equals(teacherName));
                var updatePractice = await db.PracticeSchedules.Where(p => p.Id == id).AsQueryable().FirstOrDefaultAsync();
                updatePractice.TeacherId = teacher.Id;
                db.PracticeSchedules.UpdateRange(updatePractice);
                await db.SaveChangesAsync();
                return Json("Преподаватель по практике изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<IActionResult> AddPractice(int groupId, int practiceCount)
        {
            await db.PracticeSchedules.AddAsync(new PracticeSchedule { GroupId = groupId, PracticeNumber = practiceCount + 1 });
            await db.SaveChangesAsync();
            return RedirectToAction("EditPracticeSchedule", new { groupId = groupId });
        }

        public async Task<IActionResult> DeletePractice(int practiceId, int groupId)
        {
            PracticeSchedule practice = await db.PracticeSchedules.FirstOrDefaultAsync(p => p.Id == practiceId);
            if (practice != null)
            {
                db.Entry(practice).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("EditPracticeSchedule", new { groupId = groupId });
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> AddConsultation(int groupId, int consultationsCount)
        {
            await db.ConsultationsSchedules.AddAsync(new ConsultationSchedule { GroupId = groupId,  ConsultationNumber = consultationsCount + 1});
            await db.SaveChangesAsync();
            return RedirectToAction("EditExamsSchedule", new { groupId = groupId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConsultation(int consultationId, int groupId)
        {
            ConsultationSchedule consultation = await db.ConsultationsSchedules.FirstOrDefaultAsync(p => p.Id == consultationId);
            if (consultation != null)
            {
                db.Entry(consultation).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("EditExamsSchedule", new { groupId = groupId });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<JsonResult> EditConsultationDate(int? id, string date)
        {
            if (id != null)
            {
                ConsultationSchedule consultation = await db.ConsultationsSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updateConsultation = await db.ConsultationsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateConsultation.Date = date;
                db.ConsultationsSchedules.UpdateRange(updateConsultation);
                await db.SaveChangesAsync();
                return Json("Дата консультации изменена");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditConsultationTime(int? id, string time)
        {
            if (id != null)
            {
                var updateConsultation = await db.ConsultationsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateConsultation.Time = time;
                db.ConsultationsSchedules.UpdateRange(updateConsultation);
                await db.SaveChangesAsync();
                return Json("Время консультации изменено");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditConsultationRoom(int? id, string room)
        {
            if (id != null)
            {
                var updateConsultation = await db.ConsultationsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateConsultation.Room = room;
                db.ConsultationsSchedules.UpdateRange(updateConsultation);
                await db.SaveChangesAsync();
                return Json("Кабинет консультации изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditConsultationSubject(int? id, string subjectName)
        {
            if (id != null)
            {
                var updateConsultation = await db.ConsultationsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                if (subjectName != null)
                {
                    Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.SubjectName.Equals(subjectName));
                    if(subject != null)
                    {
                        updateConsultation.ConsultationSubjectId = subject.Id;
                    } else
                    {
                        return Json("Предмет не найден");
                    }
                } else
                {
                    updateConsultation.ConsultationSubjectId = null;
                }
                db.ConsultationsSchedules.UpdateRange(updateConsultation);
                await db.SaveChangesAsync();
                return Json("Предмет консультации изменен");

            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditConsultationTeacher(int? id, string teacherName)
        {
            if (id != null)
            {
                var updateConsultation = await db.ConsultationsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                if(teacherName != null)
                {
                    Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.teacherFullName.Equals(teacherName));
                    if(teacher != null)
                    {
                        updateConsultation.TeacherId = teacher.Id;
                    } else
                    {
                        return Json("Преподаватель не найден");
                    }
                } else
                {
                    updateConsultation.TeacherId = null;
                }
                db.ConsultationsSchedules.UpdateRange(updateConsultation);
                await db.SaveChangesAsync();
                return Json("Преподаватель консультации изменен");
            }
            return Json(NotFound());
        }
        
        public async Task<JsonResult> GetSchedules(int? id)
        {
            Console.WriteLine("Id - " + id);
            List<Schedule> allSchedule = await db.Schedules.Where(s => s.GroupId == id).OrderBy(a => a.DayOfTheWeek).ToListAsync();
            allSchedule = allSchedule.OrderBy(a => a.SubjectNumber).ToList();
            var scheduleJson = JsonConvert.SerializeObject(allSchedule);
            return Json(scheduleJson);
        }


        public async Task<IActionResult> SubjectsList(string searchText)
        {
            if(!String.IsNullOrEmpty(searchText))
            {
                return View(await db.Subjects.Where(s => s.SubjectName.Contains(searchText)).OrderBy(s => s.SubjectName).ToListAsync());
            } else
            {
                return View(await db.Subjects.OrderBy(s => s.SubjectName).ToListAsync());
            }
        }

        public async Task<JsonResult> AddSubject(string subjectName)
        {
            Console.WriteLine("SubjectName - " + subjectName);
            Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.SubjectName.Equals(subjectName));
            if (subject == null)
            {
                await db.Subjects.AddAsync(new Subject { SubjectName = subjectName });
                await db.SaveChangesAsync();
                return Json("Предмет добавлен");
            }
            else
            {
                return Json("Предмет с таким названием уже добавлен");
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int? subjectId)
        {
            if (subjectId != null)
            {
                Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
                if (subject != null)
                {
                    foreach (var item in db.Schedules.Where(s => s.SubjectDenominatorId == subject.Id || s.SubjectNumeratorId == subject.Id))
                    {
                        var updateSchedule = await db.Schedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                        if(item.SubjectDenominatorId == subject.Id)
                        {
                            updateSchedule.SubjectDenominatorId = null;
                        } else if(item.SubjectNumeratorId == subject.Id)
                        {
                            updateSchedule.SubjectNumeratorId = null;
                        } else if(item.SubjectDenominatorId == subject.Id && item.SubjectNumeratorId == subject.Id)
                        {
                            updateSchedule.SubjectDenominatorId = null;
                            updateSchedule.SubjectNumeratorId = null;
                        }
                    }
;                   db.Entry(subject).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                    return RedirectToAction("SubjectsList");
                } else
                {
                    return NotFound();
                }
            } else
            {
                return NotFound();
            }
        }


        public async Task<JsonResult> GetSubjectInfo(int? subjectId)
        {
            if(subjectId != null)
            {
                Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
                return Json(subject);
            }
            return Json(NotFound());
        }

        public async Task<JsonResult> UpdateSubject(int? subjectId, string subjectName)
        {
            if(subjectId != null)
            {
                Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
                if(subject != null)
                {
                    if(!subjectName.Equals(""))
                    {
                        Subject recurringSubject = await db.Subjects.FirstOrDefaultAsync(s => s.SubjectName.Equals(subjectName));
                        if(recurringSubject == null)
                        {
                            var updateSubject = await db.Subjects.Where(s => s.Id == subjectId).AsQueryable().FirstOrDefaultAsync();
                            updateSubject.SubjectName = subjectName;
                            db.Subjects.UpdateRange(updateSubject);
                            await db.SaveChangesAsync();
                            return Json("Предмет изменен");
                        } else
                        {
                            return Json("Такой предмет уже добавлен");
                        }
                    } else
                    {
                        return Json("Введите название предмета");
                    }
                } else
                {
                    return Json("Предмет не найден");
                }
            } else
            {
                return Json("Предмет не найден");
            }
        }


        [HttpPost]
        public async Task<JsonResult> ImportSubjects(IFormFile excelFile)
        {
            if (excelFile != null)
            {
                string path = app.WebRootPath + "/ExcelFiles/" + excelFile.FileName;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await excelFile.CopyToAsync(fileStream);
                }
                Workbook wb = new Workbook(path);
                WorksheetCollection collection = wb.Worksheets;

                Worksheet worksheet = collection[0];
                Console.WriteLine("Worksheet - " + worksheet.Name);

                int rows = worksheet.Cells.MaxDataRow;
                int cols = worksheet.Cells.MaxDataColumn;

                for (int i = 0; i <= rows; i++)
                {
                    string newSubject = (string)worksheet.Cells[i, 0].Value;
                    if (newSubject != null)
                    {
                        Subject subject = await db.Subjects.FirstOrDefaultAsync(s => s.SubjectName.Equals(newSubject));
                        if (subject == null)
                        {
                            await db.Subjects.AddAsync(new Subject { SubjectName = newSubject });
                        }
                    }
                }
                await db.SaveChangesAsync();
                FileInfo fileInfo = new FileInfo(path);

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                return Json("success");
            }
            else
            {
                return Json("unsuccessfull");
            }
        }


        public async Task<ActionResult> ExportTeacherSchedule(int? teacherId)
        {
            if(teacherId != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
                if(teacher != null)
                {
                    Workbook wb = new Workbook();
                    Worksheet worksheet = wb.Worksheets[0];
                    string[] days = new string[5] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница" };
                    worksheet.Cells.Merge(1, 1, 1, 4);
                    worksheet.Cells.Merge(1, 5, 1, 4);
                    worksheet.Cells[1, 1].PutValue("1 смена");
                    worksheet.Cells[1, 5].PutValue("2 смена");
                    Style style = wb.CreateStyle();
                    style.HorizontalAlignment = TextAlignmentType.Center;
                    style.Font.IsBold = true;
                    Style daysStyle = wb.CreateStyle();
                    daysStyle.VerticalAlignment = TextAlignmentType.Center;
                    daysStyle.Font.IsBold = true;
                    Style scheduleStyle = wb.CreateStyle();
                    scheduleStyle.VerticalAlignment = TextAlignmentType.Center;
                    scheduleStyle.HorizontalAlignment = TextAlignmentType.Center;
                    scheduleStyle.IsTextWrapped = true;
                    worksheet.Cells[1, 1].SetStyle(style);
                    worksheet.Cells[1, 5].SetStyle(style);
                    worksheet.Cells.SetColumnWidth(0, 13.0);
                    for (int i = 1; i < 5; i++)
                    {
                        worksheet.Cells[2, i].PutValue(i);
                        worksheet.Cells[2, i].SetStyle(style);
                        worksheet.Cells[2, i + 4].PutValue(i);
                        worksheet.Cells[2, i + 4].SetStyle(style);
                    }
                    for(int i = 0; i < 10; i+=2)
                    {
                        worksheet.Cells.InsertRow(i + 4);
                        worksheet.Cells.Merge(i + 3, 0, 2, 1);
                        worksheet.Cells[i + 3, 0].PutValue(days[i / 2]);
                        worksheet.Cells.SetRowHeight(i + 3, 35.0);
                        worksheet.Cells.SetRowHeight(i + 4, 35.0);
                        worksheet.Cells[i + 3, 0].SetStyle(daysStyle);
                    }
                    for (int i = 1; i < 11; i += 2)
                    {
                        int lessonNumber = 1;
                        var teachersSchedule = await db.TeachersSchedules
                            .Include(t => t.ScheduleDenominator)
                            .Include(t => t.ScheduleDenominator.SubjectDenominator)
                            .Include(t => t.ScheduleDenominator.SubjectNumerator)
                            .Include(t => t.ScheduleDenominator.Group)
                            .Include(t => t.ScheduleNumerator.SubjectDenominator)
                            .Include(t => t.ScheduleNumerator.SubjectNumerator)
                            .Include(t => t.ScheduleNumerator.Group)
                            .Where(t => t.TeacherId == teacher.Id && (t.DayOfTheWeek == ((i + 1) / 2)) && t.Shift == 1)
                            .OrderBy(t => t.DayOfTheWeek)
                            .OrderBy(t => t.SubjectNumber)
                            .ToListAsync();
                        foreach (var item in teachersSchedule)
                        {
                            Console.WriteLine(item.TeacherId);
                            worksheet.Cells.SetColumnWidth(lessonNumber, 30.0);
                            worksheet.Cells[i + 2, lessonNumber].SetStyle(scheduleStyle);
                            worksheet.Cells[i + 3, lessonNumber].SetStyle(scheduleStyle);
                            if (item.ScheduleNumeratorId != null)
                            {
                                worksheet.Cells[i + 2, lessonNumber].PutValue(item.ScheduleDenominator?.SubjectDenominator?.SubjectName + "\n" + item.ScheduleDenominator?.Group?.GroupName + "\n" + item.ScheduleDenominator?.RoomDenominator);
                                worksheet.Cells[i + 3, lessonNumber].PutValue(item.ScheduleNumerator?.SubjectNumerator?.SubjectName + "\n" + item.ScheduleNumerator?.Group?.GroupName + "\n" + item.ScheduleNumerator?.RoomNumerator);
                            }
                            else
                            {
                                worksheet.Cells.Merge(i + 2, lessonNumber, 2, 1);
                                worksheet.Cells[i + 2, lessonNumber].PutValue(item.ScheduleDenominator?.SubjectDenominator?.SubjectName + "\n" + item.ScheduleDenominator?.Group?.GroupName + "\n" + item.ScheduleDenominator?.RoomDenominator);
                            }
                            lessonNumber++;
                        }
                    }

                    for (int i = 1; i < 11; i += 2)
                    {
                        int lessonNumber = 5;
                        var teachersSchedule = await db.TeachersSchedules
                            .Include(t => t.ScheduleDenominator)
                            .Include(t => t.ScheduleDenominator.SubjectDenominator)
                            .Include(t => t.ScheduleDenominator.SubjectNumerator)
                            .Include(t => t.ScheduleDenominator.Group)
                            .Include(t => t.ScheduleNumerator.SubjectDenominator)
                            .Include(t => t.ScheduleNumerator.SubjectNumerator)
                            .Include(t => t.ScheduleNumerator.Group)
                            .Where(t => t.TeacherId == teacher.Id && (t.DayOfTheWeek == ((i + 1) / 2)) && t.Shift == 2)
                            .OrderBy(t => t.DayOfTheWeek)
                            .OrderBy(t => t.SubjectNumber)
                            .ToListAsync();
                        foreach (var item in teachersSchedule)
                        {
                            Console.WriteLine(item.TeacherId);
                            worksheet.Cells.SetColumnWidth(lessonNumber, 30.0);
                            worksheet.Cells[i + 2, lessonNumber].SetStyle(scheduleStyle);
                            worksheet.Cells[i + 3, lessonNumber].SetStyle(scheduleStyle);
                            if (item.ScheduleNumeratorId != null)
                            {
                                worksheet.Cells[i + 2, lessonNumber].PutValue(item.ScheduleDenominator?.SubjectDenominator?.SubjectName + "\n" + item.ScheduleDenominator?.Group?.GroupName + "\n" + item.ScheduleDenominator?.RoomDenominator);
                                worksheet.Cells[i + 3, lessonNumber].PutValue(item.ScheduleNumerator?.SubjectNumerator?.SubjectName + "\n" + item.ScheduleNumerator?.Group?.GroupName + "\n" + item.ScheduleNumerator?.RoomNumerator);
                            }
                            else
                            {
                                worksheet.Cells.Merge(i + 2, lessonNumber, 2, 1);
                                worksheet.Cells[i + 2, lessonNumber].PutValue(item.ScheduleDenominator?.SubjectDenominator?.SubjectName + "\n" + item.ScheduleDenominator?.Group?.GroupName + "\n" + item.ScheduleDenominator?.RoomDenominator);
                            }
                            lessonNumber++;
                        }
                    }
                    string path = app.WebRootPath + "/ExcelFiles/" + teacher.teacherFullName + ".xlsx";
                    wb.Save(path, SaveFormat.Xlsx);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, "application/x-msdownload", teacher.teacherFullName + ".xlsx"); 
                } else
                {
                    return Json("unsuccessfull");
                }
            } else
            {
                return Json("unsuccessfull");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
