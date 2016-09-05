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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenCppCoverage.VSPackage.Helper
{
    class DataGridFileSystemSelectionColumn : DataGridBoundColumn
    {
        public BindingBase FileFilterBinding { get; set; }
        public string FileFilter { get; set; }

        public BindingBase ModeBinding { get; set; }
        public FileSystemSelectionControl.SelectionMode Mode { get; set; }

        //-----------------------------------------------------------------------   
        protected override FrameworkElement GenerateEditingElement(
            DataGridCell cell, 
            object dataItem)
        {
            var fileSystemSelectionControl = new FileSystemSelectionControl();

            if (!BindIfNotNull(
                    fileSystemSelectionControl, 
                    FileSystemSelectionControl.FileFilterProperty, 
                    this.FileFilterBinding))
                fileSystemSelectionControl.FileFilter = this.FileFilter;

            if (!BindIfNotNull(
                    fileSystemSelectionControl, 
                    FileSystemSelectionControl.ModeProperty, 
                    this.ModeBinding))
                fileSystemSelectionControl.Mode = this.Mode;
            
            fileSystemSelectionControl.SetBinding(
                FileSystemSelectionControl.SelectedPathProperty, 
                this.Binding);
            cell.SetBinding(DataGridCell.ToolTipProperty, this.Binding);

            return fileSystemSelectionControl;
        }

        //-----------------------------------------------------------------------
        protected override FrameworkElement GenerateElement(
            DataGridCell cell, 
            object dataItem)
        {
            var currentTextBlock = new TextBlock();
            currentTextBlock.SetBinding(TextBlock.TextProperty, this.Binding);

            return currentTextBlock;
        }

        //-----------------------------------------------------------------------   
        bool BindIfNotNull(
            FrameworkElement element, 
            DependencyProperty property, 
            BindingBase binding)
        {
            if (binding != null)
            {
                element.SetBinding(property, binding);
                return true;
            }

            return false;
        }
    }
}