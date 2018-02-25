using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var workbook = new XLWorkbook(@"D:\DATEN\Programmierung\GitHub\Mrv.Regatta.PreisDesMinisterpraesidenten\code\Mrv.Regatta.PreisDesMinisterpraesidenten\bin\x86\Debug\results.xlsx");

            if (workbook.Worksheets.Count != 1)
            {
                throw new Exception("Excel-Datei muss genau ein Arbeitsblatt enthalten!");
            }

            var worksheet = workbook.Worksheets.Single();


            // worksheet.Row(5).InsertRowsAbove(1);

            worksheet.Column(6).InsertColumnsAfter(2);

            workbook.Save();

        }
    }
}
