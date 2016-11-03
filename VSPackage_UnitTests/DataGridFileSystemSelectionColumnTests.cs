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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VSPackage_UnitTests
{
    //-------------------------------------------------------------------------
    [TestClass]
    public class DataGridFileSystemSelectionColumnTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void GenerateEditingElementConstantBindings()
        {
            TestHelper.RunInUIhread(() =>
            {
                var dataGridColumn = new DataGridFileSystemSelectionColumnMock();
                var str = new BindableValue<string>("42");

                dataGridColumn.Binding = CreateBinding(str);
                dataGridColumn.FileFilter = "filter";
                dataGridColumn.Mode = FileSystemSelectionControl.SelectionMode.FolderSelection;
                var cell = new DataGridCell();
                var control = (FileSystemSelectionControl)
                                    dataGridColumn.GenerateEditingElementPublic(
                                        cell, null);
                Assert.AreEqual(dataGridColumn.FileFilter, control.FileFilter);
                Assert.AreEqual(dataGridColumn.Mode, control.Mode);
                Assert.AreEqual(str.Value, control.SelectedPath);
                Assert.AreEqual(str.Value, cell.ToolTip);
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void GenerateEditingElementBinding()
        {
            TestHelper.RunInUIhread(() =>
            {
                var dataGridColumn = new DataGridFileSystemSelectionColumnMock();
                dataGridColumn.Binding = CreateBinding(new BindableValue<string>("42"));

                var fileFilter = new BindableValue<string>(string.Empty);
                dataGridColumn.FileFilterBinding = CreateBinding(fileFilter);

                var mode = new BindableValue<FileSystemSelectionControl.SelectionMode>(
                    FileSystemSelectionControl.SelectionMode.ExistingFileSelection);
                dataGridColumn.ModeBinding = CreateBinding(mode);

                var control = (FileSystemSelectionControl)
                                    dataGridColumn.GenerateEditingElementPublic(
                                        new DataGridCell(), null);

                fileFilter.Value = "fileFilter";
                mode.Value = FileSystemSelectionControl.SelectionMode.FolderSelection;

                Assert.AreEqual(fileFilter.Value, control.FileFilter);
                Assert.AreEqual(mode.Value, control.Mode);
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void GenerateElement()
        {
            TestHelper.RunInUIhread(() =>
            {
                var dataGridColumn = new DataGridFileSystemSelectionColumnMock();
                var value = new BindableValue<string>("42");
                dataGridColumn.Binding = CreateBinding(value); 

                var control = (TextBlock)
                                    dataGridColumn.GenerateElementPublic(
                                        new DataGridCell(), null);

                value.Value = "newValue";
                Assert.AreEqual(value.Value, control.Text);
            });
        }

        //-------------------------------------------------------------------------
        class DataGridFileSystemSelectionColumnMock : DataGridFileSystemSelectionColumn
        {
            //---------------------------------------------------------------------
            public FrameworkElement GenerateEditingElementPublic(
                DataGridCell cell,
                object dataItem)
            {
                return GenerateEditingElement(cell, dataItem);
            }

            //---------------------------------------------------------------------
            public FrameworkElement GenerateElementPublic(
                    DataGridCell cell,
                    object dataItem)
            {
                return GenerateElement(cell, dataItem);
            }

        }

        //---------------------------------------------------------------------
        public class BindableValue<T> : PropertyChangedNotifier
        {
            T value;

            public BindableValue(T value)
            {
                this.value = value;
            }

            public T Value
            {
                get { return this.value; }
                set { this.SetField(ref this.value, value); }
            }
        }

        //---------------------------------------------------------------------
        Binding CreateBinding<T>(BindableValue<T> str)
        {
            var binding = new Binding();
            binding.Source = str;
            binding.Path = new PropertyPath(nameof(BindableValue<T>.Value));

            return binding;
        }
    }
}