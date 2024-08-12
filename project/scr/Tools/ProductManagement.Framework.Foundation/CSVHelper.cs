using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Foundation
{
    public class CSVHelper
    {

        public static async Task<Stream> ToCSVAsync(DataTable table)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
            List<string> col_strs = new List<string>();
            foreach (DataColumn col in table.Columns)
            {
                col_strs.Add(col.ColumnName);
            }
            await sw.WriteLineAsync(string.Join(",", col_strs));
            foreach (DataRow row in table.Rows)
            {
                List<string> row_strs = new List<string>();
                foreach (DataColumn col in table.Columns)
                {
                    row_strs.Add(row[col].ToString());
                }
                await sw.WriteLineAsync(string.Join(",", row_strs));
            }
            ms.Position = 0;
            return ms;
        }

        public static  Stream ToCSVAsync(IEnumerable<dynamic> datas)
        {
            StringBuilder sb = new StringBuilder();
            List<string> col_strs = new List<string>();
            var props = datas.First() as IDictionary<string,object>;
            foreach (var prop in props)
            {
                col_strs.Add(prop.Key);
            }
             sb.AppendLine(string.Join(",", col_strs));
            foreach (var  item in datas)
            {
                List<string> row_strs = new List<string>();
                 props = item as IDictionary<string, object>;
                foreach (var prop in props)
                {
                    row_strs.Add(prop.Value?.ToString());
                }
                 sb.AppendLine(string.Join(",", row_strs));
            }
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            ms.Position = 0;
            return ms;
        }
    }
}
