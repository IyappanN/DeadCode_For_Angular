using System;
using System.Collections.Generic;
using System.Text;
using DeadCodeFinderForAngular;

namespace DeadCodeFinderForAngularTests
{
    public class MethodFinderTestsBase
    {
        private string ProjectPath => "C:/TFSSource/dev_1/Frontend/DCDx/app";
        private string OutputFileName => "DeadCode_Scan.txt";
        private int ClassThreshold => 10;
        private int MethodThreshold => 250;

        public MethodFinder MethodFinder { get; }
        public MethodFinderTestsBase()
        {
            MethodFinder = new MethodFinder("C:/TFSSource/dev_1/Frontend/DCDx/app");
            MethodFinder.outputFileName = OutputFileName;
            MethodFinder.projectPath = ProjectPath;
            MethodFinder.classThreshold = ClassThreshold;
            MethodFinder.methodThreshold = MethodThreshold;
        }

        public void GetConfiguration()
        {

            MethodFinder.outputFileName = OutputFileName;
            MethodFinder.projectPath = ProjectPath;
            MethodFinder.classThreshold = ClassThreshold;
            MethodFinder.methodThreshold = MethodThreshold;
        }

    }

    public class Configurations
    {
        public string ProjectPath { set; get; }
        public string OutputFileName { set; get; }
        public int ClassThreshold { set; get; }
        public int MethodThreshold { set; get; }

    }
}
