using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BitToGithub
{
    class Program
    {
        static void Main(string[] args)
        {
            const string bbUsername = "";
            const string bbPassword = "";
            const string bbRepoName = "";
            const string bbRepoOwnerName = "";

            const string githubUsername = "";
            const string githubPassword = "";
            const string githubRepoName = "";
            const string githubRepoOwnerName = "";

            var isAllDefined = new[] { bbUsername, bbPassword, bbRepoName, githubUsername, githubPassword, githubRepoName, githubRepoOwnerName }.All(param => string.IsNullOrEmpty(param) == false);
            if (isAllDefined)
            {
                //string bbRepoIssuesUrl = "https://api.bitbucket.org/1.0/repositories/\{bbUsername}/\{bbRepoName}/issues";
                string bbRepoIssuesUrl = string.Format("https://api.bitbucket.org/1.0/repositories/{0}/{1}/issues", bbRepoOwnerName, bbRepoName);


                var github = new GitHubClient(new Octokit.ProductHeaderValue("MyAmazingApp"));
                github.Credentials = new Credentials(githubUsername, githubPassword);
                var repo = github.Repository.Get(githubRepoOwnerName, githubRepoName).Result;

                using (HttpClient client = new HttpClient())
                {
                    const string authHeaderRaw = bbUsername + ":" + bbPassword;
                    const string migratedLabel = "bitbucket-migrated";
                    AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Basic", EncodeToBase64(authHeaderRaw));
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    // create the migration label.
                    try
                    {
                        github.Issue.Labels.Get(githubRepoOwnerName, githubRepoName, migratedLabel).Wait();
                    }
                    catch (Exception ex)
                    {
                        // assume that the label is not there. create.
                        github.Issue.Labels.Create(githubRepoOwnerName, githubRepoName, new NewLabel(migratedLabel, "fef2c0")).Wait();
                    }

                    int startIndex = 1;
                    while (true)
                    {
                        System.Threading.Thread.Sleep(3000);
                        //var response = client.GetAsync(bbRepoIssuesUrl + "?start=\{startIndex}").Result;
                        var response = client.GetAsync(bbRepoIssuesUrl + "?start=" + startIndex.ToString()).Result;
                        response.EnsureSuccessStatusCode();
                        var result = response.Content.ReadAsAsync<BitBuckect.Rootobject>().Result;
                        startIndex += result.issues.Length;
                        if (result.issues.Length > 0)
                        {
                            foreach (var issue in result.issues)
                            {
                                Console.WriteLine("found issue {0}", issue.local_id);
                                var newIssue = new NewIssue(issue.title);
                                newIssue.Labels.Add(migratedLabel);
                                //newIssue.Body = issue.content +  "\{Environment.NewLine}\{Environment.NewLine}> Originally created at \{issue.utc_created_on} (UTC) by \{issue.reported_by.username} as a(n) \{issue.priority} issue.";
                                newIssue.Body =
                                    issue.content +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    String.Format("> Originally created at {0} (UTC) by {1} as a(n) {2} issue.", issue.utc_created_on, issue.reported_by.username, issue.priority);

                                if (string.IsNullOrWhiteSpace(issue.metadata.component) == false)
                                {
                                    // check label
                                    try
                                    {
                                        github.Issue.Labels.Get(githubRepoOwnerName, githubRepoName, issue.metadata.component).Wait();
                                    }
                                    catch (Exception ex)
                                    {
                                        // assume that the label is not there. create.
                                        Console.WriteLine("Creating label {0}", issue.metadata.component);
                                        github.Issue.Labels.Create(githubRepoOwnerName, githubRepoName, new NewLabel(issue.metadata.component, "d4c5f9")).Wait();
                                    }

                                    newIssue.Labels.Add(issue.metadata.component);
                                }

                                // push the issue to GitHub
                                Console.WriteLine("Creating issue {0}", issue.local_id);
                                var createdIssue = github.Issue.Create(githubRepoOwnerName, githubRepoName, newIssue).Result;
                                Console.WriteLine("Create issue {0} on github.", createdIssue.Number);

                                if (issue.status.Equals("resolved", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Console.WriteLine("Closing issue {0} on github.", createdIssue.Number);
                                    github.Issue.Update(githubRepoOwnerName, githubRepoName, createdIssue.Number, new IssueUpdate
                                    {
                                        State = ItemState.Closed
                                    }).Wait();
                                }

                                if (issue.comment_count > 0)
                                {
                                    var commentsResponse = client.GetAsync(bbRepoIssuesUrl + "/" + issue.local_id + "/comments").Result;

                                    if (commentsResponse.IsSuccessStatusCode)
                                    {
                                        var comments = commentsResponse.Content.ReadAsAsync<IEnumerable<BitBucketComment.Class1>>().Result;
                                        foreach (var comment in comments)
                                        {
                                            Console.WriteLine("Adding comment {0} for github issue {1}.", comment.comment_id, createdIssue.Number);
                                            github.Issue.Comment.Create(githubRepoOwnerName, githubRepoName, createdIssue.Number, comment.content).Wait();
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to fetch comments for issue {0}", issue.local_id);
                                    }
                                }
                                System.Threading.Thread.Sleep(3000);
                            }
                        }
                        else
                        {
                            Console.ReadLine();
                            break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Set all the necessary constant variables.");
            }
        }

        private static string EncodeToBase64(string value)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(toEncodeAsBytes);
        }
    }
}