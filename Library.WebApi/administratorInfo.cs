using System.ComponentModel.DataAnnotations;

namespace WebApplication
{
    public class AdminLogIn
    {
        [Required(ErrorMessage = "Email can not be empty")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password can not be empty")]
        [DataType(DataType.Password)]
        public string Pwd { get; set; }
        public string Token { get; set; }
    }

    public class AdminSignUp
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
    }
    
    //get admin info 
    public class AdminInfo
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminGender { get; set; }
        public string AdminToken { get; set; }
        public string AdminRemark { get; set; }
        public string AdminPassword { get; set; }
    }
}