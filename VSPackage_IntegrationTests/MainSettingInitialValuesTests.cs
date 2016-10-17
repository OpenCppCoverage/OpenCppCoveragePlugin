// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2014 OpenCppCoverage
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

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSPackage_IntegrationTests
{
    [TestClass()]
    public class MainSettingInitialValuesTests
    {
        //---------------------------------------------------------------------
        void RunInUIhread(Action action)
        {
            UIThreadInvoker.Invoke(action);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void EmptySolution()
        {
            RunInUIhread(() =>
            {
                var solutionService = TestHelpers.GetService<IVsSolution>();
                solutionService.CloseSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave, null, 0);
                var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

                Assert.AreEqual(BasicSettingController.None,
                    controller.BasicSettingController.CurrentConfiguration);
                Assert.AreEqual(BasicSettingController.None,
                    controller.BasicSettingController.CurrentProject);
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void NotCppStartupProject()
        {
            RunInUIhread(() =>
            {                
                TestHelpers.OpenSolution(TestHelpers.CSharpConsoleApplication);
                var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

                Assert.AreEqual(BasicSettingController.None, 
                    controller.BasicSettingController.CurrentConfiguration);
                Assert.AreEqual(BasicSettingController.None, 
                    controller.BasicSettingController.CurrentProject);
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void SeveralStartupProjects()
        {
            RunInUIhread(() =>
            {                
                TestHelpers.OpenSolution(TestHelpers.CppConsoleApplication, TestHelpers.CppConsoleApplication2);
                var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

                Assert.AreEqual("Debug|Win32",
                    controller.BasicSettingController.CurrentConfiguration);
                Assert.AreEqual(TestHelpers.CppConsoleApplication,
                    controller.BasicSettingController.CurrentProject);
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void ProjectInFolder()
        {
            RunInUIhread(() =>
            {
                TestHelpers.OpenSolution(TestHelpers.ConsoleApplicationInFolder);
                var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

                Assert.AreEqual("Debug|Win32",
                    controller.BasicSettingController.CurrentConfiguration);
                Assert.AreEqual(TestHelpers.ConsoleApplicationInFolder,
                    controller.BasicSettingController.CurrentProject);
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void DoesNotCompile()
        {
            //TestHelpers.OpenDefaultSolution(TestHelpers.CppConsoleApplication2);
            //Assert.AreEqual("OpenCppCoverage\n\nBuild failed.", TestHelpers.GetOpenCppCoverageMessage());
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void StartUpProjectSettings()
        {
            RunInUIhread(() =>
            {
                var debugSettings = TestHelpers.OpenSolution(TestHelpers.CppConsoleApplication);
                debugSettings.Command = "Command";
                debugSettings.CommandArguments = "Arguments";
                debugSettings.WorkingDirectory = ".";

                var controller = TestHelpers.ExecuteOpenCppCoverageCommand();
                var settings = controller.BasicSettingController;
                Assert.AreEqual(debugSettings.Command, settings.ProgramToRun);
                Assert.AreEqual(debugSettings.CommandArguments, settings.Arguments);
                Assert.AreEqual(debugSettings.WorkingDirectory, settings.OptionalWorkingDirectory);

                var expectedProjects = new List<string> {
                        TestHelpers.CppConsoleApplication,
                        TestHelpers.CppConsoleApplication2,
                        TestHelpers.CppConsoleApplicationDll,
                        TestHelpers.ConsoleApplicationInFolder };
                CollectionAssert.AreEquivalent(
                    expectedProjects,
                    settings.SelectableProjects.Select(p => p.FullName).ToList());
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void InvalidProgramToRun()
        {
            //CheckInvalidSettings(
            //    (debugSettings, v) => debugSettings.Command = v,
            //    debugSettings => debugSettings.Command,
            //    "OpenCppCoverage\n\nDebugging command \"{0}\" does not exist.");
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverageX86()
        {
            //TestHelpers.OpenDefaultSolution(TestHelpers.CppConsoleApplication);
            //CheckCoverage(ConfigurationName.Debug, PlatFormName.Win32);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverageX64()
        {
            
            //CheckCoverage(ConfigurationName.Debug, PlatFormName.x64);
        }
        
        //---------------------------------------------------------------------
        static string GetProjectFolder(string projectName)
        {
            var rootFolder = TestHelpers.GetIntegrationTestsSolutionFolder();

            return Path.GetDirectoryName(Path.Combine(rootFolder, projectName));
        }

        //---------------------------------------------------------------------
        static void CheckOutput(string output, string lineStartsWith, string textToFound)
        {
            using (var reader = new StringReader(output))
            {
                var line = reader.ReadLine();

                while (line != null)
                {
                    if (line.StartsWith(lineStartsWith) && line.Contains(textToFound))
                        return;
                    line = reader.ReadLine();
                }
            }

            Assert.Fail(string.Format("Cannot found {0} with a starting line :{1}",
                textToFound, lineStartsWith));
        }
    }
}
