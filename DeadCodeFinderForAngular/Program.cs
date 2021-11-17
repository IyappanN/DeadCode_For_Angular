using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeadCodeFinderForAngular
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcefolder = args[0];
            MethodFinder obj = new MethodFinder(sourcefolder);
            obj.GetDeadCodeConfig();
            obj.componentMethodList();
            obj.serviceMethodList();
            obj.unusedClassList();
            obj.GetGateState();
            Console.WriteLine("Unused Methods count {0}", obj.unusedMethods);
            Console.WriteLine("Unused Classes count {0}", obj.unusedClasses);
            Console.WriteLine("Dead Code Final State : {0}", obj.gatingStatus);
            obj.unusedMethods = 0;
            obj.unusedClasses = 0;
            // System.Console.ReadKey();
        }
    }

}
