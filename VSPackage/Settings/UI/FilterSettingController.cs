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

using GalaSoft.MvvmLight.Command;
using OpenCppCoverage.VSPackage.Helper;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class UnifiedDiffs : PropertyChangedNotifier
    {
        string unifiedDiffPath;
        string optionalRootFolder;

        //---------------------------------------------------------------------
        public string UnifiedDiffPath
        {
            get { return this.unifiedDiffPath; }
            set { SetField(ref this.unifiedDiffPath, value); }
        }

        //---------------------------------------------------------------------
        public string OptionalRootFolder
        {
            get { return this.optionalRootFolder; }
            set { SetField(ref this.optionalRootFolder, value); }
        }
    }

    //-------------------------------------------------------------------------
    class FilterSettingController
    {
        readonly IFileSystemDialog fileSystemDialog;

        //---------------------------------------------------------------------
        public FilterSettingController(IFileSystemDialog fileSystemDialog)
        {
            this.fileSystemDialog = fileSystemDialog;
            this.SourcePatterns = new ObservableCollection<BindableString>();
            this.ExcludedSourcePatterns = new ObservableCollection<BindableString>();
            this.ModulePatterns = new ObservableCollection<BindableString>();
            this.ExcludedModulePatterns = new ObservableCollection<BindableString>();
            this.UnifiedDiffs = new ObservableCollection<UnifiedDiffs>();            
            this.UnifiedDiffCellClickCommand = new RelayCommand(OnUnifiedDiffCellClickCommand);
        }

        //---------------------------------------------------------------------
        void OnUnifiedDiffCellClickCommand()
        {
            DataGridHelper.HandleCellClick(this.CurrentUnifiedDiffCellInfo, this.UnifiedDiffs,
                (item, bindingPath) =>
                {
                    switch (bindingPath)
                    {
                        case nameof(item.UnifiedDiffPath):
                            return fileSystemDialog.SelectFile(
                                "Diff Files (*.diff)|*.diff|All Files (*.*)|*.*",
                                path => item.UnifiedDiffPath = path);
                        case nameof(item.OptionalRootFolder):
                            return fileSystemDialog.SelectFolder(path => item.OptionalRootFolder = path);
                        default:
                            throw new InvalidOperationException("Invalid Value for BindingPath: " + bindingPath);
                    };
                });
        }

        public DataGridCellInfo CurrentUnifiedDiffCellInfo { get; set; }
        public ICommand UnifiedDiffCellClickCommand { get; private set; }
        public ObservableCollection<BindableString> SourcePatterns { get; private set; }
        public ObservableCollection<BindableString> ExcludedSourcePatterns { get; private set; }
        public ObservableCollection<BindableString> ModulePatterns { get; private set; }
        public ObservableCollection<BindableString> ExcludedModulePatterns { get; private set; }
        public ObservableCollection<UnifiedDiffs> UnifiedDiffs { get; private set; }
    }
}
