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

using EnvDTE80;
using OpenCppCoverage.VSPackage.CoverageData;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Settings;
using System.IO;
using System.Linq;

namespace OpenCppCoverage.VSPackage
{
    class CoverageRunner
    {
        readonly ProjectBuilder projectBuilder;
        readonly OutputWindowWriter outputWindowWriter;
        readonly CoverageTreeManager coverageTreeManager;
        readonly CoverageViewCreationListener coverageViewCreationListener;
        readonly CoverageDataDeserializer coverageDataDeserializer;
        readonly ErrorHandler errorHandler;

        //---------------------------------------------------------------------
        public CoverageRunner(
            DTE2 dte,
            OutputWindowWriter outputWindowWriter,
            CoverageTreeManager coverageTreeManager,
            ProjectBuilder projectBuilder,
            CoverageViewCreationListener coverageViewCreationListener,
            CoverageDataDeserializer coverageDataDeserializer,
            ErrorHandler errorHandler)
        {
            this.outputWindowWriter = outputWindowWriter;
            this.coverageTreeManager = coverageTreeManager;
            this.projectBuilder = projectBuilder;
            this.coverageViewCreationListener = coverageViewCreationListener;
            this.coverageDataDeserializer = coverageDataDeserializer;
            this.errorHandler = errorHandler;
        }

        //---------------------------------------------------------------------
        public void RunCoverageOnStartupProject(MainSettings settings)
        {
           this.errorHandler.Execute(() =>
           {
               if (settings.BasicSettings.CompileBeforeRunning)
               {
                   projectBuilder.Build(settings.SolutionConfigurationName, settings.ProjectName,
                       compilationSuccess =>
                       {
                           if (!compilationSuccess)
                               throw new VSPackageException("Build failed.");

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
            outputWindowWriter.WriteLine("Start computing code coverage...");

            if (!File.Exists(settings.BasicSettings.ProgramToRun))
            {
                throw new VSPackageException(
                    string.Format(@"File ""{0}"" does not exist. " 
                    + @"Please use a valid value for ""Program to run""", 
                    settings.BasicSettings.ProgramToRun));
            }

            var coveragePath = AddBinaryOutput(settings.ImportExportSettings);
            var openCppCoverage = new OpenCppCoverage(outputWindowWriter);

            openCppCoverage.RunCodeCoverage(settings);

            if (!File.Exists(coveragePath))
            {
                outputWindowWriter.WriteLine("The execution of the previous line failed." +
                    " Please execute the previous line in a promt" + 
                    " command to have more information about the issue.");
                throw new VSPackageException("Cannot generate coverage. See output pane for more information");                
            }
            outputWindowWriter.WriteLine("Coverage written in " + coveragePath);

            try
            {
                var coverageRate = BuildCoverageRate(coveragePath);
                coverageTreeManager.ShowTreeCoverage(coverageRate);
                this.coverageViewCreationListener.CoverageRate = coverageRate;
            }
            finally
            {
                File.Delete(coveragePath);
            }
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
