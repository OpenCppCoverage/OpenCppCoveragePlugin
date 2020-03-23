﻿// OpenCppCoverage is an open source code coverage for C++.
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

using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using OpenCppCoverage.VSPackage.CoverageData;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Settings;
using System;
using System.IO;
using System.Linq;

namespace OpenCppCoverage.VSPackage
{
    class CoverageRunner
    {
        //---------------------------------------------------------------------
        public static readonly string BuilderFailedMsg = "Build failed.";
        public static readonly string InvalidValueForProgramToRun =
            @"Please use a valid value for ""Program to run""";
        public static readonly string InvalidProgramToRunMsg =
            @"File ""{0}"" does not exist. " + InvalidValueForProgramToRun;

        //---------------------------------------------------------------------
        readonly DTE2 dte;
        readonly ProjectBuilder projectBuilder;
        readonly OutputWindowWriter outputWindowWriter;
        readonly CoverageTreeManager coverageTreeManager;
        readonly ICoverageViewManager coverageViewManager;
        readonly CoverageDataDeserializer coverageDataDeserializer;
        readonly ErrorHandler errorHandler;
        readonly OpenCppCoverageRunner openCppCoverageRunner;

        //---------------------------------------------------------------------
        public CoverageRunner(
            DTE2 dte,
            OutputWindowWriter outputWindowWriter,
            CoverageTreeManager coverageTreeManager,
            ProjectBuilder projectBuilder,
            ICoverageViewManager coverageViewManager,
            CoverageDataDeserializer coverageDataDeserializer,
            ErrorHandler errorHandler,
            OpenCppCoverageRunner openCppCoverageRunner)
        {
            this.dte = dte;
            this.outputWindowWriter = outputWindowWriter;
            this.coverageTreeManager = coverageTreeManager;
            this.projectBuilder = projectBuilder;
            this.coverageViewManager = coverageViewManager;
            this.coverageDataDeserializer = coverageDataDeserializer;
            this.errorHandler = errorHandler;
            this.openCppCoverageRunner = openCppCoverageRunner;
        }

        //---------------------------------------------------------------------
        public void RunCoverageOnStartupProject(MainSettings settings)
        {
           this.errorHandler.Execute(() =>
           {
               var basicSettings = settings.BasicSettings;
               if (basicSettings.CompileBeforeRunning)
               {
                   projectBuilder.Build(basicSettings.SolutionConfigurationName, basicSettings.ProjectName,
                       compilationSuccess =>
                       {
                           if (!compilationSuccess)
                               throw new VSPackageException(BuilderFailedMsg);

                           RunCoverage(settings);
                       });
               }
               else
               {
                   RunCoverage(settings);
               }
           });
        }

        //---------------------------------------------------------------------
        void RunCoverage(MainSettings settings)
        {
            outputWindowWriter.ActivatePane();

            if (!File.Exists(settings.BasicSettings.ProgramToRun))
            {
                throw new VSPackageException(
                    string.Format(InvalidProgramToRunMsg, settings.BasicSettings.ProgramToRun));
            }

            System.Threading.Tasks.Task onCoverageFinished;
            var coveragePath = new TemporaryFile();
            try
            {
                AddBinaryOutput(settings.ImportExportSettings, coveragePath);
                onCoverageFinished = openCppCoverageRunner.RunCodeCoverageAsync(settings);
            }
            catch (Exception e)
            {
                coveragePath.Dispose();
                OutputWindowWriter.WriteLine("ERROR: " + e.Message);
                throw;
            }

            onCoverageFinished.ContinueWith(task => 
                OnCoverageFinishedAsync(task, coveragePath, onCoverageFinished).Wait());
        }

        //---------------------------------------------------------------------
        System.Threading.Tasks.Task OnCoverageFinishedAsync(
            System.Threading.Tasks.Task onCoverageFinished,
            TemporaryFile coveragePath,
            System.Threading.Tasks.Task avoidGCCollect)
        {
            return errorHandler.ExecuteAsync(async () =>
            {
                using (coveragePath)
                {
                    var exception = onCoverageFinished.Exception?.InnerExceptions.FirstOrDefault();
                    if (exception != null)
                        throw exception;

                    CoverageRate coverageRate = null;
                    if (File.Exists(coveragePath.Path))
                        coverageRate = BuildCoverageRate(coveragePath.Path);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    OnCoverageRateBuilt(coverageRate, coveragePath.Path);
                }
            });
        }

        //---------------------------------------------------------------------
        void OnCoverageRateBuilt(CoverageRate coverageRate, string coveragePath)
        {
            if (coverageRate == null)
            {
                OutputWindowWriter.WriteLine("ERROR: The execution of the previous line failed.");
                OutputWindowWriter.WriteLine(" Please execute the previous line in a promt command to have more information about the issue.");
                throw new VSPackageException("Cannot generate coverage. See output pane for more information");                
            }
            OutputWindowWriter.WriteLine("COVERAGE: Computing finished\u0006");
            OutputWindowWriter.WriteLine(" Result written in \"" + coveragePath + "\"");

            coverageTreeManager.ShowTreeCoverage(this.dte, this.coverageViewManager, coverageRate);
            this.coverageViewManager.CoverageRate = coverageRate;
        }

        //---------------------------------------------------------------------        
        void AddBinaryOutput(
            ImportExportSettings importExportSettings, 
            TemporaryFile coveragePath)
        {
            var exports = importExportSettings.Exports.ToList();
            exports.Add(new ImportExportSettings.Export
            {
                Type = ImportExportSettings.Type.Binary,
                Path = coveragePath.Path
            });
            importExportSettings.Exports = exports;
        }

        //---------------------------------------------------------------------        
        CoverageRate BuildCoverageRate(string coveragePath)
        {
            using (var stream = new FileStream(coveragePath.ToString(), FileMode.Open))
            {
                var coverageResult = this.coverageDataDeserializer.Deserialize(stream);
                var coverageRateBuilder = new CoverageRateBuilder.CoverageRateBuilder();

                return coverageRateBuilder.Build(coverageResult);
            }
        }
    }
}
