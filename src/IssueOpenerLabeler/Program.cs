using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Octokit;
using Octokit.Extensions;

if (args.Length != 2)
{
    System.Console.WriteLine("ERROR: Expected 1 argument: [REPO/OWNER] [ISSUE_NUMBER]");
    return 1;
}

var dir = "/github/workspace";
if (!Directory.Exists(dir))
{
    var currentDir = Directory.GetCurrentDirectory();
    Console.WriteLine($"Directory {dir} doesn't exist, changing to {currentDir}.");
    dir = currentDir;
}
var ownerAndRepo = args[0].Split('/');
var owner = ownerAndRepo[0];
var repo = ownerAndRepo[1];

var issueNumber = int.Parse(args[1]);

const string ConfigFileName = "issueopenerlabels.json";

var configFileFullPath = Path.Combine(dir, ConfigFileName);

Console.WriteLine("Running in repo: " + ownerAndRepo);
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

const string GitHubToken = "GITHUB_TOKEN";

var ghToken = Environment.GetEnvironmentVariable(GitHubToken);
if (string.IsNullOrEmpty(ghToken))
{
    System.Console.WriteLine("Couldn't find GitHub Token environment variable");
    return 1;
}

var ghClient = new ResilientGitHubClientFactory().Create(productHeaderValue: GitHubProduct.Header, credentials: new(ghToken));

//await ghClient.Issue.Labels.AddToIssue(owner: owner, name: repo, number: issueNumber, labels: new[] { "a" });
var issue = await ghClient.Issue.Get(owner: owner, name: repo, number: issueNumber);
Console.WriteLine($"Found issue: {issue.Title}");

return 0;

class GitHubProduct
{
    private static readonly string _name = "IssueOpenerLabeler";
    private static readonly string _version = "1.0";

    public static ProductHeaderValue Header { get; } = new(_name, _version);
}
