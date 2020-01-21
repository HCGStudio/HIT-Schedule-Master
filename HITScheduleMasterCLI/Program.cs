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

        }
        

    }
}
