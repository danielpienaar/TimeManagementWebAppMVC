using System.ComponentModel.DataAnnotations;

namespace TimeManagementWebApp.Models.ViewModels
{
    public class SemesterViewModel
    {
        [Required(ErrorMessage = "Please enter the semester name.")]
        public string SemesterName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the number of weeks in the semester.")]
        public int NumWeeks { get; set; }
        [Required(ErrorMessage = "Please enter the semester start date.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
    }
}
