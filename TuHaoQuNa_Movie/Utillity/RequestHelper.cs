using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Utility
{
    public static class RequestHelper
    {
        /// <summary>
        /// 将From表单值转JObjcet
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static JObject ToJObject(this HttpRequestBase Request)
        {
            JObject obj = new JObject();
            foreach (var item in Request.Form.AllKeys)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    obj.Add(item, Request.Form[item]);
                }
            }
            return obj;
        }

        /// <summary>
        /// 将From表单值转Json
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string ToJsonString(this HttpRequestBase Request)
        {
            return Request.ToJObject().ToString();
        }

        /// <summary>
        /// 将Get参数转Dictionary
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetToDictionary(this HttpRequestBase Request)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Request.QueryString.AllKeys)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    dic.Add(item, Request.QueryString[item]);
                }
            }
            return dic;
        }

        /// <summary>
        /// 将Dictionary转Get参数并UrlEncode
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string DictionaryToGet(this Dictionary<string, string> dic)
        {
            try
            {
                string str = "";
                foreach (var item in dic)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        str += item.Key + "=" + Utils.UrlEncode(item.Value, Encoding.UTF8)+"&";
                    }
                }
                return string.IsNullOrWhiteSpace(str.TrimEnd('&')) ? "" : "?" + str.TrimEnd('&');
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 追加Get参数如果存在则覆盖
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GetAppend(this HttpRequestBase Request, Dictionary<string,string> par)
        {
            try
            {
                var dic_new = Request.GetToDictionary();
                foreach (var item in par)
                {
                    if (dic_new.ContainsKey(item.Key))
                        dic_new[item.Key] = item.Value;
                    else
                        dic_new.Add(item.Key, item.Value);
                }
                return dic_new.DictionaryToGet(); ;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
