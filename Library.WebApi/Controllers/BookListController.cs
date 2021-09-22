using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using JWT;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities;
using JWT.MvcDemo.Models;
using JWT.Serializers;

namespace Library.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BookListController : ControllerBase
    {
        //Get homepage book list
        [HttpGet]
        public IEnumerable<Books> GetBookLIst()
        {
            Mysql database = new Mysql();
            var res = database.ExecuteGetBooksList("SELECT * FROM library_schema.books");
            return Enumerable.Range(1, res.Count).Select(index => new Books()
            {
                BookId = res[index-1].BookId,
                BookClass = res[index-1].BookClass,
                BookName = res[index-1].BookName,
                BookAuthor = res[index-1].BookAuthor,
                BookPages = res[index-1].BookPages,
                BookAbstract = res[index-1].BookAbstract,
                BookAmount = res[index-1].BookAmount,
                BookCurrentAmount = res[index-1].BookCurrentAmount,
                BookBorrowTimes = res[index-1].BookBorrowTimes,
                BookRemark = res[index-1].BookRemark,
                BookPublishInfo = res [index-1].BookPublishInfo,
                BookPublishDate = res[index-1].BookPublishDate,
                BookLanguage = res[index-1].BookLanguage,
                BookLocation = res[index-1].BookLocation,
                BookImg = res[index-1].BookImg,
                BookContent = res[index-1].BookContent,
                BookSummary = res[index-1].BookSummary
            }).ToArray();
        }
        
        //get book details
        [HttpGet]
        public IEnumerable GetBooksDetail(int bookId)
        {
            Mysql database = new Mysql();
            var res = database.ExecuteGetBooksList($"SELECT * FROM library_schema.books WHERE book_id = {bookId}");
            return Enumerable.Range(1, res.Count).Select(index => new Books()
            {
                BookId = res[index-1].BookId,
                BookClass = res[index-1].BookClass,
                BookName = res[index-1].BookName,
                BookAuthor = res[index-1].BookAuthor,
                BookPages = res[index-1].BookPages,
                BookAbstract = res[index-1].BookAbstract,
                BookAmount = res[index-1].BookAmount,
                BookCurrentAmount = res[index-1].BookCurrentAmount,
                BookBorrowTimes = res[index-1].BookBorrowTimes,
                BookRemark = res[index-1].BookRemark,
                BookPublishInfo = res [index-1].BookPublishInfo,
                BookPublishDate = res[index-1].BookPublishDate,
                BookLanguage = res[index-1].BookLanguage,
                BookLocation = res[index-1].BookLocation,
                BookImg = res[index-1].BookImg,
                BookContent = res[index-1].BookContent,
                BookSummary = res[index-1].BookSummary
            }).ToArray();
        }
        //Submit reserve book apply
        [HttpPost]
        public object ReserveBooks([FromBody] ReserveBooks para)
        {
            string userToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            try
            {
                // //update reader overdue status from borrow_list table
                var reader = database.ExecuteGetUserId($"SELECT reader_id FROM library_schema.reader WHERE (`token` = '{userToken}');");
                int readerId = reader[0].ReaderId;
                var recordList = database.GetOverdueStatus($"SELECT * FROM library_schema.borrow_list WHERE (`reader_id` = '{readerId}');");
                Boolean isOverdue = false;
                if (recordList != null)
                {
                    for (int i = 0; i < recordList.Count; i++)
                    {
                        while (recordList[i].OverdueStatus == true)
                        {
                            isOverdue = true;
                            break;
                        }
                    }
                }
                // var isSignin = database.ExecuteGetBooksList($"SELECT * FROM library_schema.reader WHERE (`token` = '{userToken}');");
                if (reader != null && isOverdue == false)
                {
                    var res = database.ExecuteGetBooksList($"SELECT * FROM library_schema.books WHERE (`book_id` = {para.BookId});");
                    database.ExecuteNonQuery(
                        $"INSERT INTO `library_schema`.`borrow_list` (`reader_id`, `book_id`, `book_name`, `borrow_date`, `return_date`,`penalty`) VALUES ('{readerId}', '{para.BookId}','{res[0].BookName}', '{para.BorrowDate.ToString("yyyy-MM-dd HH:mm:ss")}','{para.BorrowDate.AddDays(7).ToString("yyyyMMddHHmmss")}','{para.Penalty}')");
                    database.ExecuteNonQuery(
                        $" UPDATE `library_schema`.`books` SET `book_current_amount` = book_current_amount-1,`book_borrow_times` = book_borrow_times+1 WHERE (`book_id` = {para.BookId});");
                    return new StatusResponse
                        { Success = true, Message = "Record SuccessFully Saved."};
                }else if(reader != null && isOverdue)
                {
                    return new StatusResponse
                        { Success = false, Message = "You have overdue books or unpaid fines"};
                }
                else
                {
                    return new StatusResponse
                        { Success = false, Message = "Have not sign in."};
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               return new StatusResponse
                    { Success = false, Message = "Invalid Data." };
            }
           
        }
        
        //get borrow records
        [HttpGet]
        public StatusResponse GetBorrowRecords()
        {
            string userToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var reader = database.ExecuteGetUserId($"SELECT reader_id FROM library_schema.reader WHERE (`token` = '{userToken}');");
            if (reader == null)
            {
                return new StatusResponse
                {
                    Success = false, Message = "Have not sign in."
                };
            }
            else
            {
                var resStatue = database.ExecuteGetPenalty(
                    $"SELECT * FROM library_schema.borrow_list WHERE (`reader_id` = '{reader[0].ReaderId}');");
                if (!resStatue)
                {
                    return new StatusResponse()  {
                        Success = false, Message = "Retry."
                    };
                }
                var res = database.ExecuteGetBorrowRecords($"SELECT * FROM library_schema.borrow_list WHERE (`reader_id` = '{reader[0].ReaderId}');");
                if (res == null)
                {
                    return new StatusResponse
                        {
                            Success = true,
                            Message = "No records",
                        };
                }
                else
                {
                    return new StatusResponse
                    {
                        Success = true,
                        Message = "Get Records",
                        BookList = res,

                    };
                }
            }
        }

        //Extend borrow time period (7 days a time)
        [HttpPost]
        public object ExtendBorrowTime([FromBody] ReserveBooks para)
        {
            // Console.WriteLine(para.ReturnDate);
            Mysql database = new Mysql();
            try
            {
                database.ExecuteNonQuery(
                        $" UPDATE `library_schema`.`borrow_list` SET `return_date` = date_add(return_date,interval 1 week) WHERE (`record_id` = {para.RecordId});");
                    return new StatusResponse
                        { Success = true, Message = "Record SuccessFully Saved."};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new StatusResponse
                    { Success = false, Message = "Invalid Data." };
            }
            
        }
    }
}
