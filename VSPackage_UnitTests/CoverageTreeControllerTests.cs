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
using Moq;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Editor;
using System.Collections.Generic;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class CoverageTreeControllerTests
    {
        readonly Mock<IEditorHighlighter> editorHighlighter;
        readonly CoverageTreeController controller;

        //---------------------------------------------------------------------
        public CoverageTreeControllerTests()
        {
            this.editorHighlighter = new Mock<IEditorHighlighter>();
            this.controller = new CoverageTreeController(editorHighlighter.Object);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Root()
        {
            var name = "name";

            bool propertyChangedCalled = false;
            controller.PropertyChanged += (s, e) => propertyChangedCalled = true;

            controller.CoverageRate = new CoverageRate(name, 0);
            Assert.IsTrue(propertyChangedCalled);
            Assert.AreEqual(name, controller.Root.Text);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void CurrentFileTreeNode()
        {            
            var node1 = new FileTreeNode("", new FileCoverage("", new List<LineCoverage>()));
            var node2 = new FileTreeNode("", new FileCoverage("", new List<LineCoverage>()));

            controller.Current = node1;
            editorHighlighter.Verify(e => e.DisplayCoverage(node1.Coverage), Times.Once);
            editorHighlighter.Verify(e => e.RemoveCoverage(null), Times.Never);
            
            controller.Current = node2;
            editorHighlighter.Verify(e => e.DisplayCoverage(node2.Coverage), Times.Once);
            editorHighlighter.Verify(e => e.RemoveCoverage(node1.Coverage), Times.Once);
            
            editorHighlighter.VerifyAll();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void CurrentIsNotFileTreeNode()
        {
            editorHighlighter.Verify(
                e => e.DisplayCoverage(It.IsAny<FileCoverage>()), Times.Never);

            controller.Current = new RootCoverageTreeNode(new CoverageRate("", 0));

            editorHighlighter.VerifyAll();
        }
    }
}   