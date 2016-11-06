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
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass]
    public class OpenCppCoverageCmdLineTests
    {        
        readonly MainSettings mainSettings = new MainSettings();

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
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Sources()
        {
            mainSettings.BasicSettings.SourcePaths = new List<string> { "path3", "path4" };
            mainSettings.FilterSettings.AdditionalSourcePaths = new List<string> { "path1", "path2" };

            Check(string.Format(@"{0} ""path1"" {0} ""path2"" {0} ""path3"" {0} ""path4""",
                OpenCppCoverageCmdLine.SourcesFlag), mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Modules()
        {
            mainSettings.BasicSettings.ModulePaths = ToList("mod2");
            mainSettings.FilterSettings.AdditionalModulePaths = ToList("mod1");

            Check(string.Format(@"{0} ""mod1"" {0} ""mod2""",
                OpenCppCoverageCmdLine.ModulesFlag), mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void WorkingDirFlag()
        {
            mainSettings.BasicSettings.WorkingDirectory = "WorkingDirectory";

            Check(string.Format(@"{0} ""WorkingDirectory""",
                OpenCppCoverageCmdLine.WorkingDirFlag), mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ExcludedModules()
        {
            mainSettings.FilterSettings.ExcludedModulePaths = ToList("module");

            Check(string.Format(@"{0} ""module""",
                OpenCppCoverageCmdLine.ExcludedModulesFlag), mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ExcludedSources()
        {
            mainSettings.FilterSettings.ExcludedSourcePaths = ToList("src");

            Check(string.Format(@"{0} ""src""",
                OpenCppCoverageCmdLine.ExcludedSourcesFlag), mainSettings);
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
            
            Check(string.Format(@"{0} ""path1{1}root"" {0} ""path2""",
                OpenCppCoverageCmdLine.UnifiedDiffFlag,
                OpenCppCoverageCmdLine.UnifiedDiffSeparator), mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void InputCoverage()
        {
            mainSettings.ImportExportSettings.InputCoverages = ToList("input");

            Check(string.Format(@"{0} ""input""",
                OpenCppCoverageCmdLine.InputCoverageFlag), mainSettings);
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

                Check(string.Format(@"{0} ""{1}{2}path""",
                    OpenCppCoverageCmdLine.ExportTypeFlag,
                    exportType.ToString().ToLowerInvariant(),
                    OpenCppCoverageCmdLine.ExportTypeSeparator), mainSettings);
            }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void CoverChildren()
        {
            mainSettings.ImportExportSettings.CoverChildrenProcesses = true;

            Check(OpenCppCoverageCmdLine.CoverChildrenFlag, mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void NoAggregateByFile()
        {
            mainSettings.ImportExportSettings.AggregateByFile = false;

            Check(OpenCppCoverageCmdLine.NoAggregateByFileFlag, mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ConfigFileFlag()
        {
            mainSettings.MiscellaneousSettings.OptionalConfigFile = "file";

            Check(string.Format(@"{0} ""file""",
                OpenCppCoverageCmdLine.ConfigFileFlag), mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Quiet()
        {
            mainSettings.MiscellaneousSettings.LogTypeValue =
                MiscellaneousSettings.LogType.Quiet;

            Check(OpenCppCoverageCmdLine.QuietFlag, mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Verbose()
        {
            mainSettings.MiscellaneousSettings.LogTypeValue = 
                MiscellaneousSettings.LogType.Verbose;

            Check(OpenCppCoverageCmdLine.VerboseFlag, mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ContinueAfterCppException()
        {
            mainSettings.MiscellaneousSettings.ContinueAfterCppExceptions = true;

            Check(OpenCppCoverageCmdLine.ContinueAfterCppExceptionFlag, mainSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ProgramToRunAndArg()
        {
            mainSettings.BasicSettings.ProgramToRun = "program";
            mainSettings.BasicSettings.Arguments = "arguments";

            var cmdLine = OpenCppCoverageCmdLine.Build(mainSettings);

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

            var cmdLine = OpenCppCoverageCmdLine.Build(mainSettings);
            Assert.IsTrue(cmdLine.TrimStart().StartsWith(OpenCppCoverageCmdLine.PluginFlag));
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

            var cmdLine = OpenCppCoverageCmdLine.Build(mainSettings);
            Assert.IsTrue(cmdLine.TrimStart().StartsWith(OpenCppCoverageCmdLine.PluginFlag));
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void EmptyImportExportSettings()
        {
            var importExportSettings = mainSettings.ImportExportSettings;

            importExportSettings.InputCoverages = new List<string> { string.Empty };
            importExportSettings.Exports = new List<ImportExportSettings.Export>
                { new ImportExportSettings.Export() };

            var cmdLine = OpenCppCoverageCmdLine.Build(mainSettings);
            Assert.IsTrue(cmdLine.TrimStart().StartsWith(OpenCppCoverageCmdLine.PluginFlag));
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void WhiteSpaceMiscellaneousSettings()
        {
            mainSettings.MiscellaneousSettings.OptionalConfigFile = "   ";

            var cmdLine = OpenCppCoverageCmdLine.Build(mainSettings);
            Assert.IsTrue(cmdLine.TrimStart().StartsWith(OpenCppCoverageCmdLine.PluginFlag));
        }

        //---------------------------------------------------------------------
        static List<T> ToList<T>(T value)
        {
            return new List<T> { value };
        }

        //---------------------------------------------------------------------
        void Check(string expectedArguments, MainSettings mainSettings)
        {
            var cmdLine = OpenCppCoverageCmdLine.Build(mainSettings);
            Assert.IsTrue(cmdLine.StartsWith(expectedArguments),
                string.Format("{0}\ndoes not start with\n{1}", cmdLine, expectedArguments));
        }
    }
}
