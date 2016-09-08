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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenCppCoverage.VSPackage.Settings
{
    class StartUpProjectSettingsBuilder
    {
        //---------------------------------------------------------------------
        public StartUpProjectSettingsBuilder(
            Solution2 solution, 
            IConfigurationManager configurationManager)
        {
            solution_ = solution;
            configurationManager_ = configurationManager;
        }

        //---------------------------------------------------------------------
        public StartUpProjectSettings ComputeSettings()
        {
            var projects = GetProjects();
            var startupProject = GetStartupProject(projects);
            var startupConfiguration = configurationManager_.GetConfiguration(startupProject);
            var debugSettings = new DynamicVCDebugSettings(startupConfiguration.DebugSettings);

            if (debugSettings == null)
                throw new Exception("DebugSettings is null");
            var settings = new StartUpProjectSettings
            {
                WorkingDir = startupConfiguration.Evaluate(debugSettings.WorkingDirectory),
                Arguments = startupConfiguration.Evaluate(debugSettings.CommandArguments),
                Command = startupConfiguration.Evaluate(debugSettings.Command),
                SolutionConfigurationName = configurationManager_.GetSolutionConfigurationName(),
                ProjectName = startupProject.UniqueName,
                CppProjects = BuildCppProject(configurationManager_, projects)
            };
            
            return settings;                                                 
        }

        //---------------------------------------------------------------------
        List<ExtendedProject> GetProjects()
        {
            var projects = new List<ExtendedProject>();
            
            foreach (Project project in solution_.Projects)
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
        ExtendedProject GetStartupProject(List<ExtendedProject> projects)
        {
            var startupProjects = GetStartupProjects(projects);

            if (startupProjects.Count == 0)
                throw new VSPackageException("No C++ startup project found.");
            
            if (startupProjects.Count != 1)
            {
                var error = new StringBuilder();

                error.Append("You cannot run OpenCppCoverage for several projects:\n");
                foreach (var project in startupProjects)
                    error.Append(" - " + project.UniqueName + "\n");
                throw new VSPackageException(error.ToString());
            }

            return startupProjects[0];
        }

        //---------------------------------------------------------------------
        List<ExtendedProject> GetStartupProjects(List<ExtendedProject> projects)
        {            
            var startupProjectsNames = solution_.SolutionBuild.StartupProjects as object[];

            if (startupProjectsNames == null)
                throw new VSPackageException("Cannot get startup projects.");

            var startupProjectsSet = new HashSet<String>();
            foreach (String projectName in startupProjectsNames)
                startupProjectsSet.Add(projectName);

            return projects.Where(p => startupProjectsSet.Contains(p.UniqueName)).ToList();
        }

        //---------------------------------------------------------------------
        static IEnumerable<StartUpProjectSettings.CppProject> BuildCppProject(
            IConfigurationManager configurationManager,
            List<ExtendedProject> projects)
        {
            var cppProjects = new List<StartUpProjectSettings.CppProject>();

            foreach (var project in projects)
            {
                var configuration = configurationManager.FindConfiguration(project);

                if (configuration != null)
                {
                    var cppProject = new StartUpProjectSettings.CppProject()
                    {
                        ModulePath = configuration.PrimaryOutput,
                        SourcePaths = ComputeCommonFolders(project.Files.Select(f => f.FullPath)),
                        Name = project.UniqueName
                    };
                    cppProjects.Add(cppProject);
                }
            }

            return cppProjects;
        }

        //---------------------------------------------------------------------       
        static IEnumerable<string> ComputeCommonFolders(IEnumerable<string> projectFilePaths)
        {
            var commonFolders = new List<string>();

            foreach (var path in projectFilePaths)
                commonFolders.Add(Path.GetDirectoryName(path) + Path.DirectorySeparatorChar);
            commonFolders.Sort();
            int index = 0;
            string previousFolder = null;

            while (index < commonFolders.Count)
            {
                string folder = commonFolders[index];

                if (previousFolder != null && folder.StartsWith(previousFolder))
                    commonFolders.RemoveAt(index);
                else
                {
                    previousFolder = folder;
                    ++index;
                }
            }

            return commonFolders;
        }

        readonly Solution2 solution_;
        readonly IConfigurationManager configurationManager_;
    }
}
