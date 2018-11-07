using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace WebApplication_NewPC
{
    public static class ToolsHelper
    {

        /// <summary>
        /// 将Get参数转Dictionary
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetToDictionary(this HttpRequestBase request)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in request.QueryString.AllKeys)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    dic.Add(item, request.QueryString[item]);
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
                string p_str = "";
                foreach (var item in dic)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                        p_str += item + "=" + item.Value + "&";
                }
                p_str = p_str.TrimEnd('&');
                p_str = string.IsNullOrWhiteSpace(p_str) ? "" : "?" + p_str;
                return p_str;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string AssembleMovieUrl(this HttpRequestBase request, RouteValueDictionary rValue, Dictionary<string, string> par = null)
        {
            try
            {
                var route_new = request.RequestContext.RouteData.Values;
                foreach (var item in rValue)
                    route_new[item.Key] = item.Value;

                //{action}/{year}/{type}-{country}-{order}-{page}
                string action = route_new["action"]==null?null: route_new["action"].ToString();
                string year = route_new["year"] ==null?null: route_new["year"].ToString();
                year = year == "all" ? null : year;
                string type = route_new["type"] ==null?null: route_new["type"].ToString();
                string country = route_new["country"] ==null?null: route_new["country"].ToString();
                string order = route_new["order"] ==null?null: route_new["order"].ToString();
                string page = route_new["page"] ==null?null: route_new["page"].ToString();
                string url = string.Format("{0}{1}/{2}-{3}-{4}-{5}",
                    string.IsNullOrWhiteSpace(action)?"":"/"+ action,
                    string.IsNullOrWhiteSpace(year) ? "" : "/" + year,
                    type,
                    country,
                    order,
                    page
                    );

                var dic_new = request.GetToDictionary();
                if (par!=null)
                {
                    foreach (var item in par)
                    {
                        if (dic_new.ContainsKey(item.Key))
                            dic_new[item.Key] = item.Value;
                        else
                            dic_new.Add(item.Key, item.Value);
                    }
                }
                return url + dic_new.DictionaryToGet();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}