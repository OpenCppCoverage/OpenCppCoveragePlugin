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
    public class RunCoverageTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void DoesNotCompile()
        {
            TestHelpers.OpenSolution(TestHelpers.CppConsoleApplication2);
            TestHelpers.RunCoverage();
            var outputMessage = TestHelpers.GetOpenCppCoverageMessage();
            Assert.AreEqual("OpenCppCoverage\n\n" + CoverageRunner.BuilderFailedMsg, 
                outputMessage);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void InvalidProgramToRun()
        {
            TestHelpers.OpenSolution(TestHelpers.CppConsoleApplication);
            using (var debugSettings = 
                SolutionConfigurationHelpers.GetCurrentDebugSettings(
                            TestHelpers.CppConsoleApplication))
            {
                var settings = debugSettings.Value;
                settings.Command = "Invalid";
                TestHelpers.RunCoverage();
                var outputMessage = TestHelpers.GetOpenCppCoverageMessage();
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
            TestHelpers.OpenSolution(
                TestHelpers.CppConsoleApplication, 
                ConfigurationName.Debug, 
                PlatFormName.Win32);
            var coverageTreeController = TestHelpers.RunCoverageAndWait();
            var root = coverageTreeController.Root;

            Assert.IsTrue(root.CoveredLineCount > 0);
            Assert.IsTrue(root.UncoveredLineCount > 0);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverageX64()
        {
            TestHelpers.OpenSolution(
                TestHelpers.CppConsoleApplication,
                ConfigurationName.Debug,
                PlatFormName.x64);
            var coverageTreeController = TestHelpers.RunCoverageAndWait();
            var root = coverageTreeController.Root;

            Assert.IsTrue(root.CoveredLineCount > 0);
            Assert.IsTrue(root.UncoveredLineCount > 0);
        }
    }
}
