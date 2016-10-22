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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage;

namespace VSPackage_IntegrationTests
{
    [TestClass()]
    public class RunCoverageTests: TestHelpers
    {
        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void DoesNotCompile()
        {
            OpenSolution(CppConsoleApplication2);
            RunCoverage();
            var outputMessage = GetOpenCppCoverageMessage();
            Assert.AreEqual("OpenCppCoverage\n\n" + CoverageRunner.BuilderFailedMsg, 
                outputMessage);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void InvalidProgramToRun()
        {
            OpenSolution(CppConsoleApplication);
            using (var debugSettings = 
                SolutionConfigurationHelpers.GetCurrentDebugSettings(
                            CppConsoleApplication))
            {
                var settings = debugSettings.Value;
                settings.Command = "Invalid";
                RunCoverage();
                var outputMessage = GetOpenCppCoverageMessage();
                var expectedMessage = "OpenCppCoverage\n\n" +
                    string.Format(CoverageRunner.InvalidProgramToRunMsg, settings.Command);

                Assert.AreEqual(expectedMessage, outputMessage);
            }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverageX86()
        {
            OpenSolution(
                CppConsoleApplication, 
                ConfigurationName.Debug, 
                PlatFormName.Win32);
            var coverageTreeController = RunCoverageAndWait();
            var root = coverageTreeController.Root;

            Assert.IsTrue(root.CoveredLineCount > 0);
            Assert.IsTrue(root.UncoveredLineCount > 0);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverageX64()
        {
            OpenSolution(
                CppConsoleApplication,
                ConfigurationName.Debug,
                PlatFormName.x64);
            var coverageTreeController = RunCoverageAndWait();
            var root = coverageTreeController.Root;

            Assert.IsTrue(root.CoveredLineCount > 0);
            Assert.IsTrue(root.UncoveredLineCount > 0);
        }
    }
}
