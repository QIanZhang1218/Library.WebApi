using System;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Http;
using WebApplication;

namespace Library.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SignInController : ControllerBase
    {
        [HttpPost]
        public Object VerifySignIn([FromBody] SignIn para)
        {
            Mysql database = new Mysql();
            var res = database.VerifyReader($"SELECT * FROM library_schema.reader WHERE reader_email={para.Email} AND reader_pwd={para.Password} ;");
            if (res == null)
            {
                return new Response { Status = "Invalid", Message = "Invalid User." };
            }
            else
                return new Response { Status = "Success", Message = "Login Successfully" };
        }
    }
}