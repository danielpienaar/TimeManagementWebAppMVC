using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagementClassLibrary;
using TimeManagementWebApp.Data;
using TimeManagementWebApp.Models.ViewModels;

namespace TimeManagementWebApp.Controllers
{
    public class SemesterController : Controller
    {
        private readonly TimeManagementDbContext _db;
        //Currently logged in student
        public static Student? LoggedInStudent;

        public SemesterController(TimeManagementDbContext context)
        {
            _db = context;
        }

        //Log out and return to sign in page
        public IActionResult LogOut()
        {
            LoggedInStudent = null;
            return RedirectToAction("Index", "Home");
        }

        //Select a semester and view the module list for the semester id
        public IActionResult Select(int id)
        {
            ModuleController.CurrentSemester = _db.Semesters.Find(id);
            return RedirectToAction("Index", "Module");
        }

        // GET: Semester
        public async Task<IActionResult> Index()
        {
            //Only show page if logged in
            if (LoggedInStudent == null) return RedirectToAction("Index", "Home");
            //Clear all current module and studyhours data
            ModuleController.CurrentSemester = null;
            StudyHoursController.CurrentModule = null;
            //Set student name
            ViewBag.StudentName = LoggedInStudent.Name;
            //Get semesters async
            //web app Linq queries adapted from
            //https://www.tutorialsteacher.com/linq/what-is-linq
            //accessed 15 December 2022
            var query = from s in _db.Semesters
                        where s.Student.StudentId == LoggedInStudent.StudentId
                        select s;
            var semesters = await query.ToListAsync();
            return View(semesters);
        }

        // GET: Semester/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Semester/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SemesterName,NumWeeks,StartDate")] SemesterViewModel semester)
        {
            if (!ModelState.IsValid || LoggedInStudent == null) return View(semester);
            try
            {
                //Get student async
                var query = from st in _db.Students
                            where st.StudentId == LoggedInStudent.StudentId
                            select st;
                var student = await query.FirstOrDefaultAsync();
                if (student != null)
                {
                    Semester s = new()
                    {
                        SemesterName = semester.SemesterName,
                        NumWeeks = semester.NumWeeks,
                        StartDate = semester.StartDate,
                        EndDate = semester.StartDate + new TimeSpan(semester.NumWeeks * 7, 0, 0, 0),
                        Student = student
                    };
                    //Add student to _db async
                    _db.Add(s);
                }

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(semester);
            }
        }

        // GET: Semester/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var semester = await _db.Semesters.FindAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            SemesterViewModel semesterModel = new() { SemesterName = semester.SemesterName, NumWeeks = semester.NumWeeks, StartDate = semester.StartDate };
            return View(semesterModel);
        }

        // POST: Semester/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SemesterName,NumWeeks,StartDate")] SemesterViewModel semesterModel)
        {
            if (!ModelState.IsValid || LoggedInStudent == null) return View(semesterModel);
            try
            {
                //Get semester async
                var query = from s in _db.Semesters
                            where s.SemesterId == id
                            select s;
                var semester = await query.FirstOrDefaultAsync();

                //Update semester and _db
                if (semester != null)
                {
                    semester.SemesterName = semesterModel.SemesterName;
                    semester.NumWeeks = semesterModel.NumWeeks;
                    semester.StartDate = semesterModel.StartDate;
                    semester.EndDate = semesterModel.StartDate + new TimeSpan(semesterModel.NumWeeks * 7, 0, 0, 0);

                    //Edit study hours
                    var queryModules = from m in _db.Modules
                                       where m.Semester == semester
                                       select m;
                    var moduleList = await queryModules.ToListAsync();

                    foreach (var module in moduleList)
                    {
                        //Old max study hours for use in calculation
                        double selfStudy = module.SelfStudyHoursPerWeek;

                        //Recalculate remaining study hours and update module
                        module.SelfStudyHoursPerWeek = ((module.NumCredits * 10) / semester.NumWeeks) - module.ClassHoursPerWeek;
                        //Self study hours cannot be less than 0
                        if (module.SelfStudyHoursPerWeek < 0)
                        {
                            module.SelfStudyHoursPerWeek = 0;
                        }
                        _db.Update(module);
                        
                        //Get old study hours
                        var queryStudyHours = from st in _db.StudyHours
                                              where st.Module == module
                                              select st;
                        var studyHoursList = await queryStudyHours.ToListAsync();
                        //Sort study hours by week, adapted from
                        //https://www.techiedelight.com/sort-list-of-objects-csharp/
                        //accessed 13 December 2022
                        studyHoursList.Sort((x, y) => x.Week.CompareTo(y.Week));

                        //Generate StudyHours for modules for the edited semester and insert them
                        for (int i = 0; i < semester.NumWeeks; i++)
                        {
                            TimeSpan ts = new(i * 7, 0, 0, 0);
                            
                            //If there is an existing studyHours for this week, get the hours already studied and subtract them from the new value
                            double study = i < studyHoursList.Count ? (module.SelfStudyHoursPerWeek - (selfStudy - studyHoursList[i].RemainingStudyHours)) : module.SelfStudyHoursPerWeek;
                            //Study hours cannot be less than 0
                            if (study < 0)
                            {
                                study = 0;
                            }
                            StudyHours st = new()
                            {
                                Week = i + 1,
                                RemainingStudyHours = study,
                                Date = semester.StartDate + ts,
                                Module = module
                            };
                            _db.Add(st);
                        }

                        //Remove old study hours
                        foreach (var stu in studyHoursList)
                        {
                            _db.Remove(stu);
                        }
                    }

                    _db.Update(semester);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                if (!SemesterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Semester/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var semester = await _db.Semesters
                .FirstOrDefaultAsync(m => m.SemesterId == id);
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // POST: Semester/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var semester = await _db.Semesters.FindAsync(id);
            if (semester != null)
            {
                _db.Semesters.Remove(semester);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SemesterExists(int id)
        {
            return _db.Semesters.Any(e => e.SemesterId == id);
        }
    }
}
