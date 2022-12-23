using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeManagementClassLibrary
{
    public class Module
    {
        //Primary key = identity field
        //https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations
        //Accessed 15 December 2022
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleId { get; set; }
        [Required(ErrorMessage = "Please enter the module code.")]
        [MaxLength(10)]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the module name.")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the number of credits.")]
        public int NumCredits { get; set; }
        [Required(ErrorMessage = "Please enter the class hours per week.")]
        public int ClassHoursPerWeek { get; set; }
        [Required]
        public double SelfStudyHoursPerWeek { get; set; }
        [Required(ErrorMessage = "Please enter the week days on which you plan to study this module.")]
        public string DaysOfWeek { get; set; } = null!;
        [Required]
        public Semester Semester { get; set; } = null!;

        //StudyHours List for Weeks with remaining study hours
        public ICollection<StudyHours>? StudyHours { get; set; }
    }
}
