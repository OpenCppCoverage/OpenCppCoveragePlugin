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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    class ImportExportSettingController
    {
        //---------------------------------------------------------------------
        public enum ExportType
        {
            Html,
            Cobertura,
            Binary
        }

        //---------------------------------------------------------------------
        public class Export : PropertyChangedNotifier
        {
            ExportType? optionalType;
            string path;
                                   
            //-----------------------------------------------------------------
            public ExportType Type
            {
                get { return this.optionalType ?? default(ExportType); }
                set
                {
                    // Reset path because it can be either a file or a folder.
                    if (SetField(ref this.optionalType, value))
                        this.Path = null;
                }
            }

            //-----------------------------------------------------------------
            public ExportType? OptionalType
            {
                get { return this.optionalType; }
            }

            //-----------------------------------------------------------------
            public string Path
            {
                get { return this.path; }
                set { SetField(ref this.path, value); }
            }
        }

        readonly IFileSystemDialog fileSystemDialog;

        //---------------------------------------------------------------------
        public ImportExportSettingController(IFileSystemDialog fileSystemDialog)
        {
            this.fileSystemDialog = fileSystemDialog;
            this.ExportTypeValues = Enum.GetValues(typeof(ExportType)).Cast<ExportType>();
            this.Exports = new ObservableCollection<Export>();
            this.ExportCellClickCommand = new RelayCommand(OnExportCellClickCommand);
            this.InputCoverageCellClickCommand = new RelayCommand(OnInputCoverageCellClickCommand);
            this.InputCoverages = new ObservableCollection<BindableString>();
            this.AggregateByFile = true;
        }

        //---------------------------------------------------------------------
        public IEnumerable<ExportType> ExportTypeValues { get; private set; }
        public ObservableCollection<Export> Exports { get; private set; }
        public DataGridCellInfo CurrentExport { get; set; }
        public ICommand ExportCellClickCommand { get; private set; }

        //---------------------------------------------------------------------
        public ObservableCollection<BindableString> InputCoverages { get; private set; }
        public DataGridCellInfo CurrentInput { get; set; }
        public ICommand InputCoverageCellClickCommand { get; private set; }

        //---------------------------------------------------------------------
        public bool CoverChildrenProcesses { get; set; }
        public bool AggregateByFile { get; set; }

        //---------------------------------------------------------------------
        void OnInputCoverageCellClickCommand()
        {
            DataGridHelper.HandleCellClick(this.CurrentInput, this.InputCoverages,
                (item, bindingPath) =>
                {
                    if (bindingPath == nameof(item.Value))
                    {
                        return fileSystemDialog.SelectFile(
                            "Coverage Files (*.cov)|*.cov",
                            path => item.Value = path);
                    }

                    return false;
                });
        }

        //---------------------------------------------------------------------
        void OnExportCellClickCommand()
        {
            DataGridHelper.HandleCellClick(this.CurrentExport, this.Exports,
                (item, bindingPath) =>
                {
                    // If the item is a placeHolder, item.Type is set to the first value and not to the selected value.
                    // So we force the user to validate before entering the path.
                    if (item.OptionalType != null && bindingPath == nameof(item.Path))
                    {
                        switch (item.OptionalType.Value)
                        {
                            case ExportType.Html:
                                return fileSystemDialog.SelectFolder(path => item.Path = path);
                            case ExportType.Cobertura:
                                return fileSystemDialog.SelectFile(
                                        "Coverage Files (*.xml)|*.xml",
                                        path => item.Path = path);
                            case ExportType.Binary:
                                return fileSystemDialog.SelectFile(
                                    "Coverage Files (*.cov)|*.cov",
                                    path => item.Path = path);
                        }
                    }
                    return false;
                });
        }
    }
}
