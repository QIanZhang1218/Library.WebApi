using System.Collections.Generic;
using Library.WebApi;
using WebApplication;

namespace JWT.MvcDemo.Models
{
    public class StatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }

        public List<ReserveBooks> BookList { get; set; }
    }

    public class AdminStatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }
    }

    public class GetUserInfoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }
        public List<ManageReader> ReaderList { get; set; }
    }

    public class GetAdminInfoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }
        public List<AdminInfo> AdminList { get; set; }
    }
}