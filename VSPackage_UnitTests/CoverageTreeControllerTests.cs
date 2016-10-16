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
using System.Collections.Generic;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class CoverageTreeControllerTests
    {
        CoverageTreeController controller;
        Mock<ICoverageViewManager> coverageViewManager;

        //---------------------------------------------------------------------
        [TestInitialize]
        public void Initialize()
        {
            this.controller = new CoverageTreeController();
            this.coverageViewManager = new Mock<ICoverageViewManager>();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Root()
        {
            var name = "name";

            bool propertyChangedCalled = false;
            controller.PropertyChanged += (s, e) => propertyChangedCalled = true;

            controller.Filter = "Filter";
            controller.UpdateCoverageRate(
                new CoverageRate(name, 0), null, this.coverageViewManager.Object);
            Assert.IsTrue(propertyChangedCalled);
            Assert.AreEqual(name, controller.Root.Text);
            Assert.AreEqual("", controller.Filter);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Filter()
        {
            this.controller.UpdateCoverageRate(
                new CoverageRate("", 0), null, this.coverageViewManager.Object);
            this.controller.Filter = "filter";

            bool rootChanged = false;
            controller.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Root")
                    rootChanged = true;
            };

            controller.Filter = string.Empty;
            Assert.IsTrue(rootChanged);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Warning()
        {
            Assert.IsNull(this.controller.Warning);
            this.controller.UpdateCoverageRate(
                new CoverageRate("", 0), null, this.coverageViewManager.Object);
            Assert.IsNull(this.controller.Warning);

            this.controller.UpdateCoverageRate(
                new CoverageRate("", 42), null, this.coverageViewManager.Object);
            Assert.IsNotNull(this.controller.Warning);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void DisplayCoverage()
        {
            this.controller.UpdateCoverageRate(
                new CoverageRate("", 0), null, this.coverageViewManager.Object);
            this.coverageViewManager.VerifySet(c => c.ShowCoverage = true);
            
            this.controller.DisplayCoverage = false;
            this.coverageViewManager.VerifySet(c => c.ShowCoverage = false);
        }
    }
}   