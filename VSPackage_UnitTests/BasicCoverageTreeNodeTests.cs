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
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.CoverageTree;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class BasicCoverageTreeNodeTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestCoverage()
        {
            var coverage = new BaseCoverageTest(10, 50);
            var node = new BasicCoverageTreeNode(null, coverage, 
                RootCoverageTreeNode.IconFilename, false);

            Assert.AreEqual(10, node.CoveredLineCount);
            Assert.AreEqual(50, node.TotalLineCount);
            Assert.AreEqual(40, node.UncoveredLineCount);
            Assert.AreEqual(10 / 50.0, node.OptionalCoverageRate);
            Assert.AreEqual(40 / 50.0, node.OptionalUncoverageRate);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void TestNullCoverage()
        {
            var coverage = new BaseCoverageTest(0, 0);
            var node = new BasicCoverageTreeNode(null, coverage, 
                RootCoverageTreeNode.IconFilename, false);

            Assert.AreEqual(null, node.OptionalCoverageRate);
            Assert.AreEqual(null, node.OptionalUncoverageRate);
        }

        //---------------------------------------------------------------------
        class BaseCoverageTest : BaseCoverage
        {
            public BaseCoverageTest(
                int coverLineCount,
                int totalLineCount)
            {
                this.CoverLineCount = coverLineCount;
                this.TotalLineCount = totalLineCount;
            }
        }
    }
}
