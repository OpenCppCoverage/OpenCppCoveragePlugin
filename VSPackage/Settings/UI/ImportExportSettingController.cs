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
using System.IO;
using System.Linq;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class ImportExportSettingController: PropertyChangedNotifier
    {
        //---------------------------------------------------------------------
        public class Export : PropertyChangedNotifier
        {
            ImportExportSettings.Type type;
            string path;

            //-----------------------------------------------------------------
            public ImportExportSettings.Type Type
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
                    return (this.Type == ImportExportSettings.Type.Html) 
                        ? FileSystemSelectionControl.SelectionMode.FolderSelection 
                        : FileSystemSelectionControl.SelectionMode.NewFileSelection;
                }
            }

            //-----------------------------------------------------------------
            public string FileFilter
            {
                get
                {
                    switch (this.type)
                    {
                        case ImportExportSettings.Type.Binary: return "Coverage Files (*.cov)|*.cov";
                        case ImportExportSettings.Type.Cobertura: return "Coverage Files (*.xml)|*.xml";
                        case ImportExportSettings.Type.Html: return string.Empty;
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
        public static readonly string ExportPathExistError = 
            "Import/Export: Export path {0} already exists. Please select a new file.";
        public static readonly string ExportDirectoryNotEmptyError = 
            "Import/Export: Export directory {0} is not empty. Please select an empty directory.";
        //---------------------------------------------------------------------
        public event EventHandler<IEnumerable<string>> ErrorsChanged;

        //---------------------------------------------------------------------
        IEnumerable<string> errors = new List<string>();

        //---------------------------------------------------------------------
        public ImportExportSettingController()
        {
            this.ExportTypeValues = Enum.GetValues(typeof(ImportExportSettings.Type))
                .Cast<ImportExportSettings.Type>();
            this.Exports = new ObservableItemCollection<Export>();
            this.Exports.CollectionOrItemChanged += CollectionOrItemChangedHandler;
            this.InputCoverages = new ObservableCollection<BindableString>();
        }

        //---------------------------------------------------------------------
        public void UpdateErrorsStatus()
        {
            var newErrors = new List<string>();
            foreach (var export in this.Exports)
            {
                if (!string.IsNullOrEmpty(export.Path))
                {
                    if (File.Exists(export.Path))
                        newErrors.Add(string.Format(ExportPathExistError, export.Path));
                    else if (
                        Directory.Exists(export.Path)
                        && Directory.EnumerateFileSystemEntries(export.Path).Any())
                    {
                        newErrors.Add(string.Format(ExportDirectoryNotEmptyError, export.Path));
                    }
                }
            }
            if (!newErrors.SequenceEqual(this.errors))
            {
                this.errors = newErrors;
                this?.ErrorsChanged(this, this.errors);
            }
        }

        //---------------------------------------------------------------------
        void CollectionOrItemChangedHandler(object sender, EventArgs e)
        {
            UpdateErrorsStatus();
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject()
        {
            this.Exports.Clear();
            this.InputCoverages.Clear();
            this.CoverChildrenProcesses = false;
            this.AggregateByFile = true;
        }

        //---------------------------------------------------------------------
        public ImportExportSettings GetSettings()
        {
            return new ImportExportSettings
            {
                Exports = this.Exports.Select(e => new ImportExportSettings.Export
                {
                    Path = e.Path,
                    Type = e.Type
                }),
                InputCoverages = this.InputCoverages.ToStringList(),
                AggregateByFile = this.AggregateByFile,
                CoverChildrenProcesses = this.CoverChildrenProcesses  
            };
        }

        //---------------------------------------------------------------------
        public IEnumerable<ImportExportSettings.Type> ExportTypeValues { get; }
        public ObservableItemCollection<Export> Exports { get; }
        public ObservableCollection<BindableString> InputCoverages { get; }

        //-----------------------------------------------------------------
        bool coverChildrenProcesses;
        public bool CoverChildrenProcesses
        {
            get { return this.coverChildrenProcesses; }
            set { SetField(ref this.coverChildrenProcesses, value); }
        }

        //-----------------------------------------------------------------
        bool aggregateByFile;
        public bool AggregateByFile
        {
            get { return this.aggregateByFile; }
            set { SetField(ref this.aggregateByFile, value); }
        }
    }
}
