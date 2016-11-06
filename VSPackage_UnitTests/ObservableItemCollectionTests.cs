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
using OpenCppCoverage.VSPackage.Helper;

namespace VSPackage_UnitTests
{
    [TestClass]
    public class ObservableItemCollectionTests
    {
        ObservableItemCollection<BindableValue<int>> collection;
        bool eventCalled;

        //---------------------------------------------------------------------
        [TestInitialize]
        public void TestInitialize()
        {
            this.collection = new ObservableItemCollection<BindableValue<int>>();
            this.eventCalled = false;
            collection.CollectionOrItemChanged += (sender, e) => { this.eventCalled = true; };
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void CollectionOrItemChanged()
        {
            this.collection.Add(new BindableValue<int>(0));
            Assert.IsTrue(this.eventCalled);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ItemChanged()
        {
            var item = new BindableValue<int>(0);
            this.collection.Add(item);
            this.eventCalled = false;

            item.Value = 42;
            Assert.IsTrue(this.eventCalled);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void UnregisterItemRemove()
        {
            var item = new BindableValue<int>(0);
            this.collection.Add(item);
            this.collection.Remove(item);

            this.eventCalled = false;
            item.Value = 42;
            Assert.IsFalse(this.eventCalled);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void UnregisterClear()
        {
            var item = new BindableValue<int>(0);
            this.collection.Add(item);
            this.collection.Clear();

            this.eventCalled = false;
            item.Value = 42;
            Assert.IsFalse(this.eventCalled);
        }
    }
}