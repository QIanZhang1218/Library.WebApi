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
		
		// 无返回执行： 修改 or 插入 or 删除
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
							BookPages = dataReader.IsDBNull("book_pages") ? 0 : dataReader.GetInt32("book_pages"),
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
		
		//Get  Borrow Records
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
						//get return status 
						bool isReturn = dataReader.GetBoolean("isReturn");
						bool isPaid = dataReader.GetBoolean("isPaid");
						bool isPickUp = dataReader.GetBoolean("isPickUp");
						readerId = dataReader.GetInt32("reader_id");
						DateTime currentTime = Convert.ToDateTime(DateTime.Now.ToLongDateString().ToString());
						DateTime returnTime = dataReader.GetDateTime("return_date");
						int result = DateTime.Compare(currentTime, returnTime);
						// readers who has pick up their reservation and overdue and the penalty is unpaid then will add penalty amount day by day.
						if (isPickUp && isReturn == false && result==1 && isPaid == false)
						{
						    int delay = Convert.ToInt32(currentTime .Subtract(returnTime).Days.ToString());
						    //Penalty: $5 each day
						    decimal penalty =delay * 5;
						    penaltySum += penalty;
						    ExecuteNonQuery(
						        $"UPDATE `library_schema`.`borrow_list` SET `penalty` = {penalty} WHERE (`record_id` = {dataReader.GetInt32("record_id")})");
						}
						borrowRecords.Add(new ReserveBooks()
						{
							RecordId = dataReader.GetInt32("record_id"),
							BookId = dataReader.GetInt32("book_id"),
							UserId = dataReader.GetInt32("reader_id"),
							BookName = dataReader.GetString("book_name"),
							BorrowDate =  dataReader.GetDateTime("borrow_date"),
							ReturnDate = dataReader.GetDateTime("return_date"),
							Penalty = dataReader.GetDecimal("penalty"),
							Status = dataReader.GetBoolean("isReturn"),
							PickUpStatus = dataReader.GetBoolean("isPickUp"),
							PenaltyStatus = dataReader.GetBoolean("isPaid"),
							OverdueStatus = dataReader.GetBoolean("isOverdue")
						});
					}
					Console.WriteLine("!"+penaltySum);
					ExecuteNonQuery(
						$"UPDATE `library_schema`.`reader` SET `reader_unpaid_penalty` = {penaltySum} WHERE (`reader_id` = {readerId})");
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
		public List<ManageReader> ExecuteGetUserInfo(string str)
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
							// ReaderName = dataReader.GetString("reader_name"),
							// ReaderEmail = dataReader.GetString("reader_email"),
							// ReaderUnpaid = dataReader.GetInt32("reader_unpaid_penalty"),
							// ReaderOnhold = dataReader.GetInt32("reader_borrow_numbers"),
							// ReaderRemark = dataReader.IsDBNull("reader_remark") ? null : dataReader.GetString("reader_remark"),
							// Token = dataReader.IsDBNull("token") ? null : dataReader.GetString("token"),
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
		
		public List<ReserveBooks> GetBorrowListInfo(string str) {
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
							OverdueStatus = dataReader.GetBoolean("isOverdue")
							// ReaderName = dataReader.GetString("reader_name"),
							// ReaderEmail = dataReader.GetString("reader_email"),
							// ReaderUnpaid = dataReader.GetInt32("reader_unpaid_penalty"),
							// ReaderOnhold = dataReader.GetInt32("reader_borrow_numbers"),
							// ReaderRemark = dataReader.IsDBNull("reader_remark") ? null : dataReader.GetString("reader_remark"),
							// Token = dataReader.IsDBNull("token") ? null : dataReader.GetString("token"),
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
		}
    }

