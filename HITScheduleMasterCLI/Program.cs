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
