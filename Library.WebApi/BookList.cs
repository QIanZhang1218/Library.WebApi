using System;

namespace Library.WebApi
{
    public class Books
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookClass { get; set; }
        public string BookAuthor { get; set; }
        public int BookPages { get; set; }
        public string BookAbstract { get; set; }
        public int BookAmount { get; set; }
        public int BookCurrentAmount { get; set; }
        public int BookBorrowTimes { get; set; }
        public string BookRemark { get; set; }
        public string BookPublishing { get; set; }
        public string BookLanguage { get; set; }
        public string BookLocation { get; set; }
        public string BookImg { get; set; }
    }
}
