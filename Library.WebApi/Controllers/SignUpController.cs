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
                database.ExecuteNonQuery(
                    $"INSERT INTO `library_schema`.`reader` (`reader_name`, `reader_pwd`, `reader_email`) VALUES ('{para.Name}', '{para.Password}', '{para.Email}')");
                return new StatusResponse
                    { Success = true, Message = "Record SuccessFully Saved.",Token="fake-jwt-token" };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new StatusResponse
                    { Success = false, Message = "Invalid Data." };
            }
            
        }
    }
}
    