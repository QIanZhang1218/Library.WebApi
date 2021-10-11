using System;
using System.Collections.Generic;
using System.Data;
using JWT.MvcDemo.Models;
using Library.WebApi;
using Library.WebApi.Controllers;
using MySql.Data.MySqlClient;
using Ubiety.Dns.Core;
using WebApplication;

namespace Library.WebApi.Models
{
    public class Mysql
    {
    	//连接字符串
    	private static string constr = "server=127.0.0.1;port=3306;database=library_schema;user=root;password=mm970708";
        
		//No return: modify, insert or delete
		public void ExecuteNonQuery(string str)
        {
            var con = new MySqlConnection(constr);
            try
            {
            	//连接数据库
                con.Open();
                string sql = str;
                MySqlCommand command = new MySqlCommand(sql, con);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
            	// 记得关闭连接
                con.Close();
            }
        }
		//With return:modify, insert or delete
		public string ExecuteNonQueryReturn(string str)
		{
			var con = new MySqlConnection(constr);
			try
			{
				//连接数据库
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				command.ExecuteNonQuery();
				return ("Success");
			}
			catch (Exception e)
			{
				return (e.Message);
			}
			finally
			{
				// 记得关闭连接
				con.Close();
			}
		}
		
		//Get books info
		public List<Books> ExecuteGetBooksList(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<Books> booksInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					booksInfo = new List<Books>();
					while (dataReader.Read())
					{
						// Console.WriteLine("@@@");
						booksInfo.Add(new Books()
						{
							BookId = dataReader.GetInt32("book_id"),
							BookClass = dataReader.GetString("book_class"),
							BookName = dataReader.GetString("book_name"),
							BookAuthor = dataReader.GetString("book_author"),
							BookPages = dataReader.IsDBNull("book_pages") ? null : dataReader.GetString("book_pages"),
							BookAbstract = dataReader.IsDBNull("book_abstract") ? null : dataReader.GetString("book_abstract"),
							BookAmount = dataReader.GetInt32("book_amount"),
							BookCurrentAmount = dataReader.GetInt32("book_current_amount"),
							BookBorrowTimes = dataReader.GetInt32("book_borrow_times"),
							BookRemark = dataReader.IsDBNull("book_remark") ? null : dataReader.GetString("book_remark"),
							BookPublishInfo = dataReader.GetString("publication_info"),
							BookPublishDate = dataReader.GetString("publication_date"),
							BookLanguage = dataReader.GetString("book_language"),
							BookLocation = dataReader.GetString("book_location"),
							BookImg = dataReader.GetString("book_img"),
							BookContent = dataReader.IsDBNull("book_content") ? null : dataReader.GetString("book_content"),
							BookSummary = dataReader.GetString("book_summary")
						});
					}
				}

				dataReader.Close();
			}
			catch (Exception e)	
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return booksInfo;
		}
		
		//User SignIn
		public List<UserInfo> VerifyReader(string str)
		{
			bool status = false;
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			StatusResponse result = new StatusResponse();
			//return value
			List<UserInfo> signInInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					status = true;
					signInInfo = new List<UserInfo>();
					while (dataReader.Read())
					{
						// Console.WriteLine("@@@");
						signInInfo.Add(new UserInfo()
						{
							Email =  dataReader.GetString("reader_email"),
							Password = dataReader.GetString("reader_pwd"),
							// UserId  = dataReader.GetInt32("reader_id")
						});
						//Token
						var payload = new Dictionary<string, object>
						{
							{ "email",signInInfo[0].Email + DateTime.Now},
							{ "pwd", signInInfo[0].Password }
						};
						result.Token = JwtHelp.SetJwtEncode(payload);
						result.Success = true;
						result.Message = "成功";
						signInInfo[0].Token = result.Token;
					}
				}

				dataReader.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return signInInfo;
		}
		//Auto cancel reservation for books which have been reserved but not pick up on time
		public void ExeciteAutoCancel(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					int readerId = 0;
					while (dataReader.Read())
					{
						//get return status 
						int borrowStatus = dataReader.GetInt32("borrow_status");
						DateTime currentTime = Convert.ToDateTime(DateTime.Now.ToLongDateString().ToString());
						DateTime borroeTime = dataReader.GetDateTime("borrow_date");
						int autoCancel = DateTime.Compare(currentTime, borroeTime);
						//readers who hace alreay make a reservation but unpick up on the borrow date then will auto cancel the reservation on the next day of borrow date
						if (borrowStatus == 10 && autoCancel == 1)
						{
							ExecuteNonQuery(
								$"UPDATE `library_schema`.`borrow_list` SET `borrow_status` = 99 WHERE (`record_id` = {dataReader.GetInt32("record_id")})");
						}
					}
				}

				dataReader.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
		}

		//Get  Borrow Records
		public Boolean ExecuteGetPenalty(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					decimal penaltySum = 0;
					int readerId = 0;
					while (dataReader.Read())
					{
						//get return status 
						int borrowStatus = dataReader.GetInt32("borrow_status");
						// bool isReturn = dataReader.GetBoolean("isReturn");
						bool isPaid = dataReader.GetBoolean("isPaid");
						// bool isPickUp = dataReader.GetBoolean("isPickUp");
						readerId = dataReader.GetInt32("reader_id");
						DateTime currentTime = Convert.ToDateTime(DateTime.Now.ToLongDateString().ToString());
						DateTime returnTime = dataReader.GetDateTime("return_date");
						int result = DateTime.Compare(currentTime, returnTime);
						// readers who has pick up their reservation and overdue and the penalty is unpaid then will add penalty amount day by day.
						if ((borrowStatus == 20 ) && result==1 && isPaid == false)
						{
						    int delay = Convert.ToInt32(currentTime .Subtract(returnTime).Days.ToString());
						    //Penalty: $5 each day
						    decimal penalty =delay * 5;
						    penaltySum += penalty;
						    ExecuteNonQuery(
						        $"UPDATE `library_schema`.`borrow_list` SET `penalty` = '{penalty}' WHERE (`record_id` = {dataReader.GetInt32("record_id")})");
						    ExecuteNonQuery(
							    $"UPDATE `library_schema`.`reader` SET `reader_unpaid_penalty` = {penaltySum} WHERE (`reader_id` = {readerId})");
						}
					}
					//Console.WriteLine("!"+penaltySum);
					
					return true;
				}

				dataReader.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return false;
		}
		
		//get unpaid penalty 
		public List<ReserveBooks> ExecuteGetBorrowRecords(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<ReserveBooks> borrowRecords = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					decimal penaltySum = 0;
					int readerId = 0;
					borrowRecords = new List<ReserveBooks>();
					while (dataReader.Read())
					{
						object actualReturnDate = null;
						borrowRecords.Add(new ReserveBooks()
						{
							RecordId = dataReader.GetInt32("record_id"),
							BookId = dataReader.GetInt32("book_id"),
							UserId = dataReader.GetInt32("reader_id"),
							BookName = dataReader.GetString("book_name"),
							BorrowDate =  dataReader.GetDateTime("borrow_date"),
							ReturnDate = dataReader.GetDateTime("return_date"),
							Penalty = dataReader.GetDecimal("penalty"),
							BorrowStatus = dataReader.GetInt32("borrow_status"),
							PenaltyStatus = dataReader.GetBoolean("isPaid"),
							ReserveDate = dataReader.GetDateTime("reserve_date"),
							ActualReturnDate =dataReader.IsDBNull("actual_return_date") ? Convert.ToDateTime("2020-01-01 00:00:00") :dataReader.GetDateTime("actual_return_date")
						});
					}
					return borrowRecords;
				}

				dataReader.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return borrowRecords;
		}

		//Administrator Login
		public List<AdminLogIn> VerifyAdmin(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			StatusResponse result = new StatusResponse();
			List<AdminLogIn> AdminLogin = new List<AdminLogIn>();
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					AdminLogin = new List<AdminLogIn>();
					while (dataReader.Read())
					{
						AdminLogin.Add(new AdminLogIn()
							{
								Email =  dataReader.GetString("admin_email"),
								Pwd = dataReader.GetString("admin_pwd")
							}
						);
						//Token
						var payload = new Dictionary<string, object>
						{
							{ "email",AdminLogin[0].Email + DateTime.Now},
							{ "pwd", AdminLogin[0].Pwd }
						};
						result.Token = JwtHelp.SetJwtEncode(payload);
						result.Success = true;
						result.Message = "成功";
						AdminLogin[0].Token = result.Token;
					}
				}
				dataReader.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return AdminLogin;
		}
		
		//get reader info
		public List<ManageReader> ExecuteGetUserId(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<ManageReader> readersInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					readersInfo = new List<ManageReader>();
					while (dataReader.Read())
					{
						readersInfo.Add(new ManageReader()
						{
							ReaderId = dataReader.GetInt32("reader_id"),
						});
					}
				}

				dataReader.Close();
			}
			catch (Exception e)	
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return readersInfo;
		}
		
		public List<ReserveBooks> GetOverdueStatus(string str) {
				MySqlConnection con = new MySqlConnection(constr);
				MySqlDataReader dataReader = null;
				//return value
				List<ReserveBooks> borrowListInfo = null;
				try
				{
					con.Open();
					string sql = str;
					MySqlCommand command = new MySqlCommand(sql, con);
					dataReader = command.ExecuteReader();
					if (dataReader != null && dataReader.HasRows)
					{
						borrowListInfo = new List<ReserveBooks>();
						while (dataReader.Read())
						{
							borrowListInfo.Add(new ReserveBooks()
							{
								UserId = dataReader.GetInt32("reader_id"),
								BorrowStatus = dataReader.GetInt32("borrow_status"),
								ReturnDate = dataReader.GetDateTime("return_date"),
								PenaltyStatus = dataReader.GetBoolean("isPaid")
							});
						}
					}

					dataReader.Close();
				}
				catch (Exception e)	
				{
					Console.WriteLine(e.Message);
				}
				finally
				{
					con.Close();
				}
				return borrowListInfo;
			}
		
		//get admin id and token
		public List<AdminInfo> GetAdminId(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<AdminInfo> adminInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					adminInfo = new List<AdminInfo>();
					while (dataReader.Read())
					{
						adminInfo.Add(new AdminInfo()
						{
							AdminId = dataReader.GetInt32("admin_id"),
						});
					}
				}

				dataReader.Close();
			}
			catch (Exception e)	
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return adminInfo;
		}
		//admin website get reader list table
		public List<ManageReader> GetReaderList(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<ManageReader> readersInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					readersInfo = new List<ManageReader>();
					while (dataReader.Read())
					{
						readersInfo.Add(new ManageReader()
						{
							ReaderId = dataReader.GetInt32("reader_id"),
							ReaderName = dataReader.GetString("reader_name"),
							ReaderEmail = dataReader.GetString("reader_email"),
							ReaderUnpaid = dataReader.GetInt32("reader_unpaid_penalty"),
							ReaderOnhold = dataReader.GetInt32("reader_borrow_numbers"),
							ReaderRemark = dataReader.IsDBNull("reader_remark") ? null : dataReader.GetString("reader_remark"),
							Token = dataReader.IsDBNull("token") ? null : dataReader.GetString("token"),
							Pwd = dataReader.GetString("reader_pwd")
						});
					}
				}

				dataReader.Close();
			}
			catch (Exception e)	
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return readersInfo;
		}
		
		public List<AdminInfo> GetAdminList(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<AdminInfo> adminInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader(); 
				if (dataReader != null && dataReader.HasRows)
				{
					adminInfo = new List<AdminInfo>();
					while (dataReader.Read())
					{
						adminInfo.Add(new AdminInfo()
						{
							AdminId = dataReader.GetInt32("admin_id"),
							AdminName = dataReader.GetString("admin_name"),
							AdminEmail = dataReader.GetString("admin_email"),
							AdminGender = dataReader.GetString("admin_gender"),
							AdminToken = dataReader.IsDBNull("token") ? null : dataReader.GetString("token"),
							AdminRemark = dataReader.IsDBNull("admin_remark") ? null : dataReader.GetString("admin_remark"),
							AdminPassword = dataReader.GetString("admin_pwd")
						});
					}
				}

				dataReader.Close();
			}
			catch (Exception e)	
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return adminInfo;
		}
		
		//unpaid penalty
		public decimal ExecuteUnpaidPenalty(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			decimal result = 0;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					while (dataReader.Read())
					{
						result = (decimal) dataReader.GetDouble("Total");
					}
				}

				dataReader.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}

			return result;
		}
		
		//user message
		public List<UserMessage> GetUserMessage(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<UserMessage> readersInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					readersInfo = new List<UserMessage>();
					while (dataReader.Read())
					{
						readersInfo.Add(new UserMessage()
						{
							MessageId = dataReader.GetInt32("id"),
							ReaderName = dataReader.GetString("name"),
							ReaderEmail = dataReader.GetString("email"),
							ReaderMessage = dataReader.GetString("message"),
							MessageStatus = dataReader.GetString("status")
						});
					}
				}

				dataReader.Close();
			}
			catch (Exception e)	
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				con.Close();
			}
			return readersInfo;
		}
    }
}



