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

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    static class DataGridHelper
    {
        //---------------------------------------------------------------------
        public static void HandleCellClick<T>(
            DataGridCellInfo cellInfo,
            ObservableCollection<T> collection,
            Func<T, string, bool> action) where T : class, new()
        {
            if (cellInfo.IsValid)
            {
                var item = cellInfo.Item as T;
                bool newItemCreated = false;

                if (item == null && cellInfo.Item == CollectionView.NewItemPlaceholder)
                {
                    item = new T();
                    newItemCreated = true;
                }

                if (item == null)
                    throw new InvalidOperationException("Error in HandleCellClick");

                var column = cellInfo.Column as DataGridBoundColumn;

                if (column != null)
                {
                    var binding = (Binding)column.Binding;
                    var propertyPath = binding.Path;

                    if (action(item, propertyPath.Path) && newItemCreated)
                        collection.Add(item);
                }
            }
        }
    }
}
