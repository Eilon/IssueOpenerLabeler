using System;
using System.IO;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

if (args.Length != 2)
{
    System.Console.WriteLine("ERROR: Expected 2 arguments: [DIR] [REPO/OWNER]");
    return 1;
}

var dir = args[0];
var repoAndOwner = args[1];

const string ConfigFileName = "issueopenerlabels.json";

var configFileFullPath = Path.Combine(dir, ConfigFileName);

System.Console.WriteLine("Running in repo: " + repoAndOwner);
System.Console.WriteLine("Config data:");
System.Console.WriteLine(File.ReadAllText(configFileFullPath));

return 0;
