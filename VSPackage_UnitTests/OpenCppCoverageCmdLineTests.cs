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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage;
using OpenCppCoverage.VSPackage.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass]
    public class OpenCppCoverageCmdLineTests
    {
        readonly MainSettings mainSettings = new MainSettings();
        OpenCppCoverageCmdLine openCppCoverageCmdLine;
        TemporaryFile configFile;

        //---------------------------------------------------------------------
        [TestInitialize]
        public void Initialize()
        {
            mainSettings.BasicSettings = new BasicSettings
            {
                ModulePaths = new List<string>(),
                SourcePaths = new List<string>(),
                WorkingDirectory = string.Empty,
                Arguments = string.Empty,
                ProgramToRun = string.Empty,
            };
            mainSettings.FilterSettings = new FilterSettings
            {
                AdditionalModulePaths = new List<string>(),
                AdditionalSourcePaths = new List<string>(),
                ExcludedModulePaths = new List<string>(),
                ExcludedSourcePaths = new List<string>(),
                UnifiedDiffs = new List<FilterSettings.UnifiedDiff>()
            };
            mainSettings.ImportExportSettings = new ImportExportSettings
            {
                InputCoverages = new List<string>(),
                Exports = new List<ImportExportSettings.Export>(),
                AggregateByFile = true
            };
            mainSettings.MiscellaneousSettings = new MiscellaneousSettings();
            mainSettings.DisplayProgramOutput = true;

            configFile = new TemporaryFile();
            openCppCoverageCmdLine = new OpenCppCoverageCmdLine(configFile);
        }

        //---------------------------------------------------------------------
        [TestCleanup]
        public void Cleanup()
        {
            configFile.Dispose();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Sources()
        {
            mainSettings.BasicSettings.SourcePaths = new List<string> { "path3", "path4" };
            mainSettings.FilterSettings.AdditionalSourcePaths = new List<string> { "path1", "path2" };

            BuildAndCheckConfig(OpenCppCoverageCmdLine.SourcesFlag, "path1", "path2", "path3", "path4");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Modules()
        {
            mainSettings.BasicSettings.ModulePaths = ToList("mod2");
            mainSettings.FilterSettings.AdditionalModulePaths = ToList("mod1");

            BuildAndCheckConfig(OpenCppCoverageCmdLine.ModulesFlag, "mod1", "mod2");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void WorkingDirFlag()
        {
            mainSettings.BasicSettings.WorkingDirectory = "WorkingDirectory";

            BuildAndCheckConfig(OpenCppCoverageCmdLine.WorkingDirFlag, "WorkingDirectory");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ExcludedModules()
        {
            mainSettings.FilterSettings.ExcludedModulePaths = ToList("module");

            BuildAndCheckConfig(OpenCppCoverageCmdLine.ExcludedModulesFlag, "module");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ExcludedSources()
        {
            mainSettings.FilterSettings.ExcludedSourcePaths = ToList("src");

            BuildAndCheckConfig(OpenCppCoverageCmdLine.ExcludedSourcesFlag, "src");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void UnifiedDiff()
        {
            var unifiedDiffWithRoot = new FilterSettings.UnifiedDiff
            {
                OptionalRootFolder = "root",
                UnifiedDiffPath = "path1"
            };
            var unifiedDiff = new FilterSettings.UnifiedDiff { UnifiedDiffPath = "path2" };

            mainSettings.FilterSettings.UnifiedDiffs =
                    new List<FilterSettings.UnifiedDiff> { unifiedDiffWithRoot, unifiedDiff };

            BuildAndCheckConfig(OpenCppCoverageCmdLine.UnifiedDiffFlag,
                $"path1{OpenCppCoverageCmdLine.UnifiedDiffSeparator}root",
                "path2");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void InputCoverage()
        {
            mainSettings.ImportExportSettings.InputCoverages = ToList("input");

            BuildAndCheckConfig(OpenCppCoverageCmdLine.InputCoverageFlag, "input");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ExportType()
        {
            var exportTypes = Enum.GetValues(typeof(ImportExportSettings.Type))
                                    .Cast<ImportExportSettings.Type>();
            foreach (var exportType in exportTypes)
            {
                var export = new ImportExportSettings.Export
                {
                    Type = exportType,
                    Path = "path"
                };

                mainSettings.ImportExportSettings.Exports = ToList(export);

                BuildAndCheckConfig(OpenCppCoverageCmdLine.ExportTypeFlag,
                    $"{exportType.ToString().ToLowerInvariant()}{OpenCppCoverageCmdLine.ExportTypeSeparator}path");
            }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void CoverChildren()
        {
            mainSettings.ImportExportSettings.CoverChildrenProcesses = true;

            BuildAndCheckConfig(OpenCppCoverageCmdLine.CoverChildrenFlag);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void NoAggregateByFile()
        {
            mainSettings.ImportExportSettings.AggregateByFile = false;

            BuildAndCheckConfig(OpenCppCoverageCmdLine.NoAggregateByFileFlag);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ConfigFileFlag()
        {
            using (var config = new TemporaryFile())
            {
                File.WriteAllText(config.Path, "argument=value1\nargument=value2");
                mainSettings.MiscellaneousSettings.OptionalConfigFile = config.Path;

                BuildAndCheckConfig("argument", "value1", "value2");
            }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Quiet()
        {
            mainSettings.MiscellaneousSettings.LogTypeValue =
                MiscellaneousSettings.LogType.Quiet;

            BuildAndCheckCommandLine(OpenCppCoverageCmdLine.QuietFlag);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Verbose()
        {
            mainSettings.MiscellaneousSettings.LogTypeValue =
                MiscellaneousSettings.LogType.Verbose;

            BuildAndCheckCommandLine(OpenCppCoverageCmdLine.VerboseFlag);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ContinueAfterCppException()
        {
            mainSettings.MiscellaneousSettings.ContinueAfterCppExceptions = true;

            BuildAndCheckConfig(OpenCppCoverageCmdLine.ContinueAfterCppExceptionFlag);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ProgramToRunAndArg()
        {
            mainSettings.BasicSettings.ProgramToRun = "program";
            mainSettings.BasicSettings.Arguments = "arguments";

            var cmdLine = openCppCoverageCmdLine.Build(mainSettings);

            Assert.IsTrue(cmdLine.EndsWith(@"""program"" arguments"));
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void WhiteSpaceAndEmptyBasicSettings()
        {
            var basicSettings = mainSettings.BasicSettings;
            basicSettings.WorkingDirectory = string.Empty;
            basicSettings.ModulePaths = new List<string> { string.Empty };
            basicSettings.SourcePaths = new List<string> { " " };

            BuildAndCheckEmpyConfig();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void WhiteSpaceAndEmptyFilterSettings()
        {
            var filterSettings = mainSettings.FilterSettings;

            filterSettings.AdditionalModulePaths = new List<string> { string.Empty };
            filterSettings.AdditionalSourcePaths = new List<string> { string.Empty };
            filterSettings.ExcludedModulePaths = new List<string> { " " };
            filterSettings.ExcludedSourcePaths = new List<string> { string.Empty };
            filterSettings.UnifiedDiffs = new List<FilterSettings.UnifiedDiff>
                { new FilterSettings.UnifiedDiff() };

            BuildAndCheckEmpyConfig();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void EmptyImportExportSettings()
        {
            var importExportSettings = mainSettings.ImportExportSettings;

            importExportSettings.InputCoverages = new List<string> { string.Empty };
            importExportSettings.Exports = new List<ImportExportSettings.Export>
                { new ImportExportSettings.Export() };

            BuildAndCheckEmpyConfig();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void WhiteSpaceMiscellaneousSettings()
        {
            mainSettings.MiscellaneousSettings.OptionalConfigFile = "   ";

            BuildAndCheckEmpyConfig();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void DisplayProgramOutput()
        {
            mainSettings.DisplayProgramOutput = true;
            BuildAndCheckConfig(OpenCppCoverageCmdLine.PluginFlag);

            mainSettings.DisplayProgramOutput = false;
            var lines = BuildConfig();
            Assert.IsTrue(lines.Count == 0);
        }

        //---------------------------------------------------------------------
        static List<T> ToList<T>(T value)
        {
            return new List<T> { value };
        }

        //---------------------------------------------------------------------
        void BuildAndCheckEmpyConfig()
        {
            var lines = BuildConfig();
            Assert.AreEqual(1, lines.Count);
            Assert.IsTrue(lines.Contains(OpenCppCoverageCmdLine.PluginFlag +
                OpenCppCoverageCmdLine.OptionValueSeparator +
                OpenCppCoverageCmdLine.OptionWithoutValue));
        }

        //---------------------------------------------------------------------
        void BuildAndCheckConfig(string name, params string[] values)
        {
            var lines = BuildConfig();

            if (values.Length == 0)
                values = new[] { OpenCppCoverageCmdLine.OptionWithoutValue };

            foreach (var value in values)
            {
                var expectedLine = $"{name}{OpenCppCoverageCmdLine.OptionValueSeparator}{value}";
                var found = lines.Contains(expectedLine);
                Assert.IsTrue(found,
                    $"Cannot find the line {expectedLine} in\n{string.Join("\n", lines)}");
            }
        }

        //---------------------------------------------------------------------
        HashSet<string> BuildConfig()
        {
            openCppCoverageCmdLine.Build(mainSettings);
            return File.ReadAllLines(configFile.Path).ToHashSet();
        }

        //---------------------------------------------------------------------
        void BuildAndCheckCommandLine(string expectedArguments)
        {
            var cmdLine = openCppCoverageCmdLine.Build(mainSettings);
            Assert.IsTrue(cmdLine.StartsWith(expectedArguments),
                $"{cmdLine}\ndoes not start with\n{expectedArguments}");
        }
    }
}
