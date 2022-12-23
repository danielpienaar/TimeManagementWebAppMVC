using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeManagementClassLibrary
{
    public class StudyHours
    {
        //Primary key = identity field
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudyHoursId { get; set; }
        [Required]
        public int Week { get; set; }
        [Required]
        public double RemainingStudyHours { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        public Module Module { get; set; } = null!;
    }
}
