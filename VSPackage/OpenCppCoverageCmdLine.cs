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
using System.IO;

namespace OpenCppCoverage.VSPackage
{
    class OpenCppCoverageCmdLine: IOpenCppCoverageCmdLine
    {
        readonly TemporaryFile configFile;

        //---------------------------------------------------------------------
        public static readonly string SourcesFlag = "sources";
        public static readonly string ModulesFlag = "modules";
        public static readonly string WorkingDirFlag = "working_dir";
        public static readonly string ExcludedSourcesFlag = "excluded_sources";
        public static readonly string ExcludedModulesFlag = "excluded_modules";
        public static readonly string UnifiedDiffFlag = "unified_diff";
        public static readonly string UnifiedDiffSeparator = "?";
        public static readonly string InputCoverageFlag = "input_coverage";
        public static readonly string ExportTypeFlag = "export_type";
        public static readonly string ExportTypeSeparator = ":";
        public static readonly string CoverChildrenFlag = "cover_children";
        public static readonly string NoAggregateByFileFlag = "no_aggregate_by_file";
        public static readonly string ConfigFileFlag = "--config_file";
        public static readonly string QuietFlag = "--quiet";
        public static readonly string VerboseFlag = "--verbose";
        public static readonly string ContinueAfterCppExceptionFlag = "continue_after_cpp_exception";
        public static readonly string OptimizedBuildFlag = "optimized_build";
        public static readonly string PluginFlag = "plugin";
        public static readonly string OptionValueSeparator = "=";
        public static readonly string OptionWithoutValue = "true";

        //---------------------------------------------------------------------
        public OpenCppCoverageCmdLine(TemporaryFile configFile)
        {
            this.configFile = configFile;
        }

        //---------------------------------------------------------------------
        public string Build(MainSettings settings, string lineSeparator = " ")
        {
            var configPath = this.configFile.Path;
            using (var writer = new StreamWriter(configPath))
            {
                var builder = new CommandLineBuilder();

                AppendFilterSettings(writer, settings.FilterSettings);
                AppendImportExportSettings(writer, settings.ImportExportSettings);
                AppendMiscellaneousSettings(writer, builder, settings.MiscellaneousSettings);

                // Should be last settings for program to run.
                AppendBasicSettings(writer, configPath,  builder, settings.BasicSettings, settings.DisplayProgramOutput);

                return builder.GetCommandLine(lineSeparator);
            }
        }

        //---------------------------------------------------------------------
        static void AppendBasicSettings(
            StreamWriter writer,
            string configPath,
            CommandLineBuilder builder,
            BasicSettings settings,
            bool waitAfterExit)
        {
            AppendArgumentCollection(writer, SourcesFlag, settings.SourcePaths);
            AppendArgumentCollection(writer, ModulesFlag, settings.ModulePaths);

            if (!string.IsNullOrWhiteSpace(settings.WorkingDirectory))
                AppendArgument(writer, WorkingDirFlag, settings.WorkingDirectory);
            if (settings.IsOptimizedBuildEnabled)
                AppendArgument(writer, OptimizedBuildFlag, null);

            if (waitAfterExit)
                AppendArgument(writer, PluginFlag, null);

            builder.AppendArgument(ConfigFileFlag, configPath);
            builder.AppendArgument("--", settings.ProgramToRun);
            builder.Append(settings.Arguments);
        }

        //---------------------------------------------------------------------
        static void AppendFilterSettings(
            StreamWriter writer,
            FilterSettings settings)
        {
            AppendArgumentCollection(writer, SourcesFlag, settings.AdditionalSourcePaths);
            AppendArgumentCollection(writer, ModulesFlag, settings.AdditionalModulePaths);
            AppendArgumentCollection(writer, ExcludedSourcesFlag, settings.ExcludedSourcePaths);
            AppendArgumentCollection(writer, ExcludedModulesFlag, settings.ExcludedModulePaths);

            foreach (var unifiedDiff in settings.UnifiedDiffs)
            {
                var argumentValue = unifiedDiff.UnifiedDiffPath;
                if (!string.IsNullOrWhiteSpace(argumentValue))
                {
                    if (!string.IsNullOrWhiteSpace(unifiedDiff.OptionalRootFolder))
                        argumentValue = argumentValue + UnifiedDiffSeparator + unifiedDiff.OptionalRootFolder;
                    AppendArgument(writer, UnifiedDiffFlag, argumentValue);
                }
            }
        }

        //---------------------------------------------------------------------
        static void AppendImportExportSettings(
            StreamWriter writer,
            ImportExportSettings settings)
        {
            AppendArgumentCollection(writer, InputCoverageFlag, settings.InputCoverages);

            foreach (var export in settings.Exports)
            {
                var path = export.Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var type = export.Type.ToString().ToLowerInvariant();
                    AppendArgument(writer, ExportTypeFlag,
                         type + ExportTypeSeparator + path);
                }
            }

            if (settings.CoverChildrenProcesses)
                AppendArgument(writer, CoverChildrenFlag, null);
            if (!settings.AggregateByFile)
                AppendArgument(writer, NoAggregateByFileFlag, null);
        }

        //---------------------------------------------------------------------
        void AppendMiscellaneousSettings(
            StreamWriter writer,
            CommandLineBuilder builder,
            MiscellaneousSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.OptionalConfigFile))
            {
                try
                {
                    var lines = File.ReadAllLines(settings.OptionalConfigFile);
                    foreach (var line in lines)
                        writer.WriteLine(line);
                } 
                catch (FileNotFoundException)
                {
                    string message = $"Cannot find the config file defined in Miscellanous tab: {settings.OptionalConfigFile}";
                    throw new VSPackageException(message);
                }
            }

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
                AppendArgument(writer, ContinueAfterCppExceptionFlag, null);
        }

        //---------------------------------------------------------------------
        static void AppendArgumentCollection(
            StreamWriter writer,
            string argumentName,
            IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    AppendArgument(writer, argumentName, value);
            }
        }

        //---------------------------------------------------------------------
        static void AppendArgument(
            StreamWriter writer,
            string argumentName,
            string value)
        {
            if (value == null)
                value = OptionWithoutValue;
            writer.WriteLine($"{argumentName}{OptionValueSeparator}{value}");
        }
    }
}
