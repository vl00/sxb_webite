using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public static class DataTableToList
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            if (table.Rows.Count == 0)
                return null;

            List<T> data = new List<T>();
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach (DataRow row in table.Rows)
            {
                T model = new T();
                foreach (PropertyInfo Property in propertyInfos)
                {
                    if (table.Columns.Contains(Property.Name))
                    {

                        if (row[Property.Name] != DBNull.Value)
                            Property.SetValue(model, row[Property.Name]);
                    }
                }
                data.Add(model);
            }
            return data;
        }
    }
}
