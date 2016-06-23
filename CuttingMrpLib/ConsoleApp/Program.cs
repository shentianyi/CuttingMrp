using CuttingMrpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static string dbs = @"Data Source=WANGSONG-PC\MSSQLSERVER2008R;Initial Catalog=CuttingMrp;Persist Security Info=True;User ID=sa;Password=123456@";
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            Calculator c = new Calculator(dbs);
            int i = 999999999;
            c.MakeBackflush();

            Console.WriteLine(DateTime.Now.ToLongTimeString());
            Console.Read();

        }
    }
}
