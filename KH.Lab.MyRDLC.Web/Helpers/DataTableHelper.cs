using System.Data;
using System.Reflection;

namespace KH.Lab.MyRDLC.Web.Helpers;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(this List<T> items)
    {
        var dataTable = new DataTable(typeof(T).Name);
        //Get all the properties
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            type = type.IsEnum ? Enum.GetUnderlyingType(type) : type;
            //Setting column names as Property names
            dataTable.Columns.Add(prop.Name, type);
        }
        foreach (T item in items)
        {
            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null) ?? DBNull.Value;
            }
            dataTable.Rows.Add(values);
        }
        //put a breakpoint here and check datatable
        return dataTable;
    }
}