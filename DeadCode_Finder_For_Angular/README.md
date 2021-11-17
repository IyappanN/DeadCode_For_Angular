# DeadCodeFinderForAngular

Application which scan the angular project and create the DeadCode_Scan.txt, shows the list of unused method in components and services and unused classes from code.

DeadCode_Config.json is used as the input for the appliaction.

Sample conetnt for the JOSN file.

  "Project_Path": "C:/TFSSource/dev_1/Frontend/DCDx/app",
  "Skip_Scan": "video.component.ts,create-assessment.component.ts,rename-assessment.component.ts,create-user-information.component.ts,edit-user-information.component.ts",
  "Class_Threshold": "10",
  "Method_Threshold": "250"
  
  Based on the threshold set in config file it will retun the status as passed or Failed.
 
DeadCode_Config.json should be kept on the folder where the application file is availble.

