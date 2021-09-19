using System;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Org.BouncyCastle.Crypto.Tls;
using JWT.MvcDemo.Models;

namespace Library.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SignUpController : ControllerBase
    {
        [HttpPost]
        public Object PushSignUps([FromBody] SignUpPara para)
        {
            Mysql database = new Mysql();
            try
            {
                var res = database.ExecuteNonQueryReturn(
                    $"INSERT INTO `library_schema`.`reader` (`reader_name`, `reader_pwd`, `reader_email`) VALUES ('{para.Name}', '{para.Password}', '{para.Email}')");
                Console.WriteLine(res);
                if (res == "Success")
                {
                    return new AdminStatusResponse
                        { Success = true, Message = "Record SuccessFully Saved."};
                }
                else if (res == $"Duplicate entry '{para.Email}' for key 'reader.reader_email_UNIQUE'")
                {
                    return new AdminStatusResponse
                    {
                        Success = false, Message = "Email already exist"
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new AdminStatusResponse
                    { Success = false, Message = "Invalid Data." };
            }

            return new AdminStatusResponse
            {
                Success = false,Message = "Failed"
            };

        }
    }
}
    