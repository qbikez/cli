﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.Tools.MSBuild;
using Microsoft.DotNet.Tools.Restore3;

namespace Microsoft.DotNet.Tools.Publish3
{
    public partial class Publish3Command
    {
        public string ProjectPath { get; set; }
        public string Framework { get; set; }
        public string Runtime { get; set; }
        public string OutputPath { get; set; }
        public string Configuration { get; set; }
        public string VersionSuffix { get; set; }

        public List<string> ExtraMSBuildArguments { get; set; }

        private Publish3Command()
        {
        }

        public int Execute()
        {
            int restoreResult = EnsureRestored();
            if (restoreResult != 0)
            {
                throw new GracefulException("Restore failed.  Please fix the errors and try publishing again.");
            }

            List<string> msbuildArgs = new List<string>();

            if (!string.IsNullOrEmpty(ProjectPath))
            {
                msbuildArgs.Add(ProjectPath);
            }

            msbuildArgs.Add("/t:Publish");

            if (!string.IsNullOrEmpty(Framework))
            {
                msbuildArgs.Add($"/p:TargetFramework={Framework}");
            }

            if (!string.IsNullOrEmpty(Runtime))
            {
                msbuildArgs.Add($"/p:RuntimeIdentifier={Runtime}");
            }

            if (!string.IsNullOrEmpty(OutputPath))
            {
                msbuildArgs.Add($"/p:PublishDir={OutputPath}");
            }

            if (!string.IsNullOrEmpty(Configuration))
            {
                msbuildArgs.Add($"/p:Configuration={Configuration}");
            }

            if (!string.IsNullOrEmpty(VersionSuffix))
            {
                msbuildArgs.Add($"/p:VersionSuffix={VersionSuffix}");
            }

            msbuildArgs.AddRange(ExtraMSBuildArguments);

            return new MSBuildForwardingApp(msbuildArgs).Execute();
        }

        /// <summary>
        /// Ensures that the project has been restored for the specified runtime.
        /// </summary>
        private int EnsureRestored()
        {
            int result = 0;

            if (!string.IsNullOrEmpty(Runtime))
            {
                result = Restore3Command.Run(new[] { $"/p:RuntimeIdentifiers={Runtime}" });
            }

            return result;
        }
    }
}
