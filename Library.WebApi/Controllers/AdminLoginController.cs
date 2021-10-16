using System;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Http;
using JWT.MvcDemo.Models;
using WebApplication;

namespace Library.WebApi.Controllers
{

    //Admin Login 
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminLoginController : ControllerBase
    {
        [HttpPost]
        public Object AdminVerify([FromBody] AdminLogIn para)
        {
            Console.WriteLine(para);
            Mysql database = new Mysql();
            var res = database.VerifyAdmin(
                $"SELECT * FROM library_schema.admin WHERE `admin_email`='{para.Email}' AND `admin_pwd`='{para.Pwd}';");
            if (res.Count == 0)
            {
                return new AdminStatusResponse {Success = false, Message = "Invalid User."};
            }
            else
            {
                var token = res[0].Token;
                database.ExecuteNonQuery(
                    $" UPDATE `library_schema`.`admin` SET `token` = '{token}' WHERE admin_email='{para.Email}';");
                return new AdminStatusResponse {Success = true, Message = "Login Successfully", Token = res[0].Token};
            }
        }
        
           //Admin sign up

        [HttpPost]
        public Object PushAdminSignUp([FromBody] AdminSignUp para)
        {
            Mysql database = new Mysql();
            try
            {
                var res = database.ExecuteNonQueryReturn(
                    $"INSERT INTO `library_schema`.`admin` (`admin_name`, `admin_pwd`, `admin_email`, `admin_gender`) VALUES ('{para.Name}', '{para.Password}', '{para.Email}','{para.Gender}')");
                if (res == "Success")
                {
                    return new AdminStatusResponse
                        { Success = true, Message = "Record SuccessFully Saved."};
                }
                else if (res == $"Duplicate entry '{para.Email}' for key 'admin.admin_email_UNIQUE'")
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
        
        //get admin name
        [HttpGet]
        public GetAdminInfoResponse GetAdminName()
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            var res = database.GetAdminList($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (res != null)
            {
                if (admin != null)
                {

                    return new GetAdminInfoResponse
                    {
                        Success = true,
                        Message = "",
                        AdminList = res,
                    };
                }
                else
                {
                    return new GetAdminInfoResponse
                        {Success = false, Message = "Have not sign in."};
                }
            }
            else
            {
                if (admin != null)
                {

                    return new GetAdminInfoResponse
                    {
                        Success = true,
                        Message = "No records",
                    };
                }
                else
                {
                    return new GetAdminInfoResponse
                        {Success = false, Message = "Have not sign in."};
                }
            }
        }
    }
    
   

    
}
    
  