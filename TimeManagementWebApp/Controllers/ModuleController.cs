using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagementClassLibrary;
using TimeManagementWebApp.Data;
using TimeManagementWebApp.Models.ViewModels;

namespace TimeManagementWebApp.Controllers
{
    public class ModuleController : Controller
    {
        private readonly TimeManagementDbContext _db;
        //Selected module
        public static Semester? CurrentSemester;

        public ModuleController(TimeManagementDbContext context)
        {
            _db = context;
        }

        // GET: Module
        public async Task<IActionResult> Index()
        {
            if (CurrentSemester == null) return View(await _db.Modules.ToListAsync());
            //Clear current studyhours data
            StudyHoursController.CurrentModule = null;
            ViewBag.SemesterName = CurrentSemester.SemesterName;
            //Get modules async
            var query = from m in _db.Modules
                where m.Semester.SemesterId == CurrentSemester.SemesterId
                select m;
            var modules = await query.ToListAsync();
            return View(modules);
        }

        //Select module and redirect to study hours
        public IActionResult Select(int id)
        {
            StudyHoursController.CurrentModule = _db.Modules.Find(id);
            return RedirectToAction("Index", "StudyHours");
        }

        // GET: Module/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Module/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,NumCredits,ClassHoursPerWeek,WeekDays")] ModuleViewModel moduleModel)
        {
            if (!ModelState.IsValid || CurrentSemester == null) return View(moduleModel);
            try
            {
                //Get semester async
                var query = from s in _db.Semesters
                    where s.SemesterId == CurrentSemester.SemesterId
                    select s;
                var semester = await query.FirstOrDefaultAsync();
                //Create moduleModel
                if (semester != null)
                {
                    Module m = new()
                    {
                        Code = moduleModel.Code,
                        Name = moduleModel.Name,
                        NumCredits = moduleModel.NumCredits,
                        ClassHoursPerWeek = moduleModel.ClassHoursPerWeek,
                        SelfStudyHoursPerWeek = ((moduleModel.NumCredits * 10) / semester.NumWeeks) - moduleModel.ClassHoursPerWeek,
                        Semester = semester
                    };
                    foreach (var day in moduleModel.WeekDays)
                    {
                        m.DaysOfWeek += day;
                        //Don't add comma separator to end
                        if (moduleModel.WeekDays.IndexOf(day) != moduleModel.WeekDays.Count - 1)
                        {
                            m.DaysOfWeek += ",";
                        }
                    }
                    _db.Add(m);
                    //Generate StudyHours for moduleModel and insert them
                    for (int i = 0; i < semester.NumWeeks; i++)
                    {
                        TimeSpan ts = new(i * 7, 0, 0, 0);
                        StudyHours st = new()
                        {
                            Week = i + 1,
                            RemainingStudyHours = m.SelfStudyHoursPerWeek,
                            Date = semester.StartDate + ts,
                            Module = m
                        };
                        _db.Add(st);
                    }
                }

                //Save changes
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(moduleModel);
            }
        }

        // GET: Module/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _db.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            ModuleViewModel moduleModel = new() { Code = @module.Code, Name = @module.Name, ClassHoursPerWeek = @module.ClassHoursPerWeek, NumCredits = @module.NumCredits};
            string[] days = @module.DaysOfWeek.Split(",");
            foreach (var day in days)
            {
                moduleModel.WeekDays.Add(day);
            }
            return View(moduleModel);
        }

        // POST: Module/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Code,Name,NumCredits,ClassHoursPerWeek,WeekDays")] ModuleViewModel moduleModel)
        {

            if (!ModelState.IsValid || CurrentSemester == null) return View(moduleModel);
            bool hoursChanged = false;
            try
            {
                //Get module async
                var query = from m in _db.Modules
                    where m.ModuleId == id
                    select m;
                var module = await query.FirstOrDefaultAsync();
                    
                //update module and _db
                if (module != null)
                {
                    //If the credits or class hours were updated, set to regenerate study hours
                    if (module.NumCredits != moduleModel.NumCredits || module.ClassHoursPerWeek != moduleModel.ClassHoursPerWeek)
                    {
                        hoursChanged = true;
                    }

                    module.Code = moduleModel.Code;
                    module.Name = moduleModel.Name;
                    module.NumCredits = moduleModel.NumCredits;
                    module.ClassHoursPerWeek = moduleModel.ClassHoursPerWeek;
                    module.SelfStudyHoursPerWeek = ((moduleModel.NumCredits * 10) / CurrentSemester.NumWeeks) - moduleModel.ClassHoursPerWeek;
                    module.DaysOfWeek = "";
                    foreach (var day in moduleModel.WeekDays)
                    {
                        module.DaysOfWeek += day;
                        //Don't add comma separator to end
                        if (moduleModel.WeekDays.IndexOf(day) != moduleModel.WeekDays.Count - 1)
                        {
                            module.DaysOfWeek += ",";
                        }
                    }
                    _db.Update(module);
                        
                    if (hoursChanged)
                    {
                        //Delete StudyHours for module
                        var querySt = from st in _db.StudyHours
                            where st.Module == module
                            select st;
                        var studyHours = await querySt.ToListAsync();
                        _db.StudyHours.RemoveRange(studyHours);
                        //Generate StudyHours for edited module and insert them
                        for (int i = 0; i < CurrentSemester.NumWeeks; i++)
                        {
                            TimeSpan ts = new(i * 7, 0, 0, 0);
                            StudyHours st = new()
                            {
                                Week = i + 1,
                                RemainingStudyHours = module.SelfStudyHoursPerWeek,
                                Date = CurrentSemester.StartDate + ts,
                                Module = module
                            };
                            _db.Add(st);
                        }
                    }
                    //Save changes
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                if (!ModuleExists(id))
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

        // GET: Module/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _db.Modules
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Module/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @module = await _db.Modules.FindAsync(id);
            if (@module != null)
            {
                _db.Modules.Remove(@module);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return _db.Modules.Any(e => e.ModuleId == id);
        }
    }
}
