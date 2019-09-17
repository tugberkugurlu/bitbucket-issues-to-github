bitbucket-issues-to-github
==========================

A simple tool written as a C# console application to migrate your issues from a Bitbucket (private or public) to GitHub (private or public) repository.

Entering the command with no options displays usage help. Command line options are used to specifiy details of the source (bitbucket) and destination (Github) repositories, and migration options, as follows:

Option            | Description
------------------|------------------------------------------------------------------
-p, --bbPass      | Required. Bitbucket login password
-r, --bbRepo      | Required. Bitbucket repository name
-o, --bbOwner     | Required. Bitbucket repository owner/organization name
-u, --bbUser      | Required. Bitbucket login user name
-P, --ghPass      | Required. GitHub login password
-R, --ghRepo      | Required. GitHub repository name
-O, --ghOwner     | Required. GitHub repository owner/organization name
-U, --ghUser      | Required. GitHub login user name
-c, --LabelColor  | (Default: <span style="background-color:#fef2c0">fef2c0</span>) Color assigned to the migration label
-l, --Label       | (Default: bitbucket-migrated) Label applied to migrated issues
--ComponentColor  | (Default: <span style="background-color:#d4c5f9">d4c5f9</span>) Color assigned to migrated Component labels

Note: if in doubt about any of the repository option values, open the repository in a web browser and look at the repository "slug" in the URL. For example, for the following URL:
https://github.com/tugberkugurlu/bitbucket-issues-to-github
The owner/organization name is: tugberkugurlu
The repository name is: bitbucket-issues-to-github

Issues imported into Github are tagged with a migration label. The default text is `bitbucket-migrated`. You may override the text and color of the migration label using the `--Label` and `--LabelColor` options.

Issues in Bitbucket can be tagged with a _Component_, but there is no matching concept in Github. Instead, the issue imported into Github will be tagged with a label, named for the Bitbucket component. You my override the color of component labels using the `--ComponentColor` option.
