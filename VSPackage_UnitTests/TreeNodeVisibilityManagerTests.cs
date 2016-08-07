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
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.CoverageTree;
using System.Collections.Generic;
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class TreeNodeVisibilityManagerTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void EmptyFilter()
        {
            var result = UpdateVisibility(CreateRoot("module", "file1", "file2"), "");

            Assert.AreEqual(false, result.Module.IsHidden);
            Assert.AreEqual(false, result.File1.IsHidden);
            Assert.AreEqual(false, result.File2.IsHidden);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ModuleNotHidden()
        {
            var result = UpdateVisibility(CreateRoot("module", "file1", "file2"), "module");

            Assert.AreEqual(false, result.Module.IsHidden);
            Assert.AreEqual(true, result.File1.IsHidden);
            Assert.AreEqual(true, result.File2.IsHidden);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void None()
        {
            var result = UpdateVisibility(CreateRoot("module", "file1", "file2"), "none");

            Assert.AreEqual(true, result.Module.IsHidden);

            // As parent is hidden, children not need to be.
            Assert.AreEqual(false, result.File1.IsHidden);
            Assert.AreEqual(false, result.File2.IsHidden);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Child()
        {
            var result = UpdateVisibility(CreateRoot("module", "file1", "file2"), "1");

            Assert.AreEqual(false, result.Module.IsHidden);
            Assert.AreEqual(false, result.File1.IsHidden);
            Assert.AreEqual(true, result.File2.IsHidden);
        }

        //---------------------------------------------------------------------
        class VisibilityResults
        {
            public ModuleTreeNode Module { get; set; }
            public FileTreeNode File1 { get; set; }
            public FileTreeNode File2 { get; set; }
        }

        //---------------------------------------------------------------------
        VisibilityResults UpdateVisibility(RootCoverageTreeNode root, string filter)
        {
            var visibilityManager = new TreeNodeVisibilityManager();

            visibilityManager.UpdateVisibility(root, filter);
            var results = new VisibilityResults();

            results.Module = root.Modules.Single();
            var fileTreeNodes = results.Module.Files.ToList();
            results.File1 = fileTreeNodes[0];
            results.File2 = fileTreeNodes[1];

            return results;
        }

        //---------------------------------------------------------------------
        RootCoverageTreeNode CreateRoot(
            string moduleName, 
            string file1, 
            string file2)
        {
            var module = new ModuleCoverage(moduleName);
            module.AddChild(new FileCoverage(file1, new List<LineCoverage>()));
            module.AddChild(new FileCoverage(file2, new List<LineCoverage>()));
            var coverage = new CoverageRate("root", 0);
            coverage.AddChild(module);

            return new RootCoverageTreeNode(coverage);
        }
    }
}
