using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CuttingMrpBackFlushWPF.Utils
{
    public static class ExportExcel
    {
        #region 导出DataGrid数据到Excel

        /// <summary>
        /// CSV格式化
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>格式化数据</returns>
        private static string FormatCSVField(string data)
        {
            return String.Format("\"{0}\"", data.Replace("\"", "\"\"\"").Replace("\n", "").Replace("\r", ""));
        }

        /// <summary>
        /// 导出DataGrid数据到Excel
        /// </summary>
        /// <param name="withHeaders">是否需要表头</param>
        /// <param name="grid">DataGrid</param>
        /// <returns>Excel内容字符串</returns>
        public static string ExportDataGrid(bool withHeaders, DataGrid grid)
        {
            string colPath;
            System.Reflection.PropertyInfo propInfo;
            System.Windows.Data.Binding binding;
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            System.Collections.IList source = (grid.ItemsSource as System.Collections.IList);
            if (source == null)
                return "";
            List<string> headers = new List<string>();
            grid.Columns.ToList().ForEach(col =>
            {
                if (col is DataGridBoundColumn)
                {
                    headers.Add(FormatCSVField(col.Header.ToString()));
                }
            });
            strBuilder.Append(String.Join(",", headers.ToArray())).Append("\r\n");
            foreach (Object data in source)
            {
                List<string> csvRow = new List<string>();
                foreach (DataGridColumn col in grid.Columns)
                {
                    if (col is DataGridBoundColumn)
                    {
                        binding = (Binding)(col as DataGridBoundColumn).Binding;
                        colPath = binding.Path.Path;
                        propInfo = data.GetType().GetProperty(colPath);
                        if (propInfo != null)
                        {
                            csvRow.Add(FormatCSVField(propInfo.GetValue(data, null).ToString()));
                        }
                    }
                }
                strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\r\n");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 导出DataGrid数据到Excel
        /// </summary>
        /// <param name="withHeaders">是否需要表头</param>
        /// <param name="grid">DataGrid</param>
        /// <returns>Excel内容字符串</returns>
        public static string ExportDataGrid(bool withHeaders, DataGrid grid, bool dataBind)
        {
            string colPath;
            System.Reflection.PropertyInfo propInfo;
            System.Windows.Data.Binding binding;
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            System.Collections.IList source = (grid.ItemsSource as System.Collections.IList);
            if (source == null)
                return "";
            List<string> headers = new List<string>();
            grid.Columns.ToList().ForEach(col =>
            {
                if (col is DataGridTemplateColumn)
                {
                    if (col.Header != null)
                    {
                        headers.Add(FormatCSVField(col.Header.ToString()));
                    }
                    else
                    {
                        headers.Add(string.Empty);
                    }
                }
            });
            strBuilder.Append(String.Join(",", headers.ToArray())).Append("\r\n");
            foreach (Object data in source)
            {
                List<string> csvRow = new List<string>();
                foreach (DataGridColumn col in grid.Columns)
                {
                    if (col is DataGridTemplateColumn)
                    {
                        FrameworkElement cellContent = col.GetCellContent(data);
                        TextBlock block = null;
                        if (cellContent.GetType() == typeof(Grid))
                        {
                            block = cellContent.FindName("TempTextblock") as TextBlock;
                        }
                        else
                        {
                            block = cellContent as TextBlock;
                        }
                        if (block != null)
                        {
                            csvRow.Add(FormatCSVField(block.Text));
                        }
                    }
                }
                strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\r\n");
                //strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\t");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 导出DataGrid数据到Excel为CVS文件
        /// 使用utf8编码 中文是乱码  改用Unicode编码
        /// 
        /// </summary>
        /// <param name="withHeaders">是否带列头</param>
        /// <param name="grid">DataGrid</param>
        public static void ExportDataGridSaveAs(bool withHeaders, DataGrid grid,string fileName)
        {
            string data = ExportDataGrid(true, grid);
            SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "xls",
                FileName = fileName,
                Filter = "Excel Files (*.xls)|*.xls|CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 1
            };
            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    using (StreamWriter writer = new StreamWriter(stream, System.Text.UnicodeEncoding.Unicode))
                    {
                        data = data.Replace(",", "\t");
                        writer.Write(data);
                        writer.Close();
                    }
                    stream.Close();
                }
            }
        }

        #endregion 导出DataGrid数据到Excel
    }
}
