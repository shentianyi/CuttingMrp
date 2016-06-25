using CuttingMrpLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static string dbs = @"Data Source=WANGSONG-PC\MSSQLSERVER2008R;Initial Catalog=CuttingMrp;Persist Security Info=True;User ID=sa;Password=123456@";
        static void Main(string[] args)
        {
            //Console.WriteLine(DateTime.Now.ToLongTimeString());
            //Calculator c = new Calculator(dbs);
            //int i = 999999999;
            //c.MakeBackflush();

            //Console.WriteLine(DateTime.Now.ToLongTimeString());
            //Console.WriteLine(DateTime.Now.ToString("HH:mm"));
            string s = @"\\BRILLIANTECH-PC\brilliantech\Cutting";
            //List<string> files = FileUtility.GetAllFilesFromDirectory(s);
            //File.Copy(@"\\192.168.1.63\brilliantech\Cutting\1.xls", "c:\\122.xls",true);
            Console.WriteLine(Path.GetFileNameWithoutExtension(@"\\192.168.1.63\brilliantech\Cutting\1.xls"));
            Console.Read();

        }
    }
}
