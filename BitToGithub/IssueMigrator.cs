// This file is part of the BitToGithub project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
//
// File: IssueMigrator.cs  Last modified: 2019-09-06@06:53 by Tim Long

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace BitToGithub
{
    internal class IssueMigrator
    {
        private readonly List<string> closedIssueStates = new List<string> {"closed", "resolved", "invalid", "wontfix"};
        private readonly MigrationOptions options;
        private readonly string bitbucketRepoIssuesUrl;

        public IssueMigrator(MigrationOptions options)
        {
            this.options = options;
            bitbucketRepoIssuesUrl =
                $"https://api.bitbucket.org/2.0/repositories/{options.bbRepoOwnerName}/{options.bbRepoName}/issues";
        }

        public async Task<ExitCode> Run()
        {
            var github = new GitHubClient(new ProductHeaderValue("bitbucket-issue-migrator"));
            github.Credentials = new Credentials(options.githubUsername, options.githubPassword);
            var repo = await github.Repository.Get(options.githubRepoOwnerName, options.githubRepoName);

            using (HttpClient client = new HttpClient())
            {
                string authHeaderRaw = $"{options.bbUsername}:{options.bbPassword}";
                AuthenticationHeaderValue authHeader =
                    new AuthenticationHeaderValue("Basic", authHeaderRaw.Base64Encoded());
                client.DefaultRequestHeaders.Authorization = authHeader;

                // create the migration label.
                try
                {
                    var label = await github.Issue.Labels.Get(
                        options.githubRepoOwnerName,
                        options.githubRepoName,
                        options.LabelName);
                }
                catch (NotFoundException ex)
                {
                    // assume that the label is not there. create.
                    var label = await github.Issue.Labels.Create(
                        options.githubRepoOwnerName,
                        options.githubRepoName,
                        new NewLabel(options.LabelName, options.LabelColor));
                }

                int startIndex = 1;
                var requestUrl = bitbucketRepoIssuesUrl;
                while (!string.IsNullOrWhiteSpace(requestUrl))
                {
                    var response = await client.GetAsync($"{bitbucketRepoIssuesUrl}?start={startIndex}");
                    response.EnsureSuccessStatusCode();
                    var repoIssues = await response.Content.ReadAsAsync<BitBucket.IssueRoot>();
                    startIndex += repoIssues.size;

                    if (repoIssues.values.Length < 1)
                        break;
                    var bodyBuilder = new StringBuilder();
                    foreach (var bitbucketIssue in repoIssues.values)
                    {
                        Console.WriteLine("found issue {0}", bitbucketIssue.id);
                        var newIssue = new NewIssue(bitbucketIssue.title);
                        newIssue.Labels.Add(options.LabelName);
                        bodyBuilder.Clear()
                            .AppendLine(bitbucketIssue.content.raw)
                            .AppendLine()
                            .Append($"> Originally created at {bitbucketIssue.created_on} ")
                            .Append($"by {bitbucketIssue.reporter.display_name} ")
                            .Append($"as a {bitbucketIssue.kind} with severity {bitbucketIssue.priority}.");
                        newIssue.Body = bodyBuilder.ToString();
                        if (!string.IsNullOrWhiteSpace(bitbucketIssue.component))
                        {
                            // Bitbucket Component --> GitHub Label
                            try
                            {
                                var label = await github.Issue.Labels.Get(
                                    options.githubRepoOwnerName,
                                    options.githubRepoName,
                                    bitbucketIssue.component);
                            }
                            catch (Exception ex)
                            {
                                // assume that the label is not there. create.
                                Console.WriteLine("Creating label {0}", bitbucketIssue.component);
                                var label = await github.Issue.Labels.Create(
                                    options.githubRepoOwnerName,
                                    options.githubRepoName,
                                    new NewLabel(bitbucketIssue.component, options.ComponentColor));
                            }
                            newIssue.Labels.Add(bitbucketIssue.component);
                        }

                        // push the issue to GitHub
                        Console.WriteLine($"Creating issue {bitbucketIssue.id} on GitHub");
                        var githubIssue = await github.Issue.Create(
                            options.githubRepoOwnerName,
                            options.githubRepoName,
                            newIssue);
                        Console.WriteLine($"Created issue {githubIssue.Number} on github.");

                        // Map issue states by closing the issue on GitHub
                        if (closedIssueStates.Contains(bitbucketIssue.state, StringComparer.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine($"Closing resolved issue {githubIssue.Number} on github.");
                            var updatedIssue = await github.Issue.Update(
                                options.githubRepoOwnerName,
                                options.githubRepoName, githubIssue.Number,
                                new IssueUpdate
                                    {
                                        State = ItemState.Closed
                                    });
                        }
                        await MigrateComments(client, bitbucketIssue, github, githubIssue);
                    }
                    requestUrl = repoIssues.next;
                }
            }
            return ExitCode.SignalSuccess;
        }

        private async Task MigrateComments(HttpClient bitbuckeClient, BitBucket.Issue issue, GitHubClient github,
            Issue githubIssue)
        {
            var pageUrl = $"{bitbucketRepoIssuesUrl}/{issue.id}/comments";
            while (!string.IsNullOrWhiteSpace(pageUrl))
            {
                var commentsResponse = await bitbuckeClient.GetAsync($"{bitbucketRepoIssuesUrl}/{issue.id}/comments");
                if (!commentsResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine(
                        $"Failed to fetch comments for Bitbucket issue {issue.id} - migration incomplete");
                    return;
                }
                var commentRoot = await commentsResponse.Content.ReadAsAsync<BitBucket.CommentRoot>();
                var commentCount = commentRoot.values.Length;
                Console.WriteLine($"Found {commentCount} comments associated with issue {issue.id}");
                if (commentCount < 1)
                    return;
                foreach (var comment in commentRoot.values)
                {
                    Console.WriteLine($"Adding comment {comment.id} for github issue {githubIssue.Number}.");
                    var createdComment = await github.Issue.Comment.Create(
                        options.githubRepoOwnerName,
                        options.githubRepoName,
                        githubIssue.Number, comment.content.raw ?? "Edited description");
                }
                pageUrl = commentRoot.next;
            }
        }
    }
}