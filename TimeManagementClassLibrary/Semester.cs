using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeManagementClassLibrary
{
    public class Semester
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SemesterId { get; set; }
        [Required(ErrorMessage = "Please enter the semester name.")]
        [MaxLength(50)]
        public string SemesterName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the number of weeks in the semester.")]
        public int NumWeeks { get; set; }
        [Required(ErrorMessage = "Please enter the semester start date.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required]
        public Student Student { get; set; } = null!;

        public ICollection<Module>? Modules { get; set; }
    }
}
