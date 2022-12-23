using System.ComponentModel.DataAnnotations;

namespace TimeManagementWebApp.Models.ViewModels
{
    public class ModuleViewModel
    {
        [Required(ErrorMessage = "Please enter the module code.")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the module name.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the number of credits.")]
        public int NumCredits { get; set; }
        [Required(ErrorMessage = "Please enter the class hours per week.")]
        public int ClassHoursPerWeek { get; set; }
        //[Required(ErrorMessage = "Please enter the week days on which you plan to study this module.")]
        //public string DaysOfWeek { get; set; } = null!;
        [Required(ErrorMessage = "Please select the week days on which you plan to study this module.")]
        public List<string> WeekDays { get; set; } = new();
    }
}
