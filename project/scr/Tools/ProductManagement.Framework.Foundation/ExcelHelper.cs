using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Foundation
{
    public static class ExcelHelper
    {
        public static Stream ToExcel(IDictionary<string, IEnumerable<dynamic>> sheets)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            foreach (var sheet in sheets)
            {
               var _sheet = workbook.CreateSheet(sheet.Key);
                var props = sheet.Value.First() as IDictionary<string, object>;
                if (props == null) {
                    throw new Exception("dynamic无法强转为IDictionary<string, object>,请对dynamic中实际类型添加强转IDictionary<string, object>实现或直接存储Dictionary<string, object>对象");
                }
                int rowIndex = 0;
                int cellIndex = 0;
                var rowi = _sheet.CreateRow(rowIndex++);
                foreach (var prop in props)
                {
                    var cell0i = rowi.CreateCell(cellIndex++);
                    cell0i.SetCellValue(prop.Key);
                }
                foreach (var item in sheet.Value)
                {
                    rowi = _sheet.CreateRow(rowIndex++);
                    props = item as IDictionary<string, object>;
                    cellIndex = 0;
                    foreach (var prop in props)
                    {
                        var cellii = rowi.CreateCell(cellIndex++);
                        cellii.SetCellValue(prop.Value?.ToString());
                    }

                }
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms,true);
            ms.Position = 0;
            return ms;
        }

        public static Stream ObjectsToExcel(IDictionary<string, IEnumerable<object>> sheets)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            foreach (var sheet in sheets)
            {
                var _sheet = workbook.CreateSheet(sheet.Key);
                var headers = sheet.Value.First().GetType().GetProperties().Select(p => p.Name);
                int rowIndex = 0;
                int cellIndex = 0;
                var rowi = _sheet.CreateRow(rowIndex++);
                foreach (var header in headers)
                {
                    var cell0i = rowi.CreateCell(cellIndex++);
                    cell0i.SetCellValue(header);
                }
                foreach (var item in sheet.Value)
                {
                    rowi = _sheet.CreateRow(rowIndex++);
                    var props = item.GetType().GetProperties();
                    cellIndex = 0;
                    foreach (var prop in props)
                    {
                        var cellii = rowi.CreateCell(cellIndex++);
                        cellii.SetCellValue(prop.GetValue(item)?.ToString());
                    }

                }
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms, true);
            ms.Position = 0;
            return ms;
        }

    }
}
