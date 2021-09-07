using JWT.MvcDemo.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.WebApi;

namespace Library.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class JwtController : ControllerBase
    {
        [HttpPost]

        /// <summary>
        /// 创建jwtToken
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public StatusResponse CreateToken(string username, string pwd)
        {

            StatusResponse result = new StatusResponse();

            //假设用户名为"admin"，密码为"123"  
            if (username == "admin" && pwd == "123")
            {

                var payload = new Dictionary<string, object>
                {
                    { "username",username },
                    { "pwd", pwd }
                };

                result.Token = JwtHelp.SetJwtEncode(payload);
                result.Success = true;
                result.Message = "成功";
            }
            else
            {
                result.Token = "";
                result.Success = false;
                result.Message = "生成token失败";
            }

            //return Json(result);
            //get请求需要修改成这样
            return result;
        }
    }
}