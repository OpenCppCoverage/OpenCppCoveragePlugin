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
        public SelectableProject(StartUpProjectSettings.CppProject project)
        {
            this.Name = Path.GetFileNameWithoutExtension(project.Path);
            this.FullName = project.Path;
            this.Project = project;
            this.IsSelected = true;
        }
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public StartUpProjectSettings.CppProject Project { get; private set; }
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
                project => new SelectableProject(project)).ToList();
            this.ProgramToRun = settings.Command;
            this.WorkingDirectory = settings.WorkingDir;
            this.Arguments = settings.Arguments;
            this.CompileBeforeRunning = true;
        }

        //---------------------------------------------------------------------
        public BasicSettings GetSettings()
        {
            var selectedProjects = this.SelectableProjects
                .Where(p => p.IsSelected)
                .Select(p => p.Project)
                .ToList();
            return new BasicSettings
            {
                ModulePaths = selectedProjects.Select(project => project.ModulePath),
                SourcePaths = selectedProjects.SelectMany(project => project.SourcePaths),
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
            set { this.SetField(ref this.programToRun, value); }
        }

        //---------------------------------------------------------------------
        string workingDirectory;
        public string WorkingDirectory
        {
            get { return this.workingDirectory; }
            set { this.SetField(ref this.workingDirectory, value); }
        }

        //---------------------------------------------------------------------
        string arguments;
        public string Arguments
        {
            get { return this.arguments; }
            set { this.SetField(ref this.arguments, value); }
        }

        //---------------------------------------------------------------------
        bool compileBeforeRunning;
        public bool CompileBeforeRunning
        {
            get { return this.compileBeforeRunning; }
            set { this.SetField(ref this.compileBeforeRunning, value); }
        }
    }
}
