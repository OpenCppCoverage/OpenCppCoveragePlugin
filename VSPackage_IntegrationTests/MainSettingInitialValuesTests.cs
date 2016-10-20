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
    [TestClass()]
    public class MainSettingInitialValuesTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void EmptySolution()
        {
            var solutionService = TestHelpers.GetService<IVsSolution>();
            solutionService.CloseSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave, null, 0);
            var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

            Assert.AreEqual(BasicSettingController.None,
                controller.BasicSettingController.CurrentConfiguration);
            Assert.AreEqual(BasicSettingController.None,
                controller.BasicSettingController.CurrentProject);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void NotCppStartupProject()
        {
            TestHelpers.OpenSolution(TestHelpers.CSharpConsoleApplication);
            var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

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
            TestHelpers.OpenSolution(TestHelpers.CppConsoleApplication, TestHelpers.CppConsoleApplication2);
            var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

            Assert.AreEqual("Debug|Win32",
                controller.BasicSettingController.CurrentConfiguration);
            Assert.AreEqual(TestHelpers.CppConsoleApplication,
                controller.BasicSettingController.CurrentProject);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void ProjectInFolder()
        {
            TestHelpers.OpenSolution(TestHelpers.ConsoleApplicationInFolder);
            var controller = TestHelpers.ExecuteOpenCppCoverageCommand();

            Assert.AreEqual("Debug|Win32",
                controller.BasicSettingController.CurrentConfiguration);
            Assert.AreEqual(TestHelpers.ConsoleApplicationInFolder,
                controller.BasicSettingController.CurrentProject);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void StartUpProjectSettings()
        {
            TestHelpers.OpenSolution(TestHelpers.CppConsoleApplication2);
            using (var debugSettings = SolutionConfigurationHelpers.GetCurrentDebugSettings(TestHelpers.CppConsoleApplication2))
            {
                var settings = debugSettings.Value;
                settings.Command = "Command";
                settings.CommandArguments = "Arguments";
                settings.WorkingDirectory = ".";

                var controller = TestHelpers.ExecuteOpenCppCoverageCommand();
                var basicSettings = controller.BasicSettingController;
                Assert.AreEqual(settings.Command, basicSettings.ProgramToRun);
                Assert.AreEqual(settings.CommandArguments, basicSettings.Arguments);
                Assert.AreEqual(settings.WorkingDirectory, basicSettings.OptionalWorkingDirectory);

                var expectedProjects = new List<string> {
                    TestHelpers.CppConsoleApplication,
                    TestHelpers.CppConsoleApplication2,
                    TestHelpers.CppConsoleApplicationDll,
                    TestHelpers.ConsoleApplicationInFolder };
                CollectionAssert.AreEquivalent(
                    expectedProjects,
                    basicSettings.SelectableProjects.Select(p => p.FullName).ToList());
            }
        }
    }
}
