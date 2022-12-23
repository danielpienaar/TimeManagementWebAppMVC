using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TimeManagementClassLibrary;
using TimeManagementWebApp.Data;
using TimeManagementWebApp.Models.ViewModels;

namespace TimeManagementWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly TimeManagementDbContext _db;

        public HomeController(TimeManagementDbContext timeManagementDbContext)
        {
            this._db = timeManagementDbContext;
        }

        //Login view
        public IActionResult Index()
        {
            return View();
        }

        //Register view
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login(SigninViewModel signin)
        {
            //Check data
            var students = _db.Students.ToList();
            foreach (var student in students)
            {
                if (student.StudentId.Equals(signin.StudentId) && student.Password.Equals(Student.Hash(signin.Password)))
                {
                    //Login and set logged in student
                    SemesterController.LoggedInStudent = student;
                    ViewBag.StudentName = student.Name;
                    //Redirect if no error
                    return RedirectToAction("Index", "Semester");
                }
            }
            //Login failed, show error
            ViewBag.StudentError = "Incorrect username or password.";
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertStudent(Student student)
        {
            //Hash Password
            student.Password = Student.Hash(student.Password);
            //Insert a student into _db
            try
            {
                _db.Students.Add(student);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                string exception = "" + e.InnerException;
                if (exception.Contains("duplicate"))
                {
                    ViewBag.StudentError = "Student already exists.";
                }
                else
                {
                    ViewBag.StudentError = exception;
                }
                return View("~/Views/Home/Register.cshtml");
            }
            //Return to login page
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}