using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using Octokit;
using Octokit.Extensions;

if (args.Length != 2)
{
    System.Console.WriteLine("ERROR: Expected 2 arguments: [REPO/OWNER] [ISSUE_NUMBER]");
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

const string GitHubToken = "GITHUB_TOKEN";

var ghToken = Environment.GetEnvironmentVariable(GitHubToken);
if (string.IsNullOrEmpty(ghToken))
{
    System.Console.WriteLine("Couldn't find GitHub Token environment variable");
    return 1;
}

var ghClient = new ResilientGitHubClientFactory().Create(productHeaderValue: GitHubProduct.Header, credentials: new(ghToken));

var issue = await ghClient.Issue.Get(owner: owner, name: repo, number: issueNumber);

var issueCreatedBy = issue.User?.Login;
Console.WriteLine($"Found issue: {issue.Title}");
Console.WriteLine($"Created by: {issueCreatedBy}");

var labelsToApply = labelData["labels"]
    .Where(labelSet => labelSet.Value.Contains(issueCreatedBy, StringComparer.OrdinalIgnoreCase))
    .Select(labelSet => labelSet.Key)
    .ToArray();

if (labelsToApply.Length == 0)
{
    Console.WriteLine($"No labels to apply!");
}
else
{
    Console.WriteLine($"Applying labels: {string.Join(", ", labelsToApply)}");

    await ghClient.Issue.Labels.AddToIssue(owner: owner, name: repo, number: issueNumber, labels: labelsToApply);

    Console.WriteLine($"Finished applying labels");
}

return 0;

class GitHubProduct
{
    private static readonly string _name = "IssueOpenerLabeler";
    private static readonly string _version = "1.0";

    public static ProductHeaderValue Header { get; } = new(_name, _version);
}
