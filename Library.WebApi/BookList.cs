using System;

namespace Library.WebApi
{
    public class Books
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookClass { get; set; }
        public string BookAuthor { get; set; }
        public string BookPages { get; set; }
        public string BookAbstract { get; set; }
        public int BookAmount { get; set; }
        public int BookCurrentAmount { get; set; }
        public int BookBorrowTimes { get; set; }
        public string BookRemark { get; set; }
        public string BookPublishInfo { get; set; }
        public string BookPublishDate { get; set; }
        public string BookLanguage { get; set; }
        public string BookLocation { get; set; }
        public string BookImg { get; set; }
        public string BookContent { get; set; }
        public string BookSummary { get; set; }
    }
}
