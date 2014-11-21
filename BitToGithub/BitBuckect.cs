using System;

namespace BitToGithub
{
    public class BitBuckect
    {
        public class Rootobject
        {
            public int count { get; set; }
            public Filter filter { get; set; }
            public object search { get; set; }
            public Issue[] issues { get; set; }
        }

        public class Filter
        {
        }

        public class Issue
        {
            public string status { get; set; }
            public string priority { get; set; }
            public string title { get; set; }
            public Reported_By reported_by { get; set; }
            public string utc_last_updated { get; set; }
            public Responsible responsible { get; set; }
            public DateTime created_on { get; set; }
            public Metadata metadata { get; set; }
            public string content { get; set; }
            public int comment_count { get; set; }
            public int local_id { get; set; }
            public int follower_count { get; set; }
            public string utc_created_on { get; set; }
            public string resource_uri { get; set; }
            public bool is_spam { get; set; }
        }

        public class Reported_By
        {
            public string username { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string display_name { get; set; }
            public bool is_staff { get; set; }
            public string avatar { get; set; }
            public string resource_uri { get; set; }
            public bool is_team { get; set; }
        }

        public class Responsible
        {
            public string username { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string display_name { get; set; }
            public bool is_staff { get; set; }
            public string avatar { get; set; }
            public string resource_uri { get; set; }
            public bool is_team { get; set; }
        }

        public class Metadata
        {
            public string kind { get; set; }
            public string version { get; set; }
            public string component { get; set; }
            public string milestone { get; set; }
        }

    }

    public class BitBucketComment
    {
        public class Rootobject
        {
            public Class1[] Property1 { get; set; }
        }

        public class Class1
        {
            public string content { get; set; }
            public Author_Info author_info { get; set; }
            public int comment_id { get; set; }
            public string utc_updated_on { get; set; }
            public bool convert_markup { get; set; }
            public string utc_created_on { get; set; }
            public bool is_spam { get; set; }
        }

        public class Author_Info
        {
            public string username { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string display_name { get; set; }
            public bool is_staff { get; set; }
            public string avatar { get; set; }
            public string resource_uri { get; set; }
            public bool is_team { get; set; }
        }

    }
}
