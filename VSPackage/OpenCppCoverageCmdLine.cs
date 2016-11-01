// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2016 OpenCppCoverage
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using OpenCppCoverage.VSPackage.Settings;
using System.Collections.Generic;
using System.Linq;

namespace OpenCppCoverage.VSPackage
{
    class OpenCppCoverageCmdLine
    {
        //---------------------------------------------------------------------
        public static readonly string SourcesFlag = "--sources";
        public static readonly string ModulesFlag = "--modules";
        public static readonly string WorkingDirFlag = "--working_dir";
        public static readonly string ExcludedSourcesFlag = "--excluded_sources";
        public static readonly string ExcludedModulesFlag = "--excluded_modules";
        public static readonly string UnifiedDiffFlag = "--unified_diff";
        public static readonly string UnifiedDiffSeparator = "?";
        public static readonly string InputCoverageFlag = "--input_coverage";
        public static readonly string ExportTypeFlag = "--export_type";
        public static readonly string ExportTypeSeparator = ":";
        public static readonly string CoverChildrenFlag = "--cover_children";
        public static readonly string NoAggregateByFileFlag = "--no_aggregate_by_file";
        public static readonly string ConfigFileFlag = "--config_file";
        public static readonly string QuietFlag = "--quiet";
        public static readonly string VerboseFlag = "--verbose";
        public static readonly string ContinueAfterCppExceptionFlag = "--continue_after_cpp_exception";
        public static readonly string PluginFlag = "--plugin";

        //---------------------------------------------------------------------
        public static string Build(MainSettings settings, string lineSeparator = " ")
        {
            var builder = new CommandLineBuilder();

            AppendFilterSettings(builder, settings.FilterSettings);
            AppendImportExportSettings(builder, settings.ImportExportSettings);
            AppendMiscellaneousSettings(builder, settings.MiscellaneousSettings);

            // Should be last settings for program to run.
            AppendBasicSettings(builder, settings.BasicSettings);

            return builder.GetCommandLine(lineSeparator);
        }

        //---------------------------------------------------------------------
        static void AppendBasicSettings(
            CommandLineBuilder builder,
            BasicSettings settings)
        {
            AppendArgumentCollection(builder, SourcesFlag, settings.SourcePaths);
            AppendArgumentCollection(builder, ModulesFlag, settings.ModulePaths);

            if (!string.IsNullOrWhiteSpace(settings.WorkingDirectory))
                builder.AppendArgument(WorkingDirFlag, settings.WorkingDirectory);

            builder.Append(" " + PluginFlag + " ");
            builder.AppendArgument("--", settings.ProgramToRun);
            builder.Append(settings.Arguments);
        }

        //---------------------------------------------------------------------
        static void AppendFilterSettings(
            CommandLineBuilder builder,
            FilterSettings settings)
        {
            AppendArgumentCollection(builder, SourcesFlag, settings.AdditionalSourcePaths);
            AppendArgumentCollection(builder, ModulesFlag, settings.AdditionalModulePaths);
            AppendArgumentCollection(builder, ExcludedSourcesFlag, settings.ExcludedSourcePaths);
            AppendArgumentCollection(builder, ExcludedModulesFlag, settings.ExcludedModulePaths);

            foreach (var unifiedDiff in settings.UnifiedDiffs)
            {                
                var argumentValue = unifiedDiff.UnifiedDiffPath;
                if (!string.IsNullOrWhiteSpace(argumentValue))
                {
                    if (!string.IsNullOrWhiteSpace(unifiedDiff.OptionalRootFolder))
                        argumentValue = argumentValue + UnifiedDiffSeparator + unifiedDiff.OptionalRootFolder;
                    builder.AppendArgument(UnifiedDiffFlag, argumentValue);
                }
            }
        }

        //---------------------------------------------------------------------
        static void AppendImportExportSettings(
            CommandLineBuilder builder,
            ImportExportSettings settings)
        {
            AppendArgumentCollection(builder, InputCoverageFlag, settings.InputCoverages);

            foreach (var export in settings.Exports)
            {
                var path = export.Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var type = export.Type.ToString().ToLowerInvariant();
                    builder.AppendArgument(ExportTypeFlag,
                         type + ExportTypeSeparator + path);
                }
            }

            if (settings.CoverChildrenProcesses)
                builder.AppendArgument(CoverChildrenFlag, null);
            if (!settings.AggregateByFile)
                builder.AppendArgument(NoAggregateByFileFlag, null);
        }

        //---------------------------------------------------------------------
        static void AppendMiscellaneousSettings(
            CommandLineBuilder builder,
            MiscellaneousSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.OptionalConfigFile))
                builder.AppendArgument(ConfigFileFlag, settings.OptionalConfigFile);

            switch (settings.LogTypeValue)
            {
                case MiscellaneousSettings.LogType.Quiet:
                    builder.AppendArgument(QuietFlag, null);
                    break;
                case MiscellaneousSettings.LogType.Verbose:
                    builder.AppendArgument(VerboseFlag, null);
                    break;
                case MiscellaneousSettings.LogType.Normal: break;
            }

            if (settings.ContinueAfterCppExceptions)
                builder.AppendArgument(ContinueAfterCppExceptionFlag, null);
        }

        //---------------------------------------------------------------------
        static void AppendArgumentCollection(
            CommandLineBuilder builder,
            string argumentName, 
            IEnumerable<string> values)
        {
            builder.AppendArgumentCollection(argumentName,
                values.Where(v => !string.IsNullOrWhiteSpace(v)));
        }
    }
}
