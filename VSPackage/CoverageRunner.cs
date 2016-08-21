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
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OpenCppCoverage.VSPackage.CoverageData;
using OpenCppCoverage.VSPackage.CoverageTree;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenCppCoverage.VSPackage
{
    class CoverageRunner
    {
        public static readonly string ProjectNameTag = " - Project Name: ";
        public static readonly string CommandTag = " - Command: ";
        public static readonly string ArgumentTag = " - Arguments: ";
        public static readonly string WorkingDirTag = " - WorkingDir: ";
        public static readonly string SelectedFolderTag = " - Selected folders: ";
        public static readonly string SelectedModuleTag = " - Selected modules: ";

        //---------------------------------------------------------------------
        public CoverageRunner(
            DTE2 dte,
            IVsWebBrowsingService webBrowsingService,
            SettingsBuilder settingsBuilder,
            ErrorHandler errorHandler,
            OutputWindowWriter outputWindowWriter,
            CoverageTreeManager coverageTreeManager)
        {
            dte_ = dte;
            webBrowsingService_ = webBrowsingService;
            settingsBuilder_ = settingsBuilder;
            errorHandler_ = errorHandler;
            outputWindowWriter_ = outputWindowWriter;
            coverageTreeManager_ = coverageTreeManager;
        }

        //---------------------------------------------------------------------
        public void RunCoverageOnStartupProject()
        {            
            var buildContext = new BuildContext();
            _dispBuildEvents_OnBuildProjConfigDoneEventHandler onBuildDone = 
                (string project, string projectConfig, string platform, string solutionConfig, bool success)
                    => OnBuildProjConfigDone(project, projectConfig, platform, solutionConfig, success, buildContext);

            buildContext.OnBuildDone = onBuildDone;
            var settings = settingsBuilder_.ComputeSettings();
            buildContext.Settings = settings;
            
            dte_.Events.BuildEvents.OnBuildProjConfigDone += onBuildDone;

            LogSettings(settings);
            outputWindowWriter_.WriteLine("Start building " + settings.ProjectName);

            var solutionBuild = dte_.Solution.SolutionBuild;
            solutionBuild.BuildProject(settings.SolutionConfigurationName, settings.ProjectName, false);
        }

        //---------------------------------------------------------------------
        void OnBuildProjConfigDone(
            string project, 
            string projectConfig, 
            string platform, 
            string solutionConfig, 
            bool success, 
            BuildContext buildContext)
        {
            // This method is executed asynchronously and so we need to catch errors.
            errorHandler_.Execute(() =>
                {
                    if (project == buildContext.Settings.ProjectName)
                    {
                        dte_.Events.BuildEvents.OnBuildProjConfigDone -= buildContext.OnBuildDone;
                        outputWindowWriter_.ActivatePane();

                        if (!success)
                            throw new VSPackageException("Build failed.");

                        outputWindowWriter_.WriteLine("Start code coverage...");

                        var settings = buildContext.Settings;
                        CheckSettings(settings);

                        var openCppCoverage = new OpenCppCoverage(outputWindowWriter_);
                        var coveragePath = openCppCoverage.RunCodeCoverage(settings);

                        outputWindowWriter_.WriteLine("Coverage written in " + coveragePath);
                        coverageTreeManager_.ShowTreeCoverage(coveragePath);
                    }
                });
        }

        //---------------------------------------------------------------------        
        void CheckSettings(SettingValue settings)
        {
            if (!File.Exists(settings.Command))
            {
                throw new VSPackageException(
                    string.Format(@"Debugging command ""{0}"" does not exist.", settings.Command));
            }

            if (!string.IsNullOrEmpty(settings.WorkingDir) && !Directory.Exists(settings.WorkingDir))
            {
                throw new VSPackageException(
                    string.Format(@"Debugging working directory ""{0}"" does not exist.", settings.WorkingDir));
            }
        }

        //---------------------------------------------------------------------
        class BuildContext
        {            
            public _dispBuildEvents_OnBuildProjConfigDoneEventHandler OnBuildDone { get; set; }
            public SettingValue Settings { get; set; }
        }

        //---------------------------------------------------------------------
        void LogSettings(SettingValue settings)
        {                     
            outputWindowWriter_.WriteLine("Current Configuration: ");
            outputWindowWriter_.WriteLine(ProjectNameTag + settings.ProjectName);
            outputWindowWriter_.WriteLine(CommandTag + settings.Command);
            outputWindowWriter_.WriteLine(ArgumentTag + settings.Arguments);
            outputWindowWriter_.WriteLine(WorkingDirTag + settings.WorkingDir);
            LogCollection(SelectedFolderTag, settings.SourcePaths);
            LogCollection(SelectedModuleTag, settings.ModulePaths);
        }

        //---------------------------------------------------------------------
        void LogCollection(string name, IEnumerable<string> values)
        {            
            foreach (var v in values)
                outputWindowWriter_.WriteLine(name + v); 
        }

        readonly SettingsBuilder settingsBuilder_;
        readonly DTE2 dte_;
        readonly OutputWindowWriter outputWindowWriter_;
        readonly ErrorHandler errorHandler_;
        readonly IVsWebBrowsingService webBrowsingService_;
        readonly CoverageTreeManager coverageTreeManager_;
    }
}
