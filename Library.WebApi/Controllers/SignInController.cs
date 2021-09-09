using System;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Http;
using JWT.MvcDemo.Models;

namespace Library.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SignInController : ControllerBase
    {
        [HttpPost]
        public Object VerifySignIn([FromBody] UserInfo para)
        {
            // string token = Request.Cookies["token"];
            // Console.WriteLine("@"+token);
            Mysql database = new Mysql();
            //update overdue status in borrow_list 
            database.ExecuteNonQuery(
                $" Update library_schema.borrow_list SET isOverdue = true WHERE (return_date < Date(now()) AND isReturn = false And isPickup = 1);");
            
            var res = database.VerifyReader($"SELECT * FROM library_schema.reader WHERE reader_email='{para.Email}' AND reader_pwd='{para.Password}' ;");
            if (res == null)
            {
                return new StatusResponse { Success = false, Message = "Invalid User."};
            }
            else
            {
                var token = res[0].Token;
                database.ExecuteNonQuery(
                    $" UPDATE `library_schema`.`reader` SET `token` = '{token}' WHERE reader_email='{para.Email}';");
                return new StatusResponse { Success = true, Message = "Login Successfully",Token = res[0].Token };
            }
        }
    }
}