using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

if (args.Length != 1)
{
    System.Console.WriteLine("ERROR: Expected 1 argument: [REPO/OWNER]");
    return 1;
}

var dir = "/github/workspace";
if (!Directory.Exists(dir))
{
    var currentDir = Directory.GetCurrentDirectory();
    Console.WriteLine($"Directory {dir} doesn't exist, changing to {currentDir}.");
    dir = currentDir;
}
var repoAndOwner = args[0];

const string ConfigFileName = "issueopenerlabels.json";

var configFileFullPath = Path.Combine(dir, ConfigFileName);

Console.WriteLine("Running in repo: " + repoAndOwner);
Console.WriteLine($"Config data loading from: {configFileFullPath}");

var configFileJsonContents = File.ReadAllText(configFileFullPath);
var labelData = JsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string[]>>>(configFileJsonContents, new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip, });
foreach (var x in labelData!)
{
    System.Console.WriteLine(x.Key + ":");
    foreach (var y in x.Value!)
    {
        System.Console.WriteLine("\t" + y.Value + ": " + string.Join(", ", y.Value));
    }
}

return 0;
