using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERICAPI.Utils
{
    public class JSONUtil
    {
        /// <summary>
        /// JArray转换成html
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        public static string JArray2HtmlTable(JArray jArray)
        {
            // 实例化一个可变字符串对象
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellpadding='1' cellspacing='1' style='border-collapse:collapse;border-color:black;font-family:Courier New;font-size mall'>");
            sbHtml.Append("<tr style='background-color:#9acd32'>");
            // 得到表头信息
            foreach(var ele in (JObject)jArray.First)
            {
                sbHtml.Append("<td align='center'><nobr>");
                sbHtml.Append(ele.Key);
                sbHtml.Append("</nobr></td>");
            }
            sbHtml.Append("</tr>");

            // 遍历表中内容
            foreach(var jObject in jArray)
            {
                sbHtml.Append("<tr>");
                foreach(var ele in (JObject)jObject)
                {
                    sbHtml.Append("<td><nobr>");
                    sbHtml.Append(ele.Value);
                    sbHtml.Append("</nobr></td>");
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");

            // 返回字符串
            return sbHtml.ToString();
        }

        /// <summary>
        /// 传入列名生成html表格
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string JArray2HtmlTable(JArray jArray, IEnumerable<string> columnName)
        {
            // 实例化一个可变字符串对象
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellpadding='1' cellspacing='1' style='border-collapse:collapse;border-color:black;font-family:Courier New;font-size mall'>");
            sbHtml.Append("<tr style='background-color:#9acd32'>");
            // 得到表头信息
            foreach (var ele in columnName)
            {
                sbHtml.Append("<td align='center'><nobr>");
                sbHtml.Append(ele);
                sbHtml.Append("</nobr></td>");
            }
            sbHtml.Append("</tr>");

            // 遍历表中内容
            foreach (var jObject in jArray)
            {
                sbHtml.Append("<tr>");
                foreach (var ele in (JObject)jObject)
                {
                    sbHtml.Append("<td><nobr>");
                    sbHtml.Append(ele.Value);
                    sbHtml.Append("</nobr></td>");
                }
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");

            // 返回字符串
            return sbHtml.ToString();
        }

    }
}
