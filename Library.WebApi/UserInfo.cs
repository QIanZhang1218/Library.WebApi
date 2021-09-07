using System.ComponentModel.DataAnnotations;


namespace Library.WebApi
{
    public class UserInfo
    {
        [Required(ErrorMessage = "Email can not be empty")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password can not be empty")]
        [DataType(DataType.Password)]
        public string Password { get; set;  }

        // public int UserId { get; set; }
        public string Token { get; set; }


    }

    public class LoginStatus
    {
        public bool SignIn { get; set; }
    }
}