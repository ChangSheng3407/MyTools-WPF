using HandyControl.Controls;
using Microsoft.Win32;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;

namespace MyTools.UserControl.Tools
{
    /// <summary>
    /// JsonExcel.xaml 的交互逻辑
    /// </summary>
    public partial class JsonExcel
    {
        public JsonExcel()
        {
            InitializeComponent();
        }
        public void Json2Excel(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "请选择.Json文件",
                    Filter = "Json文件|*.json"
                    //Filter = "Excel文件|*.xls;*.xlsx"
                };
                var result = dialog.ShowDialog();
                if (result == false) return;
                var json = File.ReadAllText(dialog.FileName);
                var jo = JsonObject.Parse(json);
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                var values = new List<Dictionary<string, object>>();
                foreach (var jarray in jo.AsArray())
                {
                    var dic = new Dictionary<string, object>();
                    foreach (var item in jarray.AsObject())
                    {
                        dic.Add(item.Key, item.Value);
                    }
                    values.Add(dic);
                }

                string createPath = Path.Combine(desktop, $"{DateTime.Now.Ticks}excel.xlsx");
                MiniExcel.SaveAs(createPath, values);
                Growl.Success($"转换成功[{createPath}]");
            }
            catch (Exception)
            {
                Growl.Error("文件内容较为复杂，暂不支持");
            }
        }

        private void Excel2Json(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "请选择excel文件",
                    Filter = "Excel文件|*.xls;*.xlsx"
                };
                var result = dialog.ShowDialog();
                if (result == false) return;
                var rows = MiniExcel.Query(dialog.FileName).ToList();
                var keysmap = new Dictionary<string, string>();

                foreach (KeyValuePair<string, object> item in rows[0])
                {
                    if (item.Value != null)
                    {
                        keysmap.Add(item.Key, item.Value.ToString());
                    }
                }

                var values = new List<Dictionary<string, object>>();
                rows.RemoveAt(0);
                foreach (IDictionary<string, object> row in rows)
                {
                    var value = new Dictionary<string, object>();
                    foreach (var item in row)
                    {
                        string key;
                        if (keysmap.TryGetValue(item.Key, out key))
                        {
                            value.Add(key, item.Value);
                        }
                    }
                    values.Add(value);
                }

                var content = JsonSerializer.Serialize(values, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true });
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string createPath = Path.Combine(desktop, $"{DateTime.Now.Ticks}json.json");
                File.WriteAllText(createPath, content);
                Growl.Success($"转换成功[{createPath}]");
            }
            catch (Exception)
            {
                Growl.Error("文件内容较为复杂，暂不支持");
            }
        }
    }
}
