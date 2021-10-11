using System;
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
    public class ManageReader
    {
        public int ReaderId { get; set; }
        public string ReaderName { get; set; }
        public string ReaderEmail { get; set; }
        public decimal ReaderUnpaid { get; set; }
        public int ReaderOnhold { get; set; }
        public string ReaderRemark { get; set; }
        public string Token { get; set; }
        public string Pwd { get; set; }
    }

    public class UserMessage
    {
        public int MessageId { get; set; }
        public string ReaderName { get; set; }
        public string ReaderEmail { get; set; }
        public string ReaderMessage { get; set; }
        public string  MessageStatus { get; set; }
    }
}