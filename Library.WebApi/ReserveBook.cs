using System;
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
        //0:not ruturn 1: return
        public Boolean Status { get; set; }
        public Boolean PenaltyStatus { get; set; }
        public Boolean PickUpStatus { get; set; }
        public Boolean OverdueStatus { get; set; }
    }
}