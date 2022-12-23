using System.ComponentModel.DataAnnotations;

namespace TimeManagementWebApp.Models.ViewModels
{
    /// <summary>
    /// Used for getting the login data from the Index View
    /// </summary>
    public class SigninViewModel
    {
        //View models
        //https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-3
        //Accessed 15 December 2022
        [Key]
        public string StudentId { get; set; } = null!;
        [Required] 
        public string Password { get; set; } = null!;
    }
}
