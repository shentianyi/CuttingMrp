﻿using CuttingMrpLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static string dbs = @"Data Source=Charlot-PC\MSSQLSERVER20082;Initial Catalog=CuttingMrp;Persist Security Info=True;User ID=sa;Password=123456@";
        static void Main(string[] args)
        {
            //Console.WriteLine(DateTime.Now.ToLongTimeString());
            //Calculator c = new Calculator(dbs);
            //int i = 999999999;
            //c.MakeBackflush();

            //Console.WriteLine(DateTime.Now.ToLongTimeString());
            //Console.WriteLine(DateTime.Now.ToString("HH:mm"));
            //string s = @"\\XINER-PC\cuttingMrp";
            //List<string> files = FileUtility.GetAllFilesFromDirectory(s);

            //foreach (string file in files) {
            //    Console.WriteLine(file);

            //}
            ////File.Copy(@"\\192.168.1.63\brilliantech\Cutting\1.xls", "c:\\122.xls",true);
            ////  Console.WriteLine(Path.GetFileNameWithoutExtension(@"\\192.168.1.63\brilliantech\Cutting\1.xls"));
            //Console.WriteLine(Path.GetFileNameWithoutExtension(@"\\ -PC\cuttingMrp"));

            Console.WriteLine(DateTime.Now);
             FileDataHandler h = new FileDataHandler();
            //  h.ImportForceStock(@"C:\cz\MrpDashSvc\Processing\2016-06-25\10-38_66_3853d45d-d6e8-4669-a71d-c3f5ba9f62dd.csv", dbs);
            //DataContext dc = new DataContext(dbs);
            //List<int> ostocks= dc.Context.GetTable<Stock>().Take(10).Select(s=>s.id).ToList();
            //List<Stock> stocks = dc.Context.GetTable<Stock>().Where(s => ostocks.Contains(s.id)).ToList();
            //foreach (Stock s in stocks) {

            //    s.quantity += 1000;
            //}
            //dc.SaveAll();
            string file = @"C:\cz\MrpDashSvc\Processing\2016-06-25\10-38_66_3853d45d-d6e8-4669-a71d-c3f5ba9f62dd.csv";
          string p=  Path.GetFullPath(file );
            Console.WriteLine(Path.GetDirectoryName(file));
            Console.WriteLine(DateTime.Now);
            Console.Read();

        }
    }
}