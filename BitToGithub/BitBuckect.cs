// This file is part of the BitToGithub project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
//
// File: BitBuckect.cs  Last modified: 2019-09-06@06:47 by Tim Long

using System;

namespace BitToGithub
{
    public class BitBucket
    {
        public class Comment
        {
            public Content content { get; set; }

            public DateTime created_on { get; set; }

            public int id { get; set; }

            public Issue issue { get; set; }

            public Links links { get; set; }

            public string type { get; set; }

            public DateTime? updated_on { get; set; }

            public Person user { get; set; }
        }

        public class CommentRoot
        {
            public int page { get; set; }

            public int pagelen { get; set; }

            public int size { get; set; }

            public Comment[] values { get; set; }
            public string next { get; set; }
            public string previous { get; set; }

            }

        public class Content
        {
            public string html { get; set; }

            public string markup { get; set; }

            public string raw { get; set; }

            public string type { get; set; }
        }

        public class Issue
        {
            public Person assignee { get; set; }

            public string component { get; set; }

            public Content content { get; set; }

            public DateTime? created_on { get; set; }

            public DateTime? edited_on { get; set; }

            public int id { get; set; }

            public string kind { get; set; }

            public Links links { get; set; }

            public string milestone { get; set; }

            public string priority { get; set; }

            public Person reporter { get; set; }

            public Repository repository { get; set; }

            public string state { get; set; }

            public string title { get; set; }

            public string type { get; set; }

            public DateTime? updated_on { get; set; }

            public string version { get; set; }

            public int votes { get; set; }

            public int watches { get; set; }
        }

        public class IssueRoot
        {
            public int page { get; set; }

            public int pagelen { get; set; }

            public int size { get; set; }

            public Issue[] values { get; set; }

            public string next { get; set; }
            public string previous { get; set; }
        }

        public class Links
        {
        }

        public class Person
        {
            public string account_id { get; set; }

            public string display_name { get; set; }

            public Links links { get; set; }

            public string nickname { get; set; }

            public string type { get; set; }

            public string uuid { get; set; }
        }

        public class Repository
        {
        }
    }
}