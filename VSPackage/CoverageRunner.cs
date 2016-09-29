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
using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Settings;
using System.IO;
using System.Linq;

namespace OpenCppCoverage.VSPackage
{
    class CoverageRunner
    {
        public static readonly string ProjectNameTag = " - Project Name: ";

        //---------------------------------------------------------------------
        public CoverageRunner(
            DTE2 dte,
            ErrorHandler errorHandler,
            OutputWindowWriter outputWindowWriter,
            CoverageTreeManager coverageTreeManager)
        {
            dte_ = dte;
            errorHandler_ = errorHandler;
            outputWindowWriter_ = outputWindowWriter;
            coverageTreeManager_ = coverageTreeManager;
        }

        //---------------------------------------------------------------------
        public void RunCoverageOnStartupProject(MainSettings settings)
        {            
            var buildContext = new BuildContext();
            _dispBuildEvents_OnBuildProjConfigDoneEventHandler onBuildDone = 
                (string project, string projectConfig, string platform, string solutionConfig, bool success)
                    => OnBuildProjConfigDone(project, projectConfig, platform, solutionConfig, success, buildContext);

            buildContext.OnBuildDone = onBuildDone;
            buildContext.Settings = settings;
            
            dte_.Events.BuildEvents.OnBuildProjConfigDone += onBuildDone;
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
                        CheckSettings(settings.BasicSettings);

                        var coveragePath = AddBinaryOutput(settings.ImportExportSettings);
                        var openCppCoverage = new OpenCppCoverage(outputWindowWriter_);

                        openCppCoverage.RunCodeCoverage(settings);

                        if (!File.Exists(coveragePath))
                            throw new VSPackageException("Cannot generate coverage. See output pane for more information.");
                        outputWindowWriter_.WriteLine("Coverage written in " + coveragePath);
                        coverageTreeManager_.ShowTreeCoverage(coveragePath);
                    }
                });
        }

        //---------------------------------------------------------------------        
        string AddBinaryOutput(ImportExportSettings importExportSettings)
        {
            var coveragePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var exports = importExportSettings.Exports.ToList();
            exports.Add(new ImportExportSettings.Export
            {
                Type = ImportExportSettings.Type.Binary,
                Path = coveragePath
            });
            importExportSettings.Exports = exports;
            return coveragePath;
        }

        //---------------------------------------------------------------------        
        void CheckSettings(BasicSettings settings)
        {
            if (!File.Exists(settings.ProgramToRun))
            {
                throw new VSPackageException(
                    string.Format(@"Debugging command ""{0}"" does not exist.", settings.ProgramToRun));
            }
        }

        //---------------------------------------------------------------------
        class BuildContext
        {            
            public _dispBuildEvents_OnBuildProjConfigDoneEventHandler OnBuildDone { get; set; }
            public MainSettings Settings { get; set; }
        }

        readonly DTE2 dte_;
        readonly OutputWindowWriter outputWindowWriter_;
        readonly ErrorHandler errorHandler_;
        readonly CoverageTreeManager coverageTreeManager_;
    }
}
