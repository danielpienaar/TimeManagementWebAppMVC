using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace TimeManagementClassLibrary
{
    public class Student
    {
        //Annotations
        //https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations
        //Accessed 15 December 2022
        [Key]
        [MaxLength(10)]
        public string StudentId { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required] public string Password { get; set; } = null!;
        public ICollection<Semester>? Semesters { get; set; }

        public static string Hash(string value)
        {
            //Hash value obtained from
            //https://stackoverflow.com/questions/16999361/obtain-sha-256-string-of-a-string/17001289#17001289
            //User answered:
            //https://stackoverflow.com/users/14608904/samuel-johnson
            //Accessed 12 November 2022
            using var hash = SHA256.Create();
            var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(byteArray);
        }
    }
}
