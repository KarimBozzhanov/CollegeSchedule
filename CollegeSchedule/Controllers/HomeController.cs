using CollegeSchedule.Models;
using CollegeSchedule.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using XSystem.Security.Cryptography;

namespace CollegeSchedule.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db;
 
        readonly IConfigurationRoot configuration;
        public HomeController(ApplicationDbContext context, IHostingEnvironment env)
        {
            db = context;
           
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GroupList(string searchText, int course)
        {
            ViewData["course"] = course;
            if (!String.IsNullOrEmpty(searchText))
            {
                Console.WriteLine(searchText);
                return View(await db.Groups.Where(g => g.GroupName.Contains(searchText) && g.GroupCourse == course).OrderBy(g => g.GroupName).ToListAsync());
            } else
            {
                return View(await db.Groups.Where(g => g.GroupCourse == course).OrderBy(g => g.GroupName).ToListAsync());
            }
        }


        [HttpGet]
        public async Task<IActionResult> ScheduleTable(int groupId)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            ViewData["groupId"] = group.Id;
            ViewData["group"] = group.GroupName;
            ViewData["course"] = group.GroupCourse;
            return View(await db.Schedules.Include(s => s.TeacherDenominator).Include(s => s.TeacherNumerator).Include(s => s.Group).Where(s => s.Group.Id == groupId).OrderBy(s => s.SubjectNumber).ToListAsync());
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
        public async Task<JsonResult> CreateGroup(string groupName, int groupCourse, int groupShift)
        {
            Console.WriteLine("GroupName - " + groupName + "\nCourse - " + groupCourse + "\nShift - " + groupShift);
            Group collegeGroup = await db.Groups.FirstOrDefaultAsync(g => g.GroupName.ToLower() == groupName.ToLower());
            if(collegeGroup == null)
            {
                await db.Groups.AddAsync(new Group() { GroupName = groupName, GroupCourse = groupCourse, GroupShift = groupShift });
                await db.SaveChangesAsync();
                Group group = await db.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
                if(group != null)
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
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "08:00", EndOfLesson = "09:20" });
                                        break;
                                    case 2:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "09:30", EndOfLesson = "10:50" });
                                        break;
                                    case 3:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "11:00", EndOfLesson = "12:20" });
                                        break;
                                    case 4:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "12:30", EndOfLesson = "13:50" });
                                        break;
                                }
                            }
                            else if (groupShift == 2)
                            {
                                switch (j)
                                {
                                    case 1:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "13:00", EndOfLesson = "14:20" });
                                        break;
                                    case 2:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "14:30", EndOfLesson = "15:50" });
                                        break;
                                    case 3:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "16:00", EndOfLesson = "17:20" });
                                        break;
                                    case 4:
                                        await db.Schedules.AddAsync(new Schedule { DayOfTheWeek = i, SubjectNumber = j, GroupId = group.Id, StartOfLesson = "17:30", EndOfLesson = "18:50" });
                                        break;
                                }
                            }
                        }
                    }

                    await db.SaveChangesAsync();
                    return Json("Группа сохранена");
                } else
                {
                    return Json("Не удалось сохранить группу");
                }
            } else
            {
                return Json("Группа с таким именем уже создана");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int? groupId, int groupCourse)
        {
            if (groupId != null)
            {
                foreach (var item in db.Schedules.Where(s => s.Group.Id == groupId))
                {
                    Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == item.Id);
                    db.Entry(schedule).State = EntityState.Deleted;
                }
                Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                db.Entry(group).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("GroupList", new { course = groupCourse });
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
        public async Task<IActionResult> EditGroup(int groupId, string groupName, int groupCourse, int currentCourse)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group != null)
            {
                var groupUpdate = db.Groups.Where(g => g.Id == groupId);
                groupUpdate.FirstOrDefault().GroupName = groupName;
                groupUpdate.FirstOrDefault().GroupCourse = groupCourse;
                await db.SaveChangesAsync();
                return RedirectToAction("GroupList", new {course = currentCourse });
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> EditSchedule(int groupId)
        {
            Group group = await db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            var allModels = new AllModels();
            allModels.Schedules = await db.Schedules.Where(g => g.GroupId == groupId).OrderBy(s => s.SubjectNumber).ToListAsync();
            allModels.Teachers = await db.Teachers.OrderBy(t => t.teacherFullName).ToListAsync();
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
        public async Task<JsonResult> EditTeacherDenominator(int? id, string teacherName)
        {
            if (id != null)
            {
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
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
                Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Id == id);
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
            Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == id);
            if (teacher != null)
            {
                var teacherJson = JsonConvert.SerializeObject(teacher);
                return Json(teacherJson);
            }
            return Json(NotFound());
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
                        TempData["teacherId"] = teacherId;
                        TempData["teacherPassword"] = teacherPass;
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
            var allModels = new AllModels();
            allModels.ExamsSchedules = await db.ExamsSchedules.Include(e => e.Teacher).Where(e => e.GroupId == groupId).ToListAsync();
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
            allModels.ExamsSchedules = await db.ExamsSchedules.Where(e => e.GroupId == groupId).ToListAsync();
            allModels.ConsultationSchedules = await db.ConsultationsSchedules.Where(c => c.GroupId == groupId).ToListAsync();
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
                } else
                {
                    return View(teacherName);
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> TeachersSchedule()
        {
            int? teacherId = (int)TempData["teacherId"];
            string teacherPass = TempData["teacherPassword"] as string;
            if (teacherId != null)
            {
                Console.WriteLine(teacherId + " " + teacherPass);
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
                if (teacher != null && teacher.TeacherPassword.Equals(teacherPass))
                {
                    ViewData["teacherName"] = teacher.teacherFullName;
                    ViewBag.teacherId = teacher.Id;
                    ViewBag.MondayLessons = db.Schedules.Count(s => s.DayOfTheWeek == 1 && (s.TeacherDenominator.Id == teacherId || s.TeacherNumeratorId == teacherId)) + 1;
                    ViewBag.TuesdayLessons = db.Schedules.Count(s => s.DayOfTheWeek == 2 && (s.TeacherDenominator.Id == teacherId || s.TeacherNumeratorId == teacherId)) + 1;
                    ViewBag.WednesdayLessons = db.Schedules.Count(s => s.DayOfTheWeek == 3 && (s.TeacherDenominator.Id == teacherId || s.TeacherNumeratorId == teacherId)) + 1;
                    ViewBag.ThursdayLessons = db.Schedules.Count(s => s.DayOfTheWeek == 4 && (s.TeacherDenominator.Id == teacherId || s.TeacherNumeratorId == teacherId)) + 1;
                    ViewBag.FridayLessons = db.Schedules.Count(s => s.DayOfTheWeek == 5 && (s.TeacherDenominator.Id == teacherId || s.TeacherNumeratorId == teacherId)) + 1;
                    Console.WriteLine(db.Schedules.Count(s => s.DayOfTheWeek == 1 && s.TeacherDenominator.Id == teacherId));
                    return View(await db.Schedules.Include(s => s.TeacherDenominator).Include(s => s.TeacherNumerator).Include(s => s.Group).Where(s => s.TeacherDenominatorId == teacherId || s.TeacherNumeratorId == teacherId).ToListAsync());
                } else
                {
                    return RedirectToAction("TeachersList", "Home");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> DeleteTeacher(int? teacherId)
        {
            if (teacherId != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
                foreach(var item in db.Schedules.Where(s => s.TeacherDenominatorId == teacher.Id || s.TeacherNumeratorId == teacher.Id))
                {
                    var updateSchedule = await db.Schedules.Where(s => s.Id == item.Id).AsQueryable().FirstOrDefaultAsync();
                    if(item.TeacherNumeratorId == teacher.Id)
                    {
                        updateSchedule.TeacherNumeratorId = null;
                    } else if(item.TeacherDenominatorId == teacher.Id)
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
                db.Entry(teacher).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("TeachersList");
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
        public async Task<JsonResult> EditExamName(int? id, string name)
        {
            if (id != null)
            {
                ExamsSchedule exam = await db.ExamsSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updateExam = await db.ExamsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateExam.ExamName = name;
                db.ExamsSchedules.UpdateRange(updateExam);
                await db.SaveChangesAsync();
                return Json("Предмет экзамена изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditExamTeacher(int? id, int? teacherId)
        {
            if (id != null)
            {
                ExamsSchedule exam = await db.ExamsSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updateExam = await db.ExamsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateExam.TeacherId = teacherId;
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
        public async Task<JsonResult> EditPracticeTeacher(int? id, int? teacherId)
        {
            if (id != null)
            {
                PracticeSchedule practice = await db.PracticeSchedules.FirstOrDefaultAsync(e => e.Id == id);
                var updatePractice = await db.PracticeSchedules.Where(p => p.Id == id).AsQueryable().FirstOrDefaultAsync();
                updatePractice.TeacherId = teacherId;
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
        public async Task<JsonResult> EditConsultationSubject(int? id, string subject)
        {
            if (id != null)
            {
                var updateConsultation = await db.ConsultationsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateConsultation.Subject = subject;
                db.ConsultationsSchedules.UpdateRange(updateConsultation);
                await db.SaveChangesAsync();
                return Json("Предмет консультации изменен");
            }
            return Json(NotFound());
        }

        [HttpPost]
        public async Task<JsonResult> EditConsultationTeacher(int? id, int? teacherId)
        {
            if (id != null)
            {
                var updateConsultation = await db.ConsultationsSchedules.Where(s => s.Id == id).AsQueryable().FirstOrDefaultAsync();
                updateConsultation.TeacherId = teacherId;
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





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
