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

using EnvDTE;
using EnvDTE80;
using System;
using System.Runtime.InteropServices;

namespace OpenCppCoverage.VSPackage
{
    class ProjectBuilder
    {
        readonly DTE2 dte;
        readonly ErrorHandler errorHandler;
        readonly OutputWindowWriter outputWindowWriter;

        //---------------------------------------------------------------------
        public ProjectBuilder(
            DTE2 dte,
            ErrorHandler errorHandler,
            OutputWindowWriter outputWindowWriter)
        {
            this.dte = dte;
            this.errorHandler = errorHandler;
            this.outputWindowWriter = outputWindowWriter;
        }

        //---------------------------------------------------------------------
        public void Build(
            string solutionConfigurationName, 
            string projectName, 
            Action<bool> userCallBack)
        {
            var buildHandler = CreateBuildHandler(solutionConfigurationName, projectName, userCallBack);
            this.dte.Events.BuildEvents.OnBuildProjConfigDone += buildHandler;
            
            this.outputWindowWriter.WriteLine(
                "Start building " + projectName 
                + " " + solutionConfigurationName);

            var output = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            if (output != null)
                output.Activate();

            var solutionBuild = this.dte.Solution.SolutionBuild;

            try
            {
                solutionBuild.BuildProject(solutionConfigurationName, projectName, false);
            }
            catch (COMException e)
            {
                throw new VSPackageException(
                    string.Format("Error when building {0} with configuration {1}: {2}",
                        projectName, solutionConfigurationName, e.Message));
            }
        }

        //---------------------------------------------------------------------
        _dispBuildEvents_OnBuildProjConfigDoneEventHandler CreateBuildHandler(
            string solutionConfigurationName,
            string projectName,
            Action<bool> userCallBack)
        {
            var buildContext = new BuildContext();
            _dispBuildEvents_OnBuildProjConfigDoneEventHandler onBuildDone =
                (string project, string projectConfig, string platform, string solutionConfig, bool success)
                    => OnBuildProjConfigDone(
                                project, projectConfig, platform,
                                solutionConfig, success, buildContext);
            buildContext.OnBuildDone = onBuildDone;
            buildContext.UserCallBack = userCallBack;
            buildContext.ProjectName = projectName;
            buildContext.SolutionConfigurationName = solutionConfigurationName;

            return onBuildDone;
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
            this.errorHandler.Execute(() =>
            {
                if (project == buildContext.ProjectName 
                && solutionConfig == buildContext.SolutionConfigurationName)
                {
                    this.dte.Events.BuildEvents.OnBuildProjConfigDone -= buildContext.OnBuildDone;
                    buildContext.UserCallBack(success);
                }
            });
        }

        //---------------------------------------------------------------------
        class BuildContext
        {
            public _dispBuildEvents_OnBuildProjConfigDoneEventHandler OnBuildDone { get; set; }
            public Action<bool> UserCallBack { get; set; }
            public string SolutionConfigurationName { get; set; }
            public string ProjectName { get; set; }
        }
    }
}
