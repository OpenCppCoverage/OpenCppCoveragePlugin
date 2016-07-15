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
using System.Collections.Generic;
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class RootCoverageTreeNodeTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void RootCoverageTreeNode()
        {
            var file = new FileCoverage("file", new List<LineCoverage>());
            var module = new ModuleCoverage("module");
            module.AddChild(file);

            const int exitCode = 42;
            var coverage = new CoverageRate("root", exitCode);
            coverage.AddChild(module);

            var root = new RootCoverageTreeNode(coverage);
            root.EnsureLazyChildren();
            var moduleNode = root.Modules.First();

            moduleNode.EnsureLazyChildren();
            var fileNode = moduleNode.Files.First();

            Assert.AreEqual(coverage.Name, root.Text);
            Assert.AreEqual(module.Name, moduleNode.Text);
            Assert.AreEqual(file.Path, fileNode.Text);
        }
    }
}
