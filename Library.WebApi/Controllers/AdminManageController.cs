using System;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Http;
using JWT.MvcDemo.Models;


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
            var res = database.GetReaderList($"SELECT * FROM library_schema.reader");
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
                        {Success = false, Message = "Have not sign in."};
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
                        {Success = false, Message = "Have not sign in."};
                }
            }
        }

        //delete selected reader info}
        [HttpPost]
        public object DeleteUserInfo([FromBody] ManageReader para)
        {
            Boolean brrowFine = true;
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            //Check borrow list to see if the user have paid fines
            var resBorrow =
                database.ExecuteGetBorrowRecords(
                    $"SELECT * FROM library_schema.borrow_list WHERE (`reader_id` = '{para.ReaderId}');");
            //check reader to see if the user have return all borrowed books
            var resReader =
                database.GetReaderList(
                    $"SELECT * FROM library_schema.reader WHERE (`reader_id` = '{para.ReaderId}')");
            //No borrow records then no borrow fine
            if (resBorrow ==null)
            {
                brrowFine = false;
            }
            else 
            {
                if (resBorrow[0].Penalty != 0)
                {
                    if (resBorrow[0].PenaltyStatus)
                    {
                        brrowFine = false;
                    }
                }
                else
                {
                    brrowFine = false;
                }
            }

            if (admin != null && !brrowFine  && resReader[0].ReaderOnhold == 0)
            {
                try
                {
                    database.ExecuteNonQuery(
                        $" DELETE FROM `library_schema`.`reader` WHERE (`reader_id` = {para.ReaderId});");
                    return new AdminStatusResponse
                        {Success = true, Message = "Delete Successful."};
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                if (admin == null)
                {
                    return new AdminStatusResponse
                        {Success = false, Message = "Please login first."};
                }else if (brrowFine)
                {
                    return new AdminStatusResponse
                        {Success = false, Message = "Unpaid fine exist."};
                }else if (resReader[0].ReaderOnhold > 0)
                {
                    return new AdminStatusResponse
                        {Success = false, Message = "Exist on hold books."};
                }

                return new AdminStatusResponse
                    {Success = false, Message = "Failed"};
            }
        }

        [HttpPost]
        public object EditUserInfo([FromBody] ManageReader para)
        {
            Boolean brrowFine = true;
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                try
                {
                    database.ExecuteNonQuery(
                        $" UPDATE `library_schema`.`reader` SET `reader_name` = '{para.ReaderName}', `reader_pwd` = '{para.Pwd}', `reader_email` = '{para.ReaderEmail}', `reader_borrow_numbers` = '{para.ReaderOnhold}' WHERE (`reader_id` = '{para.ReaderId}');");
                    return new AdminStatusResponse
                        {Success = true, Message = "Edit Successful."};
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                return new AdminStatusResponse
                    {Success = false, Message = "Have not log in."};
            }
        }
    }
}
    