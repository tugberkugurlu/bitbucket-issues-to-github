// This file is part of the BitToGithub project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
//
// File: Program.cs  Last modified: 2019-09-06@02:44 by Tim Long

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;

namespace BitToGithub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            PrintBanner();
            var commandLineParser = new Parser(with =>
                {
                    with.CaseSensitive = true;
                    with.IgnoreUnknownArguments = false;
                    with.HelpWriter = Console.Out;
                    with.AutoVersion = true;
                    with.AutoHelp = true;
                });

            var errorMessages = new List<string>();
            MigrationOptions parsedOptions = null;

            try
            {
                var result = commandLineParser.ParseArguments<MigrationOptions>(args);
                result
                    .WithParsed(options => parsedOptions = options)
                    .WithNotParsed(errors => errors.ToList().ForEach(e => errorMessages.Add(e.ToString())));
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
            }

            if (errorMessages.Any())
                ExitCode.SignalInvalidOptions.WithErrors(errorMessages);

            try
            {
                var migrator = new IssueMigrator(parsedOptions);
                var exitCode = await migrator.Run();
                exitCode.WithErrors(Enumerable.Empty<string>());
            }
            catch (AggregateException ae)
            {
                foreach (var innerException in ae.InnerExceptions)
                {
                    errorMessages.Add(innerException.Message);
                }
                ExitCode.SignalMigrationFailed.WithErrors(errorMessages);
            }
            catch (HttpRequestException e)
            {
                errorMessages.Add($"Bitbucket error: {e.Message}");
                ExitCode.SignalMigrationFailed.WithErrors(errorMessages);
            }
            catch (Exception e)
            {
                errorMessages.Add(e.Message);
                ExitCode.SignalMigrationFailed.WithErrors(errorMessages);
            }
        }

        private static void PrintBanner()
        {
            Console.WriteLine("Bitbucket to GitHub Issue Migrator");
        }
    }
}