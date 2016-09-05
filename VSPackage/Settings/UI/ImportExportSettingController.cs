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

using OpenCppCoverage.VSPackage.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
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
            ExportType type;
            string path;

            //-----------------------------------------------------------------
            public ExportType Type
            {
                get { return this.type; }
                set
                {
                    // Reset path because it can be either a file or a folder.
                    if (SetField(ref this.type, value))
                        this.Path = null;
                }
            }

            //-----------------------------------------------------------------
            public FileSystemSelectionControl.SelectionMode SelectionMode
            {
                get
                {
                    return (this.Type == ImportExportSettingController.ExportType.Html) 
                        ? FileSystemSelectionControl.SelectionMode.FolderSelection 
                        : FileSystemSelectionControl.SelectionMode.FileSelection;
                }
            }

            //-----------------------------------------------------------------
            public string FileFilter
            {
                get
                {
                    switch (this.type)
                    {
                        case ExportType.Binary: return "Coverage Files (*.cov)|*.cov";
                        case ExportType.Cobertura: return "Coverage Files (*.xml)|*.xml";
                        case ExportType.Html: return string.Empty;
                    }
                    throw new NotSupportedException();
                }
            }

            //-----------------------------------------------------------------
            public string Path
            {
                get { return this.path; }
                set { SetField(ref this.path, value); }
            }
        }

        //---------------------------------------------------------------------
        public ImportExportSettingController()
        {
            this.ExportTypeValues = Enum.GetValues(typeof(ExportType)).Cast<ExportType>();
            this.Exports = new ObservableCollection<Export>();
            this.InputCoverages = new ObservableCollection<BindableString>();
            this.AggregateByFile = true;
        }

        //---------------------------------------------------------------------
        public IEnumerable<ExportType> ExportTypeValues { get; private set; }
        public ObservableCollection<Export> Exports { get; private set; }
        public ObservableCollection<BindableString> InputCoverages { get; private set; }        
        public bool CoverChildrenProcesses { get; set; }
        public bool AggregateByFile { get; set; }
    }
}
