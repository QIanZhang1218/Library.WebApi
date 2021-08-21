using System;
using System.Collections.Generic;
using System.Data;
using Library.WebApi;
using Library.WebApi.Controllers;
using MySql.Data.MySqlClient;
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
						Console.WriteLine("@@@");
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
							BookPublishing = dataReader.GetString("book_publishing"),
							BookLanguage = dataReader.GetString("book_language"),
							BookLocation = dataReader.GetString("book_location"),
							BookImg = dataReader.GetString("book_img")
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
		
		//SignUp
		public List<SignIn> VerifyReader(string str)
		{
			MySqlConnection con = new MySqlConnection(constr);
			MySqlDataReader dataReader = null;
			//return value
			List<SignIn> signInInfo = null;
			try
			{
				con.Open();
				string sql = str;
				MySqlCommand command = new MySqlCommand(sql, con);
				dataReader = command.ExecuteReader();
				if (dataReader != null && dataReader.HasRows)
				{
					signInInfo = new List<SignIn>();
					while (dataReader.Read())
					{
						Console.WriteLine("@@@");
						signInInfo.Add(new SignIn()
						{
							Email =  dataReader.GetString("reader_email"),
							Password = dataReader.GetString("reader_pwd")
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
			return signInInfo;
		}

    }
}
