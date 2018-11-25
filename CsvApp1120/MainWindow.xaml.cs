using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace CsvApp1120
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Data> recordsIn;

        string fileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";

            var baseFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            var extension = System.IO.Path.GetExtension(fileName);

            saveFileDialog.FileName = $"{baseFileName}-変換済み{extension}";

            if (saveFileDialog.ShowDialog() == true) {
                using (var csvWriter = new CsvWriter(new StreamWriter(saveFileDialog.FileName))) {
                    csvWriter.Configuration.HasHeaderRecord = true;
                    csvWriter.Configuration.RegisterClassMap<Data2ClassMap>();
                    csvWriter.WriteRecords(recordsIn);
                }
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();

            if (openfileDialog.ShowDialog() != true) {
                return;
            }

            List<string> result = new List<string>();

            fileName = openfileDialog.FileName;

            using (var fileReader = new StreamReader(openfileDialog.FileName, Encoding.GetEncoding("SHIFT_JIS")))
            using (var csv = new CsvReader(fileReader)) {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<Data1ClassMap>();
                recordsIn = csv.GetRecords<Data>().ToList();
            }

            using (var fileReader = new StreamReader(openfileDialog.FileName, Encoding.GetEncoding("SHIFT_JIS")))
            using (var csv = new CsvReader(fileReader)) {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<DataDispClassMap>();
                dataGrid1.ItemsSource = csv.GetRecords<DataDisp>().ToList();
            }
        }
    }

    class Data1ClassMap : ClassMap<Data>
    {
        public Data1ClassMap()
        {
            Map(x => x.Name).Index(0).Name("名前");
            Map(x => x.Address).Index(1).Name("住所");
            Map(x => x.Num).Index(2).Name("番号");
        }
    }

    class Data2ClassMap : ClassMap<Data>
    {
        public Data2ClassMap()
        {
            Map(x => x.Num2).Index(0).Name("番号2").ConvertUsing(m => {
                return $"{m.Num * 10}";
            }
            );
            Map(x => x.Name).Index(1).Name("名前");
            Map(x => x.Address).Index(2).Name("住所");
        }
    }

    class DataDispClassMap : ClassMap<DataDisp>
    {
        public DataDispClassMap()
        {
            Map(x => x.Name).Index(0).Name("名前");
            Map(x => x.Num).Index(2).Name("番号");
        }
    }

    class Data
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Num { get; set; }
        public int Num2 { get; set; }
    }

    class DataDisp
    {
        public int Num { get; set; }
        public string Name { get; set; }
    }

    enum Address
    {
        東京,
        大阪,
        その他
    }
}
