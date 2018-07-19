/**************************************************************
 *  Filename:      ValidateHelper
 *  
 *  Description:  ValidateHelper ClassFile.
 *  
 *  Company:     Coindaq 
 *  
 *  @Author:         ChaoShu
 *  
 *  @Created:       2018/5/11/周五 12:25:29 
 **************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils
{
    public static class ValidateHelper
    {
        private static readonly Regex invalidCharacter = new Regex(@"[^A-Za-z0-9\$_\.\+!*'\(\),-]", RegexOptions.Compiled);

        /// <summary>
        /// 检查传入参数是否是Double类型
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool IsDoubleValue(JObject body, params string[] parameters)
        {
            if (body == null)
                return false;
            foreach (var parameter in parameters)
            {
                try
                {
                    Convert.ToDouble((string)body[parameter]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查传入参数是否是Int64类型
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool IsInt64Value(JObject body, params string[] parameters)
        {
            if (body == null)
                return false;
            foreach (var parameter in parameters)
            {
                try
                {
                    Convert.ToInt64((string)body[parameter]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查传入参数是否是Int32类型
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool IsInt32Value(JObject body, params string[] parameters)
        {
            if (body == null)
                return false;
            foreach (var parameter in parameters)
            {
                try
                {
                    Convert.ToInt32((int)body[parameter]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsInt32Value(params string[] parameters)
        {
            foreach (var parameter in parameters)
            {
                try
                {
                    Convert.ToInt32(parameter);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查参数是否都存在
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool IsExistParameter(JObject body, params string[] parameters)
        {
            if (body == null)
                return false;
            foreach (var item in parameters)
            {
                if (!(body[item] is JValue))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 检查参数队列是否存在
        /// </summary>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool IsExistParameterArray(JObject body, params string[] parameters)
        {
            if (body == null)
                return false;
            foreach (var item in parameters)
            {
                if (!(body[item] is JArray))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 验证邮箱是否合法
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmailFormat(string email)
        {
            Regex regex = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            if (regex.IsMatch(email))
                return true;
            return false;
        }

        /// <summary>
        /// 验证手机号码是否合法
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsPhoneFormat(string phone)
        {
            Regex regex = new Regex("^((\\+86)|(86))?(13|15|18)\\d{9}$");
            if (regex.IsMatch(phone))
                return true;
            return false;
        }

        public static bool IsIdentityIdFormat(string identityid)
        {
            Regex regex = new Regex("^\\d{15}|\\d{18}$");
            if (regex.IsMatch(identityid))
                return true;
            return false;
        }

    }
}
