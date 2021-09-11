using System;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Http;
using JWT.MvcDemo.Models;
using WebApplication;

namespace Library.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminManagementController : ControllerBase
    {
        //get reader list table
        [HttpGet]
        public AdminStatusResponse GetReaderInfo()
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            Console.WriteLine(admin);
            var res = database.GetReaderList($"SELECT * FROM library_schema.reader");
            // var isSignin = database.ExecuteGetBooksList($"SELECT * FROM library_schema.reader WHERE (`token` = '{adminToken}');");
            if (res != null)
            {
                if (admin != null)
                {

                    return new AdminStatusResponse
                    {
                        Success = true,
                        Message = "",
                        ReaderList = res,
                    };
                }
                else
                {
                    return new AdminStatusResponse
                        { Success = false, Message = "Have not sign in."};
                }
            }
            else
            {
                if (admin != null)
                {

                    return new AdminStatusResponse
                    {
                        Success = true,
                        Message = "No records",
                    };
                }
                else
                {
                    return new AdminStatusResponse
                        { Success = false, Message = "Have not sign in."};
                }
            }
        }
    }
}