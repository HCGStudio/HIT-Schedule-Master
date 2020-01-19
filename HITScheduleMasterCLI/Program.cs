using System;
using System.Text.RegularExpressions;
using ScheduleTranslator;
using MobileSuit;

namespace HITScheduleMasterCLI
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var dvr = new Driver();
            dvr.LoadXls("D:\\s.xls");
            dvr.Show();
            (new MobileSuitHost(typeof(Driver))).Run();
            var regex = new Regex(@"\d+");
            while (true)
            {
                var col = regex.Matches(Console.ReadLine()??"");
                foreach (var match in col)
                {
                    Console.WriteLine(match);
                }
                
            }
            
        }
        

    }
}
