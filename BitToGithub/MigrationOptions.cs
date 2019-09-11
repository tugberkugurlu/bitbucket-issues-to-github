// This file is part of the BitToGithub project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
//
// File: MigrationOptions.cs  Last modified: 2019-09-06@00:02 by Tim Long

using CommandLine;

namespace BitToGithub
{
    class MigrationOptions
    {
        [Option('p',"bbPass", Required = true, HelpText = "Bitbucket login password")]
        public string bbPassword { get; set; }
        [Option('r',"bbRepo", Required = true, HelpText = "Bitbucket repository name")]
        public string bbRepoName { get; set; }
        [Option('o', "bbOwner", Required = true, HelpText = "Bitbucket repository owner/organization name")]
        public string bbRepoOwnerName { get; set; }
        [Option('u', "bbUser", Required = true, HelpText = "Bitbucket login user name")]
        public string bbUsername { get; set; }
        [Option('P', "ghPass", Required = true, HelpText = "GitHub login password")]
        public string githubPassword { get; set; }
        [Option('R', "ghRepo", Required = true, HelpText = "GitHub repository name")]
        public string githubRepoName { get; set; }
        [Option('O', "ghOwner", Required = true, HelpText = "GitHub repository owner/organization name")]
        public string githubRepoOwnerName { get; set; }
        [Option('U', "ghUser", Required = true, HelpText = "GitHub login user name")]
        public string githubUsername { get; set; }
        [Option('c',"LabelColor", Default = "fef2c0", HelpText = "Color assigned to the migration label")]
        public string LabelColor { get; set; }
        [Option('l',"Label", Default = "bitbucket-migrated", HelpText = "Label applied to migrated issues")]
        public string LabelName { get; set; }
        [Option( "ComponentColor", Default = "d4c5f9", HelpText = "Color assigned to migrated Component labels")]
        public string ComponentColor { get; set; }
    }
}