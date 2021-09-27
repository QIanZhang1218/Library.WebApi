using System;
using Library.WebApi.Models;
using MySql.Data.MySqlClient;

namespace Library.WebApi
{
    public class TimerHelp
    {
	    System.Threading.Timer _timer; 
        private static int interval = 1000 * 60 * 60 * 24;

        public void THelp()
        {
            _timer = new System.Threading.Timer(TimerTask, null, 0, interval);
        }

        public int Count = 1;

        public void TimerTask(object state)
        {
	        decimal penaltySum = 0;
	        Mysql database = new Mysql();
	        var res = database.ExecuteGetBorrowRecords($"SELECT * FROM library_schema.borrow_list");
	        Console.WriteLine(1);
	        foreach (var record in res)
	        {
		        DateTime currentTime = Convert.ToDateTime(DateTime.Now.ToLongDateString().ToString());
		        DateTime returnTime = record.ReturnDate;
		        int result = DateTime.Compare(currentTime, returnTime);
		        int borrowStatus = record.BorrowStatus;
		        bool isPaid = record.PenaltyStatus;
		        if ((borrowStatus == 20 || borrowStatus ==40) && result==1 && isPaid == false)
		        {
			        int delay = Convert.ToInt32(currentTime .Subtract(returnTime).Days.ToString());
			        //Penalty: $5 each day
			        decimal penalty =delay * 5;
			        penaltySum += penalty;
			        database.ExecuteNonQuery(
				        $"UPDATE `library_schema`.`borrow_list` SET `penalty` = '{penalty}' WHERE (`record_id` = '{record.RecordId}')");
		        }
		        database.ExecuteNonQuery(
			        $"UPDATE `library_schema`.`reader` SET `reader_unpaid_penalty` = {penaltySum} WHERE (`reader_id` = {record.UserId})");
	        }
	      
        }
    }
}