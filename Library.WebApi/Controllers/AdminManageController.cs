using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Http;
using JWT.MvcDemo.Models;
using WebApplication;


namespace Library.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminManagementController : ControllerBase
    {
        //get reader list table
        [HttpGet]
        public GetUserInfoResponse GetReaderInfo()
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            var res = database.GetReaderList($"SELECT * FROM library_schema.reader");
            if (res != null)
            {
                if (admin != null)
                {

                    return new GetUserInfoResponse
                    {
                        Success = true,
                        Message = "",
                        ReaderList = res,
                    };
                }
                else
                {
                    return new GetUserInfoResponse
                        {Success = false, Message = "Have not sign in."};
                }
            }
            else
            {
                if (admin != null)
                {

                    return new GetUserInfoResponse
                    {
                        Success = true,
                        Message = "No records",
                    };
                }
                else
                {
                    return new GetUserInfoResponse
                        {Success = false, Message = "Have not sign in."};
                }
            }
        }

        //delete selected reader info}
        [HttpPost]
        public object DeleteUserInfo([FromBody] ManageReader para)
        {
            Boolean brrowFine = true;
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            //Check borrow list to see if the user have paid fines
            var resBorrow =
                database.ExecuteGetBorrowRecords(
                    $"SELECT * FROM library_schema.borrow_list WHERE (`reader_id` = '{para.ReaderId}');");
            //check reader to see if the user have return all borrowed books
            var resReader =
                database.GetReaderList(
                    $"SELECT * FROM library_schema.reader WHERE (`reader_id` = '{para.ReaderId}')");
            //No borrow records then no borrow fine
            if (resBorrow == null)
            {
                brrowFine = false;
            }
            else
            {
                if (resBorrow[0].Penalty != 0)
                {
                    if (resBorrow[0].PenaltyStatus)
                    {
                        brrowFine = false;
                    }
                }
                else
                {
                    brrowFine = false;
                }
            }

            if (admin != null && !brrowFine && resReader[0].ReaderOnhold == 0)
            {
                try
                {
                    database.ExecuteNonQuery(
                        $" DELETE FROM `library_schema`.`reader` WHERE (`reader_id` = {para.ReaderId});");
                    return new GetUserInfoResponse
                        {Success = true, Message = "Delete Successful."};
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                if (admin == null)
                {
                    return new GetUserInfoResponse
                        {Success = false, Message = "Please login first."};
                }
                else if (brrowFine)
                {
                    return new GetUserInfoResponse
                        {Success = false, Message = "Unpaid fine exist."};
                }
                else if (resReader[0].ReaderOnhold > 0)
                {
                    return new GetUserInfoResponse
                        {Success = false, Message = "Exist on hold books."};
                }

                return new GetUserInfoResponse
                    {Success = false, Message = "Failed"};
            }
        }

        //edit reader info
        [HttpPost]
        public object EditUserInfo([FromBody] ManageReader para)
        {
            Boolean brrowFine = true;
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                try
                {
                    var res = database.ExecuteNonQueryReturn(
                        $" UPDATE `library_schema`.`reader` SET `reader_name` = '{para.ReaderName}', `reader_pwd` = '{para.Pwd}', `reader_email` = '{para.ReaderEmail}', `reader_borrow_numbers` = '{para.ReaderOnhold}' WHERE (`reader_id` = '{para.ReaderId}');");
                    if (res == "Success")
                    {
                        return new AdminStatusResponse
                            {Success = true, Message = "Edit Successful."};
                    }
                    else if (res == $"Duplicate entry '{para.ReaderEmail}' for key 'reader.reader_email_UNIQUE'")
                    {
                        return new AdminStatusResponse
                        {
                            Success = false, Message = "Email already exist"
                        };
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                return ("Failed");
            }
            else
            {
                return new GetUserInfoResponse
                    {Success = false, Message = "Have not log in."};
            }
        }

        //get admin list
        [HttpGet]
        public GetAdminInfoResponse GetAdminInfo()
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            var res = database.GetAdminList($"SELECT * FROM library_schema.admin");
            if (res != null)
            {
                if (admin != null)
                {

                    return new GetAdminInfoResponse
                    {
                        Success = true,
                        Message = "",
                        AdminList = res,
                    };
                }
                else
                {
                    return new GetAdminInfoResponse
                        {Success = false, Message = "Have not sign in."};
                }
            }
            else
            {
                if (admin != null)
                {

                    return new GetAdminInfoResponse
                    {
                        Success = true,
                        Message = "No records",
                    };
                }
                else
                {
                    return new GetAdminInfoResponse
                        {Success = false, Message = "Have not sign in."};
                }
            }
        }

        //edit admin info
        [HttpPost]
        public object EditAdminInfo([FromBody] AdminInfo para)
        {
            Mysql database = new Mysql();
            string adminToken = Request.Cookies["token"];
            var adminList = database.GetAdminList($"SELECT * FROM library_schema.admin;");
            if (adminList == null)
            {
                return new AdminStatusResponse
                {
                    Success = false, Message = "No records"
                };
            }

            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                try
                {
                    var res = database.ExecuteNonQueryReturn(
                        $" UPDATE `library_schema`.`admin` SET `admin_name` = '{para.AdminName}', `admin_pwd` = '{para.AdminPassword}', `admin_email` = '{para.AdminEmail}', `admin_gender` = '{para.AdminGender}',`admin_remark` = '{para.AdminRemark}'WHERE (`admin_id` = '{para.AdminId}');");
                    if (res == "Success")
                    {
                        return new AdminStatusResponse
                            {Success = true, Message = "Edit Successful."};
                    }
                    else if (res == $"Duplicate entry '{para.AdminEmail}' for key 'admin.admin_email_UNIQUE'")
                    {
                        return new AdminStatusResponse
                        {
                            Success = false, Message = "Email already exist"
                        };
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return new GetAdminInfoResponse
            {
                Success = false, Message = "Have not log in."
            };
        }

        //Delete admin 
        [HttpPost]
        public object DeleteAdminInfo([FromBody] AdminInfo para)
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            //No borrow records then no borrow fine
            if (admin != null)
            {
                try
                {
                    database.ExecuteNonQuery(
                        $" DELETE FROM `library_schema`.`admin` WHERE (`admin_id` = {para.AdminId});");
                    return new GetUserInfoResponse
                        {Success = true, Message = "Delete Successful."};
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return new GetUserInfoResponse
                {Success = false, Message = "Please login first."};
        }

        //Edit book
        [HttpPost]
        public object EditBookInfo([FromBody] Books para)
        {
            Mysql database = new Mysql();
            string adminToken = Request.Cookies["token"];
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                try
                {
                    var res = database.ExecuteNonQueryReturn(
                        $" UPDATE `library_schema`.`books` SET `book_name` = '{para.BookName}', `book_class` = '{para.BookClass}', `book_author` = '{para.BookAuthor}', `book_pages` = '{para.BookPages}',`book_abstract` = '{para.BookAbstract}',`book_amount` = '{para.BookAmount}', `book_current_amount` = '{para.BookCurrentAmount}', `book_borrow_times` = '{para.BookBorrowTimes}', `book_remark` = '{para.BookRemark}', `publication_info` = '{para.BookPublishInfo}', `book_language` = '{para.BookLanguage}',`book_location` = '{para.BookLocation}', `book_img` = '{para.BookImg}', `publication_date` = '{para.BookPublishDate}',`book_content` = '{para.BookContent}',`book_summary` = '{para.BookSummary}' WHERE (`book_id` = '{para.BookId}');");
                    if (res == "Success")
                    {
                        return new AdminStatusResponse
                            {Success = true, Message = "Edit Successful."};
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return new GetAdminInfoResponse
            {
                Success = false, Message = "Have not log in."
            };
        }

        //delete book
        [HttpPost]
        public object DeleteBookInfo([FromBody] Books para)
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            //No borrow records then no borrow fine
            if (admin != null)
            {
                try
                {
                    database.ExecuteNonQuery(
                        $" DELETE FROM `library_schema`.`books` WHERE (`book_id` = {para.BookId});");
                    return new GetUserInfoResponse
                        {Success = true, Message = "Delete Successful."};
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return new GetUserInfoResponse
                {Success = false, Message = "Please login first."};
        }

        //Add new book
        [HttpPost]
        public object AddNewBook([FromBody] Books para)
        {
            Mysql database = new Mysql();
            string adminToken = Request.Cookies["token"];
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                try
                {
                    var res = database.ExecuteNonQueryReturn(
                        $"INSERT INTO `library_schema`.`books` (`book_class`, `book_name`, `book_author`, `book_pages`,`book_abstract`,`book_amount`,`book_current_amount`,`book_borrow_times`,`book_remark`,`publication_info`,`book_language`,`book_location`,`book_img`,`publication_date`,`book_content`,`book_summary`) VALUES ('{para.BookClass}', '{para.BookName}', '{para.BookAuthor}','{para.BookPages}','{para.BookAbstract}','{para.BookAmount}','{para.BookCurrentAmount}','{para.BookBorrowTimes}','{para.BookRemark}','{para.BookPublishInfo}','{para.BookLanguage}','{para.BookLocation}','{para.BookImg}','{para.BookPublishDate}','{para.BookContent}','{para.BookSummary}')");
                    if (res == "Success")
                    {
                        return new AdminStatusResponse
                            {Success = true, Message = "Edit Successful."};
                    }

                    return new AdminStatusResponse
                    {
                        Success = false, Message = res
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return new GetAdminInfoResponse
            {
                Success = false, Message = "Have not log in."
            };
        }

        //get borrow records
        [HttpGet]
        public BorrowRecordsResponse GetBorrowRecords()
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            var res = database.ExecuteGetBorrowRecords($"SELECT * FROM library_schema.borrow_list");
            if (admin == null)
            {
                return new BorrowRecordsResponse
                {
                    Success = false,
                    Message = "Please Log In first.",
                };
            }
            if (res == null)
            {
                return new BorrowRecordsResponse
                {
                    Success = true,
                    Message = "No records",
                };
            }
            else
            {
                return new BorrowRecordsResponse
                {
                    Success = true,
                    Message = "Get Records",
                    BorrowRecords = res,

                };
            }
        }
        
        //pick up reserve book
        [HttpPost]
        public BorrowRecordsResponse PickUpBook([FromBody] ReserveBooks para)
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                try
                {
                    var res = database.ExecuteNonQueryReturn(
                        $"UPDATE `library_schema`.`borrow_list` SET `borrow_status` = 20 WHERE (`record_id` = '{para.RecordId}')");
                    if (res == "Success")
                    {
                        return new BorrowRecordsResponse
                            {Success = true, Message = "Pick Up Successful."};
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return new BorrowRecordsResponse
            {
                Success = false, Message = "Please Log In first."
            };
        }
        //Return borrow book
        [HttpPost]
        public BorrowRecordsResponse ReturnBorrowBook([FromBody] ReserveBooks para)
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                try
                {
                    if (para.Penalty > 0 && para.PenaltyStatus == false)
                    {
                        var res = database.ExecuteNonQueryReturn(
                            $"UPDATE `library_schema`.`borrow_list` SET `borrow_status` = 40 WHERE (`record_id` = '{para.RecordId}')");
                        if (res == "Success")
                        {
                            return new BorrowRecordsResponse
                                {Success = true, Message = "Returned Successful.Please pay overdue penalty."};
                        }
                    }else if (para.Penalty == 0 || para.PenaltyStatus)
                    {
                        var res = database.ExecuteNonQueryReturn(
                            $"UPDATE `library_schema`.`borrow_list` SET `borrow_status` = 30 WHERE (`record_id` = '{para.RecordId}')");
                        if (res == "Success")
                        {
                            return new BorrowRecordsResponse
                                {Success = true, Message = "Returned Successful"};
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return new BorrowRecordsResponse
            {
                Success = false, Message = "Please Log In first."
            };
        }
        
        //Cancel reservation
        [HttpPost]
        public BorrowRecordsResponse CancelReservation([FromBody] ReserveBooks para)
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            if (admin != null)
            {
                if (para.BorrowStatus != 10)
                {
                    switch (@para.BorrowStatus)
                    {
                        case 20:
                            return new BorrowRecordsResponse
                            {
                                Success = false, Message = "You have already picked the book."
                            };
                        case 30:
                            return new BorrowRecordsResponse
                            {
                                Success = false, Message = "You have already returned book."
                            };
                        case 40:
                            return new BorrowRecordsResponse
                            {
                                Success = false, Message = "Please pay the overdue fine."
                            };
                        case 99:
                            return new BorrowRecordsResponse
                            {
                                Success = false, Message = "The reservation has already been cancelled."
                            };
                        default:
                            return new BorrowRecordsResponse
                            {
                                Success = false, Message = "Failed."
                            };
                    }
                }
                var res = database.ExecuteNonQueryReturn(
                    $"UPDATE `library_schema`.`borrow_list` SET `borrow_status` = 99 WHERE (`record_id` = '{para.RecordId}')");
                if (res == "Success")
                {
                    return new BorrowRecordsResponse
                        {Success = true, Message = "Reservation Cancelled."};
                }
            }

            return new BorrowRecordsResponse
            {
                Success = false, Message = "Please Log In first."
            };
        }
        
        //Dashboard unpaid penalties
        [HttpGet]
        public DashboardResponse GetUnpaidPenalty()
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            decimal res = database.ExecuteUnpaidPenalty($"SELECT SUM(reader_unpaid_penalty) AS Total from reader;");
            if (admin == null)
            {
                return new DashboardResponse
                {
                    Success = false,
                    Message = "Please Log In first.",
                };
            }

            return new DashboardResponse
            {
                Success = true, Message = "Get unpaid penalty", ReaderUnpaid = res
            };
        }
        
        //Dashboard recent reservation
        [HttpGet]
        public StatusResponse GetRecentReservation()
        {
            string adminToken = Request.Cookies["token"];
            Mysql database = new Mysql();
            var admin = database.GetAdminId($"SELECT * FROM library_schema.admin WHERE (`token` = '{adminToken}');");
            var res = database.ExecuteGetBorrowRecords($"SELECT * FROM library_schema.borrow_list WHERE DATE_SUB(CURDATE(), INTERVAL 7 DAY) <= date(`reserve_date`);");
            if (admin == null)
            {
                return new StatusResponse
                {
                    Success = false,
                    Message = "Please Log In first.",
                };
            }
            if (res == null)
            {
                return new StatusResponse
                {
                    Success = true,
                    Message = "No records",
                };
            }
            else
            {
                return new StatusResponse
                {
                    Success = true,
                    Message = "Get Records",
                    BookList = res,
                };
            }
        }
        
        //Dashboard most popular books
        [HttpGet]
        public IEnumerable<Books> GetMostPopularBookLIst()
        {
            Mysql database = new Mysql();
            var res = database.ExecuteGetBooksList("SELECT * FROM library_schema.books order by book_borrow_times desc limit 5");
            return Enumerable.Range(1, res.Count).Select(index => new Books()
            {
                BookId = res[index-1].BookId,
                BookName = res[index-1].BookName,
                BookAuthor = res[index-1].BookAuthor,
                BookBorrowTimes = res[index-1].BookBorrowTimes,
                BookRemark = res[index-1].BookRemark,
                BookPublishInfo = res [index-1].BookPublishInfo,
                BookPublishDate = res[index-1].BookPublishDate,
                BookLanguage = res[index-1].BookLanguage,
            }).ToArray();
        }

    }
}
    