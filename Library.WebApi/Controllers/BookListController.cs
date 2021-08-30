using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using WebApplication;

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
            string[] column = new string[] { "bookId"};
            int[] columnvalue = new int[1] { bookId };
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
            Console.WriteLine(para);
            Mysql database = new Mysql();
            try
            {
                var res = database.ExecuteGetBooksList($"SELECT * FROM library_schema.books WHERE (`book_id` = {para.BookId});");
                database.ExecuteNonQuery(
                    $"INSERT INTO `library_schema`.`borrow_list` (`reader_id`, `book_id`, `book_name`, `borrow_date`, `return_date`,`penalty`) VALUES ('{para.UserId}', '{para.BookId}','{res[0].BookName}', '{para.BorrowDate.ToString("yyyy-MM-dd HH:mm:ss")}','{para.BorrowDate.AddDays(7).ToString("yyyyMMddHHmmss")}','{para.Penalty}')");
                database.ExecuteNonQuery(
                    $" UPDATE `library_schema`.`books` SET `book_current_amount` = book_current_amount-1 WHERE (`book_id` = {para.BookId});");
                return new Response
                    { Status = "Success", Message = "Record SuccessFully Saved.",Token="fake-jwt-token" };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               return new Response
                    { Status = "Error", Message = "Invalid Data." };
            }
           
        }
        
        //get borrow records
        [HttpGet]
        public IEnumerable<ReserveBooks> GetBorrowRecords()
        {
            Mysql database = new Mysql();
            var res = database.ExecuteGetBorrowRecords("SELECT * FROM library_schema.borrow_list WHERE reader_id = 1;");

            return Enumerable.Range(1, res.Count).Select(index => new ReserveBooks()
            {
                RecordId = res[index-1].RecordId,
                BookId = res[index-1].BookId,
                BookName = res[index-1].BookName,
                UserId = res[index-1].UserId,
                BorrowDate = res[index-1].BorrowDate,
                ReturnDate = res[index-1].ReturnDate,
                Penalty = res[index-1].Penalty,
                Status = res[index-1].Status

            }).ToArray();
        }
        
    }
}
