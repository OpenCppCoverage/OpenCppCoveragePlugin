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
using OpenCppCoverage.VSPackage.Settings.UI;
using System.Collections.Generic;
using System.Linq;

namespace VSPackage_IntegrationTests
{
    [TestClass]
    public class MainSettingInitialValuesTests: TestHelpers
    {
        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void EmptySolution()
        {
            var solutionService = GetService<IVsSolution>();
            solutionService.CloseSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave, null, 0);
            var controller = ExecuteOpenCppCoverageCommand();

            Assert.AreEqual(BasicSettingController.None,
                controller.BasicSettingController.CurrentConfiguration);
            Assert.AreEqual(BasicSettingController.None,
                controller.BasicSettingController.CurrentProject);
            Assert.IsTrue(controller.BasicSettingController.IsOptimizedBuildCheckBoxEnabled);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void NotCppStartupProject()
        {
            OpenSolution(CSharpConsoleApplication);
            var controller = ExecuteOpenCppCoverageCommand();

            Assert.AreEqual(BasicSettingController.None, 
                controller.BasicSettingController.CurrentConfiguration);
            Assert.AreEqual(BasicSettingController.None, 
                controller.BasicSettingController.CurrentProject);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void SeveralStartupProjects()
        {
            OpenSolution(CppConsoleApplication, CppConsoleApplication2);
            var controller = ExecuteOpenCppCoverageCommand();

            Assert.AreEqual("Debug|Win32",
                controller.BasicSettingController.CurrentConfiguration);
            Assert.AreEqual(CppConsoleApplication,
                controller.BasicSettingController.CurrentProject);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void ProjectInFolder()
        {
            OpenSolution(ConsoleApplicationInFolder);
            var controller = ExecuteOpenCppCoverageCommand();

            Assert.AreEqual("Debug|Win32",
                controller.BasicSettingController.CurrentConfiguration);
            Assert.AreEqual(ConsoleApplicationInFolder,
                controller.BasicSettingController.CurrentProject);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void StartUpProjectSettings()
        {
            OpenSolution(CppConsoleApplication2);
            using (var debugSettings = SolutionConfigurationHelpers.GetCurrentDebugSettings(CppConsoleApplication2))
            {
                var settings = debugSettings.Value;
                settings.Command = "Command";
                settings.CommandArguments = "Arguments";
                settings.WorkingDirectory = ".";

                var controller = ExecuteOpenCppCoverageCommand();
                var basicSettings = controller.BasicSettingController;
                Assert.AreEqual(settings.Command, basicSettings.ProgramToRun);
                Assert.AreEqual(settings.CommandArguments, basicSettings.Arguments);
                Assert.AreEqual(settings.WorkingDirectory, basicSettings.OptionalWorkingDirectory);

                var expectedProjects = new List<string> {
                    CppConsoleApplication,
                    CppConsoleApplication2,
                    CppConsoleApplicationDll,
                    ConsoleApplicationInFolder };
                CollectionAssert.AreEquivalent(
                    expectedProjects,
                    basicSettings.SelectableProjects.Select(p => p.FullName).ToList());
            }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void OptimizedBuild()
        {
            OpenSolution(CppConsoleApplicationDll, ConfigurationName.Debug);
            var controller = ExecuteOpenCppCoverageCommand();
            Assert.IsFalse(controller.BasicSettingController.OptimizedBuild);
            Assert.IsFalse(controller.BasicSettingController.IsOptimizedBuildCheckBoxEnabled);

            OpenSolution(CppConsoleApplicationDll, ConfigurationName.Release);
            controller = ExecuteOpenCppCoverageCommand();
            Assert.IsTrue(controller.BasicSettingController.OptimizedBuild);
            Assert.IsFalse(controller.BasicSettingController.IsOptimizedBuildCheckBoxEnabled);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void NoVCCLCompilerTool()
        {
            OpenSolution(ZeroCheck);
            var controller = ExecuteOpenCppCoverageCommand();
            var settings = controller.BasicSettingController;
            Assert.AreEqual(BasicSettingController.None, settings.CurrentConfiguration);
            Assert.AreEqual(BasicSettingController.None, settings.CurrentProject);
        }
    }
}
