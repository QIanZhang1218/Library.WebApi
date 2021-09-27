using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Library.WebApi
{
    public class ReserveBooks
    {
        public int RecordId { get; set; }

        [Required(ErrorMessage = "UserId can not be empty")]
        public int UserId { get; set; }

        public int BookId { get; set; }
        public string BookName { get; set; }

        [Required(ErrorMessage = "Start date can not be empty")]
        public DateTime BorrowDate { get; set;  }

        public DateTime ReturnDate { get; set; }
        public decimal Penalty { get; set; }
        //borrow status: 10:reserved;20:picked;30:returned;40:overdue
        public int BorrowStatus { get; set; }
        public Boolean PenaltyStatus { get; set; }
        
        public DateTime ReserveDate { get; set;  }
    }

    public class BorrowRecordsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }
        public List<ReserveBooks> BorrowRecords { get; set; }
    }
}