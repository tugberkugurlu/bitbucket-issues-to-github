// This file is part of the BitToGithub project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
//
// File: Helpers.cs  Last modified: 2019-09-06@03:52 by Tim Long

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitToGithub
{
    internal static class Helpers
    {
        public static string Base64Encoded(this string value)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static void WithErrors(this ExitCode code, IEnumerable<string> errors = null)
        {
            var errorsToReport = errors.ToList();
            switch (code)
            {
                case ExitCode.SignalSuccess:
                    Console.WriteLine("Completed successfully");
                    break;
                default:
                    if (errorsToReport.Any())
                    {
                        Console.WriteLine($"Exiting with code {(int) code} ({code}) due to the following errors:");
                        foreach (var error in errors)
                        {
                            Console.WriteLine(error);
                        }
                    }
                    else
                        Console.WriteLine($"Exiting with code {(int) code} ({code})");
                    break;
            }
#if DEBUG
            Console.WriteLine("Press <ENTER> to close the program...");
            Console.ReadLine();
#endif //DEBUG
            Environment.Exit((int) code);
        }
    }
}