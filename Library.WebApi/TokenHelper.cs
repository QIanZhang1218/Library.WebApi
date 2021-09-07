using System;
using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Text.Json;

namespace Library.WebApi
{
    public class TokenHelper
    {
        public static string SecretKey = "bfdgfdabgifhgfnbibiutbfajbvufafg";//这个服务端加密秘钥 属于私钥
        // public static JsonSerializer myJson = new JsonSerializer();
        
        public static string GenToken(UserInfo M)
        {
            var payload = new Dictionary<string, dynamic>
            {
                {"email", M.Email},//用于存放当前登录人账户信息
                {"UserPwd", M.Password}//用于存放当前登录人登录密码信息
            };
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(payload, SecretKey);
        }
    
        public static UserInfo DecodeToken(string token)
        {
            try
            {
                var json = GetTokenJson(token);
                UserInfo info = JsonSerializer.Deserialize<UserInfo>(json);
                return info;
            }
            catch (Exception)
            {
    
                throw;
            }
        }
    
        public static string GetTokenJson(string token)
        {
            try
            {
                IJwtAlgorithm jwtAlgorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer,validator,urlEncoder,jwtAlgorithm);
                var json = decoder.Decode(token, SecretKey, verify: true);
                return json;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


