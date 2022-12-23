using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagementClassLibrary;
using TimeManagementWebApp.Data;

namespace TimeManagementWebApp.Controllers
{
    public class StudyHoursController : Controller
    {
        private readonly TimeManagementDbContext _db;
        public static Module? CurrentModule;

        public StudyHoursController(TimeManagementDbContext context)
        {
            _db = context;
        }

        // GET: StudyHours
        public async Task<IActionResult> Index()
        {
            if (CurrentModule != null)
            {
                ViewBag.ModuleName = CurrentModule.Name;
                //Get studyhours async
                var query = from s in _db.StudyHours
                            where s.Module.ModuleId == CurrentModule.ModuleId
                            select s;
                var studyHours = await query.ToListAsync();
                return View(studyHours);
            }
            return View(await _db.StudyHours.ToListAsync());
        }

        //Update remaining study hours
        public async Task<IActionResult> Update(int hoursStudied, int id)
        {
            try
            {
                //Get studyhours async
                var query = from s in _db.StudyHours
                            where s.StudyHoursId == id
                            select s;
                var studyHours = await query.FirstOrDefaultAsync();
                if (studyHours != null)
                {
                    //Subtract hours studied
                    studyHours.RemainingStudyHours -= hoursStudied;
                    _db.Update(studyHours);
                    //Save changes
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                return View("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
