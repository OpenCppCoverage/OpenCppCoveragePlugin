// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2014 OpenCppCoverage
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

using EnvDTE;
using EnvDTE80;
using Microsoft.CSharp.RuntimeBinder;
using OpenCppCoverage.VSPackage.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCppCoverage.VSPackage.Settings
{
    class StartUpProjectSettingsBuilder: IStartUpProjectSettingsBuilder
    {
        //---------------------------------------------------------------------
        public StartUpProjectSettingsBuilder(
            DTE2 dte,
            IConfigurationManager configurationManager)
        {
            this.dte = dte;
            this.configurationManager = configurationManager;
        }

        //---------------------------------------------------------------------
        public StartUpProjectSettings ComputeSettings(ProjectSelectionKind kind)
        {
            var solution = (Solution2)dte.Solution;
            var solutionBuild = (SolutionBuild2)solution.SolutionBuild;
            var activeConfiguration = (SolutionConfiguration2)solutionBuild.ActiveConfiguration;

            if (activeConfiguration != null)
            {
                var settings = ComputeOptionalSettings(activeConfiguration, kind);

                if (settings != null)
                    return settings;
            }

            return new StartUpProjectSettings
            {
                CppProjects = new List<StartUpProjectSettings.CppProject>()
            };
        }

        //---------------------------------------------------------------------
        StartUpProjectSettings ComputeOptionalSettings(
            SolutionConfiguration2 activeConfiguration,
            ProjectSelectionKind kind)
        {
            var solution = (Solution2)dte.Solution;
            var projects = GetProjects(solution);
            ExtendedProject project = null;

            switch (kind)
            {
                case ProjectSelectionKind.StartUpProject:
                    project = GetOptionalStartupProject(solution, projects);
                break;
                case ProjectSelectionKind.SelectedProject:
                    project = GetOptionalSelectedProject(projects);
                break;
            }

            if (project == null) 
                return null;

            return ComputeOptionalSettings(activeConfiguration, projects, project);
        }

        //---------------------------------------------------------------------
        StartUpProjectSettings ComputeOptionalSettings(
            SolutionConfiguration2 activeConfiguration,
            List<ExtendedProject> projects,
            ExtendedProject project)
        { 
            var startupConfiguration = this.configurationManager.GetConfiguration(
                activeConfiguration, project);
            var debugSettings = startupConfiguration.DebugSettings;

            var settings = new StartUpProjectSettings();
            settings.WorkingDir = startupConfiguration.Evaluate(debugSettings.WorkingDirectory);
            settings.Arguments = startupConfiguration.Evaluate(debugSettings.CommandArguments);
            settings.Command = startupConfiguration.Evaluate(debugSettings.Command);
            settings.SolutionConfigurationName = 
                this.configurationManager.GetSolutionConfigurationName(activeConfiguration);
            settings.ProjectName = project.UniqueName;
            settings.ProjectPath = project.Path;
            settings.CppProjects = BuildCppProject(
                activeConfiguration, this.configurationManager, projects);

            var vcclCompilerTool = startupConfiguration.OptionalVCCLCompilerTool;
            if (vcclCompilerTool == null)
                return null;
            settings.IsOptimizedBuildEnabled = !vcclCompilerTool.IsOptimizeDisabled;
            settings.EnvironmentVariables = GetEnvironmentVariables(startupConfiguration);

            return settings;                                                 
        }

        //---------------------------------------------------------------------
        static IEnumerable<KeyValuePair<string, string>> 
            GetEnvironmentVariables(DynamicVCConfiguration configuration)
        {
            var environmentVariables = new List<KeyValuePair<string, string>>();
            string environmentStr = configuration.Evaluate("$(LocalDebuggerEnvironment)");

            foreach (var str in environmentStr.Split('\n'))
            {
                var equalIndex = str.IndexOf('=');
                if (equalIndex != -1 && equalIndex != str.Length - 1)
                {
                    var key = str.Substring(0, equalIndex);
                    var value = str.Substring(equalIndex + 1);

                    environmentVariables.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            return environmentVariables;
        }

        //---------------------------------------------------------------------
        List<ExtendedProject> GetProjects(Solution2 solution)
        {
            var projects = new List<ExtendedProject>();
            
            foreach (Project project in solution.Projects)
                projects.AddRange(CreateExtendedProjectsFor(project));

            return projects;
        }

        //---------------------------------------------------------------------
        List<ExtendedProject> CreateExtendedProjectsFor(Project project)
        {
            var projects = new List<ExtendedProject>();

            if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem projectItem in project.ProjectItems)
                {
                    var subProject = projectItem.SubProject;
                    if (subProject != null)
                        projects.AddRange(CreateExtendedProjectsFor(subProject));
                }
            }
            else
            {
                dynamic projectObject = project.Object;

                try
                {
                    if (projectObject.Kind == "VCProject")
                        projects.Add(new ExtendedProject(project, new DynamicVCProject(projectObject)));
                }
                catch (RuntimeBinderException)
                {
                    // Nothing because not a VCProject
                }
            }

            return projects;
        }

        //---------------------------------------------------------------------
        ExtendedProject GetOptionalStartupProject(
            Solution2 solution,
            List<ExtendedProject> projects)
        {            
            var startupProjectsNames = solution.SolutionBuild.StartupProjects as object[];

            if (startupProjectsNames == null)
                return null;

            var startupProjectsSet = new HashSet<String>();
            foreach (String projectName in startupProjectsNames)
                startupProjectsSet.Add(projectName);

            return projects.Where(p => startupProjectsSet.Contains(p.UniqueName)).FirstOrDefault();
        }

        //---------------------------------------------------------------------
        ExtendedProject GetOptionalSelectedProject(List<ExtendedProject> projects)
        {
            var selectedProjects = ((Array)this.dte.ActiveSolutionProjects).Cast<Project>();
            
            if (selectedProjects.Count() != 1)
                return null;

            var projectName = selectedProjects.First().UniqueName;
            return projects.Where(p => p.UniqueName == projectName).FirstOrDefault();
        }

        //---------------------------------------------------------------------
        static IEnumerable<StartUpProjectSettings.CppProject> BuildCppProject(
            SolutionConfiguration2 activeConfiguration,
            IConfigurationManager configurationManager,
            List<ExtendedProject> projects)
        {
            var cppProjects = new List<StartUpProjectSettings.CppProject>();

            foreach (var project in projects)
            {
                var configuration = configurationManager.FindConfiguration(activeConfiguration, project);

                if (configuration != null)
                {
                    var cppProject = new StartUpProjectSettings.CppProject()
                    {
                        ModulePath = configuration.PrimaryOutput,
                        SourcePaths = PathHelper.ComputeCommonFolders(project.Files.Select(f => f.FullPath)),
                        Path = project.UniqueName
                    };
                    cppProjects.Add(cppProject);
                }
            }

            return cppProjects;
        }

        readonly DTE2 dte;
        readonly IConfigurationManager configurationManager;
    }
}
