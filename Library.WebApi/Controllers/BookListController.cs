using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;

namespace Library.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BookListController : ControllerBase
    {
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
                BookPublishing = res[index-1].BookPublishing,
                BookLanguage = res[index-1].BookLanguage,
                BookLocation = res[index-1].BookLocation,
                BookImg = res[index-1].BookImg
            }).ToArray();
        }
    }
}
