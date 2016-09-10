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
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        //---------------------------------------------------------------------
        public BasicSettingController()
        {
            this.SelectableProjects = new List<SelectableProject>();
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject(StartUpProjectSettings settings)
        {
            this.SelectableProjects = settings.CppProjects.Select(
                project => new SelectableProject(project.Name)).ToList();
            this.ProgramToRun = settings.Command;
            this.WorkingDirectory = settings.WorkingDir;
            this.Arguments = settings.Arguments;
            this.CompileBeforeRunning = true;
        }

        //---------------------------------------------------------------------
        public BasicSettings GetSettings()
        {
            return new BasicSettings
            {
                Arguments = this.Arguments,
                ProgramToRun = this.ProgramToRun,
                CompileBeforeRunning = this.CompileBeforeRunning,
                WorkingDirectory = this.WorkingDirectory
            };
        }

        //---------------------------------------------------------------------
        List<SelectableProject> selectableProjects;
        public List<SelectableProject> SelectableProjects
        {
            get { return this.selectableProjects; }
            private set { this.SetField(ref this.selectableProjects, value); }
        }

        //---------------------------------------------------------------------
        string programToRun;
        public string ProgramToRun
        {
            get { return this.programToRun; }
            private set { this.SetField(ref this.programToRun, value); }
        }

        //---------------------------------------------------------------------
        string workingDirectory;
        public string WorkingDirectory
        {
            get { return this.workingDirectory; }
            private set { this.SetField(ref this.workingDirectory, value); }
        }

        //---------------------------------------------------------------------
        string arguments;
        public string Arguments
        {
            get { return this.arguments; }
            private set { this.SetField(ref this.arguments, value); }
        }

        //---------------------------------------------------------------------
        bool compileBeforeRunning;
        public bool CompileBeforeRunning
        {
            get { return this.compileBeforeRunning; }
            private set { this.SetField(ref this.compileBeforeRunning, value); }
        }
    }
}
