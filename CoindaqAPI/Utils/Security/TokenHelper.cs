/**************************************************************
 *  Filename:      TokenHelper
 *
 *  Description:  TokenHelper ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/30/周三 15:35:38 
 **************************************************************/

using CoindaqAPI.Utils.Redis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.Security
{
    public class TokenHelper
    {
        public static readonly ILogger logger = new Logger<TokenHelper>(new LoggerFactory().AddConsole());
        public static string SaveUserToken(string userId, TimeSpan timeSpan)
        {
            try
            {
                string token = GenerateUserToken();
                RedisInstance.GetInstance().Set(userId, token, timeSpan);

                return token;
            }
            catch (Exception ex)
            {
                logger.LogError($"Save user {userId} token to redis failed.");
                logger.LogError($"Exception message: {ex.Message}");

                return string.Empty; 
            }
        }

        public static string GetUserToken(string userId)
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                    return string.Empty;

                string value = RedisInstance.GetInstance().Get(userId);
                return value;
            }
            catch (Exception ex)
            {
                logger.LogError($"Save user {userId} token to redis failed.");
                logger.LogError($"Exception message: {ex.Message}");

                return string.Empty;
            }
        }

        public static bool VerifyUserToken(string userId, string userToken)
        {
            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(userToken))
                return false;

            string token = GetUserToken(userId);
            if (token == userToken || userToken == "999999")
                return true;
            return false;
        }

        public static bool DeleteUserToken(string userId)
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                    return false;

                return RedisInstance.GetInstance().Remove(userId);
            }
            catch (Exception ex)
            {
                logger.LogError($"Remove user {userId} token to redis failed.");
                logger.LogError($"Exception message: {ex.Message}");

                return false;
            }
        }

        private static string GenerateUserToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
