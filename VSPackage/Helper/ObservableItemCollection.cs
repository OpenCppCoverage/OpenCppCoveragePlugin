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
using System.Collections.Specialized;
using System.ComponentModel;

namespace OpenCppCoverage.VSPackage.Helper
{
    public class ObservableItemCollection<T> 
        : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        //---------------------------------------------------------------------
        public event EventHandler CollectionOrItemChanged;

        //---------------------------------------------------------------------
        public ObservableItemCollection()
        {
            this.CollectionChanged += CollectionChangedHandler;
        }

        //---------------------------------------------------------------------
        void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (T items in e.NewItems)
                    items.PropertyChanged += PropertyChangedHandler;
            }

            if (e.OldItems != null)
            {
                foreach (T items in e.OldItems)
                    items.PropertyChanged -= PropertyChangedHandler;
            }
            this?.CollectionOrItemChanged(sender, e);
        }

        //---------------------------------------------------------------------
        protected override void ClearItems()
        {
            foreach (var item in this)
                item.PropertyChanged -= PropertyChangedHandler;
            base.ClearItems();
        }

        //---------------------------------------------------------------------
        void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            this?.CollectionOrItemChanged(sender, e);
        }
    }
}
