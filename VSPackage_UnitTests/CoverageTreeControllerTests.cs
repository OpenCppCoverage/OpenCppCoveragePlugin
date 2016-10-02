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
        readonly CoverageTreeController controller;

        //---------------------------------------------------------------------
        public CoverageTreeControllerTests()
        {
            this.controller = new CoverageTreeController();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Root()
        {
            var name = "name";

            bool propertyChangedCalled = false;
            controller.PropertyChanged += (s, e) => propertyChangedCalled = true;

            controller.Filter = "Filter";
            controller.CoverageRate = new CoverageRate(name, 0);
            Assert.IsTrue(propertyChangedCalled);
            Assert.AreEqual(name, controller.Root.Text);
            Assert.AreEqual("", controller.Filter);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void Filter()
        {
            this.controller.CoverageRate = new CoverageRate("", 0);
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
            this.controller.CoverageRate = new CoverageRate("", 0);
            Assert.IsNull(this.controller.Warning);

            this.controller.CoverageRate = new CoverageRate("", 42);
            Assert.IsNotNull(this.controller.Warning);
        }
    }
}   