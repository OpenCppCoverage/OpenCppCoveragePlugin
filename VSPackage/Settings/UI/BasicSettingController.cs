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

        //---------------------------------------------------------------------
        public override string ToString()
        {
            return this.FullName;
        }
    }

    //-------------------------------------------------------------------------
    class BasicSettingController: PropertyChangedNotifier
    {
        //---------------------------------------------------------------------
        public static string None = "None";

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

            if (String.IsNullOrEmpty(settings.WorkingDir))
                this.HasWorkingDirectory = false;
            else
            {
                this.HasWorkingDirectory = true;
                this.OptionalWorkingDirectory = settings.WorkingDir;
            }

            this.Arguments = settings.Arguments;

            if (String.IsNullOrEmpty(settings.ProjectName) 
             || String.IsNullOrEmpty(settings.SolutionConfigurationName))
            {
                this.CurrentProject = None;
                this.CurrentConfiguration = None;
                this.IsCompileBeforeRunningEnabled = false;
                this.CompileBeforeRunningToolTip = "Nothing to build (No startup project set).";
            }
            else
            {
                this.CurrentProject = settings.ProjectName;
                this.CurrentConfiguration = settings.SolutionConfigurationName;
                this.IsCompileBeforeRunningEnabled = true;
                this.CompileBeforeRunningToolTip = null;
                this.CompileBeforeRunning = true;
            }
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
                WorkingDirectory = this.OptionalWorkingDirectory
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
        string optionalWorkingDirectory;
        public string OptionalWorkingDirectory
        {
            get { return this.optionalWorkingDirectory; }
            set { this.SetField(ref this.optionalWorkingDirectory, value); }
        }

        //---------------------------------------------------------------------
        bool hasWorkingDirectory;
        public bool HasWorkingDirectory
        {
            get { return this.hasWorkingDirectory; }
            set
            {
                if (this.SetField(ref this.hasWorkingDirectory, value) && !value)
                    this.OptionalWorkingDirectory = null;
            }
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

        //---------------------------------------------------------------------
        bool isCompileBeforeRunningEnabled;
        public bool IsCompileBeforeRunningEnabled
        {
            get { return this.isCompileBeforeRunningEnabled; }
            set { this.SetField(ref this.isCompileBeforeRunningEnabled, value); }
        }

        //---------------------------------------------------------------------
        string compileBeforeRunningToolTip;
        public string CompileBeforeRunningToolTip
        {
            get { return this.compileBeforeRunningToolTip; }
            set { this.SetField(ref this.compileBeforeRunningToolTip, value); }
        }

        //---------------------------------------------------------------------
        string currentProject;
        public string CurrentProject
        {
            get { return this.currentProject; }
            set { this.SetField(ref this.currentProject, value); }
        }

        //---------------------------------------------------------------------
        string currentConfiguration;
        public string CurrentConfiguration
        {
            get { return this.currentConfiguration; }
            set { this.SetField(ref this.currentConfiguration, value); }
        }
    }
}
