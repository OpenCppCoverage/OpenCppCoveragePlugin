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
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class SelectableProject
    {
        //---------------------------------------------------------------------
        public SelectableProject(string fullName)
        {
            this.IsSelected = true;
            this.Name = Path.GetFileNameWithoutExtension(fullName);
            this.FullName = fullName;
        }
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public bool IsSelected { get; set; }
    }

    //-------------------------------------------------------------------------
    class BasicSettingController: PropertyChangedNotifier
    {
        readonly IFileSystemDialog fileSystemDialog;
        string programToRun;
        string workingDirectory;

        //---------------------------------------------------------------------
        public BasicSettingController(IFileSystemDialog fileSystemDialog)
        {
            this.fileSystemDialog = fileSystemDialog;
            this.SelectableProjects = new List<SelectableProject>();

            this.ProgramToRunCommand = new RelayCommand(
                () => { this.fileSystemDialog.SelectFile(
                            "Executable Files (.exe)|*.exe", 
                            filename => ProgramToRun = filename); });
            this.WorkingDirectoryCommand = new RelayCommand(
                () => { this.fileSystemDialog.SelectFolder(path => WorkingDirectory = path); });
            this.CompileBeforeRunning = true;
        }

        //---------------------------------------------------------------------
        public IEnumerable<SelectableProject> SelectableProjects { get; private set; }
        
        //---------------------------------------------------------------------
        public string ProgramToRun
        {
            get { return programToRun; }
            private set { SetField(ref programToRun, value); }
        }

        //---------------------------------------------------------------------
        public ICommand ProgramToRunCommand { get; set; }
        
        //---------------------------------------------------------------------
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            private set { SetField(ref workingDirectory, value); }
        }

        //---------------------------------------------------------------------
        public ICommand WorkingDirectoryCommand { get; private set; }

        //---------------------------------------------------------------------
        public string Arguments { get; set; }

        //---------------------------------------------------------------------
        public bool CompileBeforeRunning { get; set; }
    }
}
