using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace DeadCodeFinder
{
    class MethodFinder
    {
        public int unusedMethods = 0;
        public int unusedClasses = 0;
        public string outputFileLocation = "";
        public string outputFileName = "DeadCode_Scan.txt";
        public StreamWriter sw;
        public string gatingStatus;
        public string projectPath = "";
        public int classThreshold;
        public int methodThreshold;
        public string[] excludeList;
        public MethodFinder(string sourcePath)
        {
            this.projectPath = sourcePath;
        }
        public void componentMethodList()
        {

            Dictionary<string, Dictionary<string, int>> componentClasses = new Dictionary<string, Dictionary<string, int>>();

            string[] componentClassPath = Directory.GetFiles(this.projectPath, "*.component.ts", SearchOption.AllDirectories);
            foreach (var excludeItem in this.excludeList)
            {
                componentClassPath = componentClassPath.Where(item => !item.Contains(excludeItem)).ToArray();
            }
            foreach (var comp in componentClassPath)
            {
                componentClasses.Add(comp, this.getMethodList(comp));
            }
            Console.WriteLine("Scanning in Progress .......................");
            foreach (var item in componentClasses)
            {
                foreach (var method in item.Value)
                {
                    if (method.Value < 2)
                    {
                        unusedMethods++;
                        this.sw.WriteLine("{0} in {1} has no reference.", method.Key, item.Key);
                    }
                }
            }
        }
        private Dictionary<string, int> getMethodList(string filePath)
        {
            Dictionary<string, int> methodList = new Dictionary<string, int>();
            Dictionary<string, int> tempMethodList = new Dictionary<string, int>();
            string[] textLine = File.ReadAllLines(filePath);
            string typescriptText = File.ReadAllText(filePath);
            string htmlText = "";
            if (File.Exists(filePath.Replace(".ts", ".html")))
            {
                htmlText = File.ReadAllText(filePath.Replace(".ts", ".html"));
            }
            int index = 0;
            foreach (var lines in textLine)
            {
                index++;
                if (lines.Contains("() {"))
                {
                    string[] methodLine = lines.Split(' ');
                    string name = methodLine[methodLine.Length - 2];
                    string methodName = name.Trim().TrimStart().TrimEnd();
                    if (textLine[index - 2].Contains("@HostListener"))
                    {
                        methodList.Add(methodName, 2);
                        tempMethodList.Add(methodName, 2);
                    }
                    else if (!methodName.StartsWith("ng") && !methodName.Contains("function ()") && !methodName.Contains("constructor()") && !methodName.Contains("//") && !methodList.ContainsKey(methodName) && !methodName.Contains("get ") && !methodName.Contains("set "))
                    {
                        methodList.Add(methodName, 0);
                        tempMethodList.Add(methodName, 0);
                    }
                }
            }
            foreach (var method in tempMethodList)
            {
                MatchCollection classReferenceCount = Regex.Matches(typescriptText, method.Key);
                MatchCollection htmlReferenceCount = Regex.Matches(htmlText, method.Key);
                methodList[method.Key] = methodList[method.Key] + classReferenceCount.Count + htmlReferenceCount.Count;
            }
            return methodList;
        }

        public void serviceMethodList()
        {
            Dictionary<string, Dictionary<string, int>> serviceClasses = new Dictionary<string, Dictionary<string, int>>();
            string[] serviceClassPath = Directory.GetFiles(this.projectPath, "*.service.ts", SearchOption.AllDirectories);
            List<string> methods = new List<string>();
            List<string> references = new List<string>();
            foreach (var excludeItem in this.excludeList)
            {
                serviceClassPath = serviceClassPath.Where(item => !item.Contains(excludeItem)).ToArray();
            }
            foreach (var service in serviceClassPath)
            {
                methods = this.getServiceMethodList(service);
                references = this.findReferenceClasses(service);
                string serviceClassName = this.extractClassName(service);
                foreach (var methodItem in methods)
                {
                    Dictionary<string, int> methodReference = new Dictionary<string, int>();
                    methodReference.Add(methodItem, 0);
                    if (!serviceClasses.ContainsKey(service))
                    {
                        serviceClasses.Add(service, methodReference);
                    }
                    foreach (var referItem in references)
                    {
                        string localServiceReferenceName = "";
                        if (File.Exists(referItem))
                        {
                            string fileText = File.ReadAllText(referItem);
                            string[] fileLines = File.ReadAllLines(referItem);
                            if (service.Equals(referItem) && fileText.Contains("this." + methodItem + "("))
                            {
                                methodReference[methodItem] = methodReference[methodItem] + Regex.Matches(fileText, ("this." + methodItem + @"\(")).Count;
                            }
                            else if (!service.Equals(referItem) && (referItem.EndsWith(".ts") || referItem.EndsWith(".html")))
                            {
                                foreach (var lines in fileLines)
                                {
                                    if (lines.Contains(": " + serviceClassName))
                                    {
                                        localServiceReferenceName = this.extractReferenceVariable(lines, serviceClassName);
                                    }
                                }
                                if (fileText.Contains(localServiceReferenceName + "." + methodItem + "("))
                                {
                                    methodReference[methodItem] = methodReference[methodItem] + Regex.Matches(fileText, ("this." + localServiceReferenceName + "." + methodItem + @"\(")).Count;
                                }
                            }
                        }
                    }
                    if (methodReference[methodItem] < 1)
                    {
                        unusedMethods++;
                        this.sw.WriteLine("{0} in {1} has no reference.", methodItem, service);
                    }
                }
            }
            this.sw.WriteLine("Total Unused method found in scan: {0}", unusedMethods);
        }

        public void unusedClassList()
        {
            string[] expectArry = Directory.GetFiles(this.projectPath, "*d.ts",
                                         SearchOption.AllDirectories);
            string[] specexpectArry = Directory.GetFiles(this.projectPath, "*.spec.ts",
                                         SearchOption.AllDirectories);
            string[] filePaths = Directory.GetFiles(this.projectPath, "*.ts",
                                         SearchOption.AllDirectories);
            IEnumerable<string> excludeItems = expectArry.ToList().Concat(specexpectArry.ToList());
            IEnumerable<string> listofItems = filePaths.ToList().Except(excludeItems);
            IEnumerable<string> scanFiles = filePaths.ToList().Except(expectArry);
            Dictionary<string, int> duplicateCode = new Dictionary<string, int>();
            Dictionary<string, int> tempList = new Dictionary<string, int>();
            Dictionary<string, int> specList = new Dictionary<string, int>();
            Dictionary<string, int> tempSepcList = new Dictionary<string, int>();
            foreach (var item in listofItems)
            {
                string[] textLine = File.ReadAllLines(item);
                foreach (var lines in textLine)
                {
                    if (lines.Contains("export class"))
                    {
                        string fileName = lines.Substring(13).Split(' ')[0];
                        if (!duplicateCode.ContainsKey(fileName))
                        {
                            duplicateCode.Add(fileName, 0);
                            tempList.Add(fileName, 0);
                        }
                    }
                }
            }
            foreach (var item in scanFiles)
            {
                string text = System.IO.File.ReadAllText(item);
                foreach (var element in duplicateCode)
                {
                    if (text.Contains(element.Key))
                    {
                        MatchCollection referenceCount = Regex.Matches(text, element.Key + " ");
                        MatchCollection declartionCount = Regex.Matches(text, element.Key + ",");
                        MatchCollection usedCount = Regex.Matches(text, element.Key + @"\)");
                        MatchCollection usedCount1 = Regex.Matches(text, element.Key + @"\(");
                        MatchCollection usedCount2 = Regex.Matches(text, element.Key + ";");
                        MatchCollection usedCount3 = Regex.Matches(text, element.Key + "]");
                        MatchCollection usedCount4 = Regex.Matches(text, element.Key + ":");
                        MatchCollection usedCount5 = Regex.Matches(text, element.Key + @"\<");
                        MatchCollection usedCount6 = Regex.Matches(text, element.Key + @"\>");
                        MatchCollection usedCount7 = Regex.Matches(text, element.Key + @"\[");
                        MatchCollection usedCount8 = Regex.Matches(text, element.Key + @"\]");
                        MatchCollection usedCount9 = Regex.Matches(text, element.Key + @"\n\r");
                        MatchCollection usedCount10 = Regex.Matches(text, element.Key + @"\s");

                        tempList[element.Key] = tempList[element.Key] + referenceCount.Count + declartionCount.Count + usedCount.Count + usedCount1.Count + usedCount2.Count + usedCount3.Count + usedCount4.Count +
                            usedCount5.Count + usedCount6.Count + usedCount7.Count + usedCount8.Count + usedCount9.Count + usedCount10.Count;

                    }
                }

            }
            foreach (var element in tempList)
            {
                if (element.Key.Contains("Mock") && element.Value < 2)
                {
                    this.sw.WriteLine("Mock : {0} has no reference.", element.Key);
                    this.unusedClasses++;
                }
                else if (element.Key.Contains("Emitter") && element.Value < 2)
                {
                    this.sw.WriteLine("Emitter : {0} has no reference.", element.Key);
                    this.unusedClasses++;
                }
                else if (element.Value < 3 && !element.Key.EndsWith("Test") && !element.Key.Contains("Mock") && !element.Key.Contains("Emitter"))
                {
                    this.sw.WriteLine("Component : {0} has no reference.", element.Key);
                    this.unusedClasses++;
                }
            }

            foreach (var item in specexpectArry)
            {
                string[] textLine = File.ReadAllLines(item);
                foreach (var lines in textLine)
                {
                    if (lines.Contains("export class"))
                    {
                        string fileName = lines.Substring(13).Split(' ')[0];
                        if (!specList.ContainsKey(fileName))
                        {
                            specList.Add(fileName, 0);
                            tempSepcList.Add(fileName, 0);
                        }
                    }
                }
            }

            foreach (var item in specexpectArry)
            {
                string text = System.IO.File.ReadAllText(item);
                foreach (var element in specList)
                {
                    if (text.Contains(element.Key))
                    {
                        MatchCollection referenceCount = Regex.Matches(text, element.Key + " ");
                        MatchCollection declartionCount = Regex.Matches(text, element.Key + ",");
                        MatchCollection usedCount = Regex.Matches(text, element.Key + @"\)");
                        MatchCollection usedCount1 = Regex.Matches(text, element.Key + @"\(");
                        MatchCollection usedCount2 = Regex.Matches(text, element.Key + ";");
                        MatchCollection usedCount3 = Regex.Matches(text, element.Key + "]");
                        MatchCollection usedCount4 = Regex.Matches(text, element.Key + ":");
                        MatchCollection usedCount5 = Regex.Matches(text, element.Key + @"\<");
                        MatchCollection usedCount6 = Regex.Matches(text, element.Key + @"\>");
                        MatchCollection usedCount7 = Regex.Matches(text, element.Key + @"\[");
                        MatchCollection usedCount8 = Regex.Matches(text, element.Key + @"\]");
                        MatchCollection usedCount9 = Regex.Matches(text, element.Key + @"\n\r");

                        tempSepcList[element.Key] = tempSepcList[element.Key] + referenceCount.Count + declartionCount.Count + usedCount.Count + usedCount1.Count + usedCount2.Count + usedCount3.Count + usedCount4.Count +
                            usedCount5.Count + usedCount6.Count + usedCount7.Count + usedCount8.Count + usedCount9.Count;

                    }
                }

            }
            foreach (var element in tempSepcList)
            {
                if (element.Key.Contains("Mock") && element.Value < 2)
                {
                    this.sw.WriteLine("Mock : {0} has no reference.", element.Key);
                    this.unusedClasses++;
                }
                else if (element.Key.Contains("Emitter") && element.Value < 2)
                {
                    this.sw.WriteLine("Emitter : {0} has no reference.", element.Key);
                    this.unusedClasses++;
                }
                else if (element.Value < 2 && !element.Key.EndsWith("Test") && !element.Key.Contains("Mock") && !element.Key.Contains("Emitter"))
                {
                    this.sw.WriteLine("Component : {0} has no reference.", element.Key);
                    this.unusedClasses++;
                }
            }
            this.sw.WriteLine("Total Unused classes found in scan: {0}", this.unusedClasses);
        }
        private List<string> findReferenceClasses(string filePath)
        {
            string[] textLine = File.ReadAllLines(filePath);
            string serviceName;
            string importServiceName = "";
            string[] serviceFilePaths1 = Directory.GetFiles(this.projectPath, "*.component.ts",
                                         SearchOption.AllDirectories);
            string[] serviceFilePaths2 = Directory.GetFiles(this.projectPath, "*dialog.ts",
                                         SearchOption.AllDirectories);
            string[] serviceFilePaths3 = Directory.GetFiles(this.projectPath, "*factory.ts",
                                      SearchOption.AllDirectories);
            string[] serviceListFilePaths = Directory.GetFiles(this.projectPath, "*.service.ts",
                                        SearchOption.AllDirectories);
            IEnumerable<string> completeReferenceList = serviceFilePaths1.Concat(serviceFilePaths2).Concat(serviceListFilePaths);
            List<string> referenceList = new List<string>();
            int count = 0;
            foreach (var line in textLine)
            {
                count++;
                if (line.Contains("export class") && (textLine[count - 3].Contains("@Injectable()") || textLine[count - 2].Contains("@Injectable()")))
                {
                    serviceName = line.Substring(13).Split(' ')[0];
                    importServiceName = "import { " + serviceName;
                    break;
                }
            }
            referenceList.Add(filePath);
            foreach (var file in completeReferenceList)
            {
                string[] referenceFileLines = File.ReadAllLines(file);
                foreach (var referLine in referenceFileLines)
                {
                    if (referLine.Contains(importServiceName))
                    {
                        if (file.Contains("component.ts") || file.Contains("dialog.ts"))
                        {
                            referenceList.Add(file);
                            referenceList.Add(file.Replace(".ts", ".html"));
                        }
                        else
                        {
                            referenceList.Add(file);
                        }
                        break;
                    }
                }
            }
            return referenceList;
        }
        private List<string> getServiceMethodList(string filePath)
        {
            string[] textLine = File.ReadAllLines(filePath);
            List<string> listOfMethods = new List<string>();
            int index = 0;
            foreach (var lines in textLine)
            {
                Regex _regex1 = new Regex(@"$*[a-z0-9]\(*\)\s\{");
                Regex _regex2 = new Regex(@"$*[a-z0-9]\(*\)\:");
                if ((_regex2.IsMatch(lines) || _regex1.IsMatch(lines)) && !lines.Contains("if ") && !lines.Contains("else ") && !lines.Contains("//") && lines.Contains("(") && !lines.Contains("constructor(") && !lines.Contains("emit(") && !lines.Contains("for") && !lines.Contains("switch") && !lines.Contains("function") && !lines.Contains("while") && !lines.Contains("get ") && !lines.Contains("set") && !lines.Contains("catch"))
                {
                    string[] methodNameline = lines.Split('(')[0].TrimStart().TrimEnd().Replace("public ", "").Replace("private ", "").Replace("async ", "").Split(' ');
                    string methodName = "";
                    if (methodNameline.Length > 1)
                    {
                        methodName = methodNameline[methodNameline.Length - 1];
                    }
                    else
                    {
                        methodName = methodNameline[0];
                    }
                    if (!listOfMethods.Contains(methodName))
                    {
                        listOfMethods.Add(methodName);
                        index++;
                    }
                }
            }
            return listOfMethods;
        }
        private string extractClassName(string filePath)
        {
            string[] fileLines = File.ReadAllLines(filePath);
            string className = "";
            int index = 0;
            foreach (var lines in fileLines)
            {
                if (lines.Contains("export class") && !lines.Contains("extends Subject<any>") && (fileLines[index - 2].Contains("@Injectable()") || fileLines[index - 1].Contains("@Injectable()")))
                {
                    className = lines.Substring(13).Split(' ')[0];
                    break;
                }
                index++;
            }
            return className;
        }
        private string extractReferenceVariable(string line, string serviceName)
        {
            Regex regex1 = new Regex(@"private *:\s" + serviceName);
            string referenceItem = (": " + serviceName);
            string[] extractedValue = Regex.Split(line, referenceItem)[0].Split(' ');
            string referenceServiceName = extractedValue[extractedValue.Length - 1];
            return referenceServiceName;
        }


        private void GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            this.outputFileLocation = new FileInfo(location.AbsolutePath).Directory.FullName;
        }
        private void CreateFile()
        {
            string fileName = this.outputFileLocation + @"/" + this.outputFileName;
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }
        }
        private void GetFileStream()
        {

            if (File.Exists(this.outputFileLocation + @"/" + this.outputFileName))
            {
                this.sw = new StreamWriter(this.outputFileLocation + @"/" + this.outputFileName);
                this.sw.AutoFlush = true;
            }
        }

        public void GetDeadCodeConfig()
        {
            this.GetExecutingDirectoryName();
            Console.WriteLine("path {0}", this.outputFileLocation);
            this.CreateFile();
            this.GetFileStream();
            StreamReader sr;
            string configContent = "";
            if (File.Exists(this.outputFileLocation + @"/DeadCode_Config.json"))
            {
                sr = new StreamReader(this.outputFileLocation + @"/DeadCode_Config.json");
                configContent = sr.ReadToEnd();
            }
            JObject configObject = JObject.Parse(configContent);
            // this.projectPath = configObject["Project_Path"].ToString();
            this.classThreshold = (int)configObject["Class_Threshold"];
            this.methodThreshold = (int)configObject["Method_Threshold"];
            var excludeLists = configObject["Files_Exlude_List"].ToString();
            this.excludeList = excludeLists.Split(',');
        }
        public void GetGateState()
        {
            if (this.unusedMethods < this.methodThreshold && this.unusedClasses < this.classThreshold)
            {
                this.gatingStatus = "Passed";
            }
            else
            {
                this.gatingStatus = "Failed";
            }
            this.sw.WriteLine("Dead Code Final State : {0}", this.gatingStatus);
        }
    }
}
