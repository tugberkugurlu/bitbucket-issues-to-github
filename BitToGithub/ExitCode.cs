// This file is part of the BitToGithub project
// 
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// 
// File: ExitCode.cs  Last modified: 2019-09-06@02:40 by Tim Long
namespace BitToGithub
{
    internal enum ExitCode
    {
        SignalSuccess = 0,
        SignalMigrationFailed = -2,
        SignalInvalidOptions = -1,
    }
}