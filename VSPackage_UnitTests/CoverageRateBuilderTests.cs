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
using OpenCppCoverage.VSPackage.CoverageData;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using System.Collections.Generic;
using System.Linq;

using ProtoBuff = OpenCppCoverage.VSPackage.CoverageData.ProtoBuff;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class CoverageRateBuilderTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void CoverageRateResults()
        {
            var file1 = CreateFileCoverage("file1", true, false, true);
            var file2 = CreateFileCoverage("file2", true, true);
            var file3 = CreateFileCoverage("file3", true, false, false);
            string coverageName = "coverageName";
            int exitCode = 42;

            var builder = new CoverageRateBuilder();
            var coverageRateResult = builder.Build(
                CreateCoverageResult(coverageName, exitCode, 
                    CreateModuleCoverage("module1", file1, file2),
                    CreateModuleCoverage("module2", file3)));

            Assert.AreEqual(coverageName, coverageRateResult.Name);
            Assert.AreEqual(exitCode, coverageRateResult.ExitCode);

            AssertCoverage(coverageRateResult, 5, 8);
            var module1 = AssertChildCoverage(coverageRateResult, 0, 4, 5);
            AssertChildCoverage(module1, 0, 2, 3); // File 1
            AssertChildCoverage(module1, 1, 2, 2); // File 2

            var module2 = AssertChildCoverage(coverageRateResult, 1, 1, 3);
            AssertChildCoverage(module2, 0, 1, 3); // File 3
        }

        //---------------------------------------------------------------------
        static ProtoBuff.FileCoverage CreateFileCoverage(
            string path,
            params bool[] hasBeenExecutedCollection)
        {
            var lineCoverages = new List<ProtoBuff.LineCoverage>();

            for (int i = 0; i < hasBeenExecutedCollection.Length; ++i)
            {
                lineCoverages.Add(
                    ProtoBuff.LineCoverage.CreateBuilder()
                        .SetLineNumber((uint)i)
                        .SetHasBeenExecuted(hasBeenExecutedCollection[i])
                        .Build());
            }

            return ProtoBuff.FileCoverage.CreateBuilder()
                .SetPath(path).AddRangeLines(lineCoverages).Build();
        }

        //---------------------------------------------------------------------
        static ProtoBuff.ModuleCoverage CreateModuleCoverage(
            string path,
            params ProtoBuff.FileCoverage[] fileCoverages)
        {
            return ProtoBuff.ModuleCoverage.CreateBuilder()
                .SetPath(path).AddRangeFiles(fileCoverages).Build();
        }

        //---------------------------------------------------------------------
        static CoverageResult CreateCoverageResult(
            string name,
            int exitCode,
            params ProtoBuff.ModuleCoverage[] moduleCoverages)
        {
            var coverageData = ProtoBuff.CoverageData.CreateBuilder()
                .SetName(name)
                .SetExitCode(exitCode)
                .SetModuleCount(0).Build();

            return new CoverageResult(coverageData, moduleCoverages);
        }

        //---------------------------------------------------------------------
        static void AssertCoverage(
            BaseCoverage coverage,
            int expectedCoverLineCount,
            int expectedTotalLineCount)
        {
            Assert.AreEqual(expectedCoverLineCount, coverage.CoverLineCount);
            Assert.AreEqual(expectedTotalLineCount, coverage.TotalLineCount);
        }

        //---------------------------------------------------------------------
        static T AssertChildCoverage<T>(
            HierarchicalCoverage<T> coverage,
            int childIndex,
            int expectedCoverLineCount,
            int expectedTotalLineCount) where T : BaseCoverage
        {
            var child = coverage.Children.ToList()[childIndex];

            AssertCoverage(child, expectedCoverLineCount, expectedTotalLineCount);
            return child;
        }
    }
}   