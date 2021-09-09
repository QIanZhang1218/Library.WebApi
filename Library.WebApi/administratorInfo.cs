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
        public string Pwd { get; set; }
        public string Gender { get; set; }
    }
}