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

using System.Collections.Generic;

namespace OpenCppCoverage.VSPackage.Settings
{
    //---------------------------------------------------------------------------
    class MainSettings
    {
        public BasicSettings BasicSettings { get; set; }
        public FilterSettings FilterSettings { get; set; }
        public ImportExportSettings ImportExportSettings { get; set; }
        public MiscellaneousSettings MiscellaneousSettings { get; set; }
        public string SolutionConfigurationName { get; set; }
        public string ProjectName { get; set; }
    }

    //---------------------------------------------------------------------------
    class BasicSettings
    {
        public IEnumerable<string> ModulePaths { get; set; }
        public IEnumerable<string> SourcePaths { get; set; }
        public string WorkingDirectory { get; set; }
        public string Arguments { get; set; }
        public string ProgramToRun { get; set; }
        public bool CompileBeforeRunning { get; set; }
    }

    //---------------------------------------------------------------------------
    class FilterSettings
    {
        public IEnumerable<string> AdditionalModulePaths { get; set; }
        public IEnumerable<string> AdditionalSourcePaths { get; set; }
        public IEnumerable<string> ExcludedModulePaths { get; set; }
        public IEnumerable<string> ExcludedSourcePaths { get; set; }
    }

    //---------------------------------------------------------------------------
    class ImportExportSettings
    {
        //---------------------------------------------------------------------
        public enum Type
        {
            Html,
            Cobertura,
            Binary
        }

        //---------------------------------------------------------------------
        public class Export
        {
            public Type Type { get; set; }
            public string Path { get; set; }
        }

        public IEnumerable<string> InputCoverages { get; set; }
        public IEnumerable<Export> Exports { get; set; }

        public bool CoverChildrenProcesses { get; set; }
        public bool AggregateByFile { get; set; }
    }

    //---------------------------------------------------------------------------
    class MiscellaneousSettings
    {
        public enum LogType
        {
            Normal,
            Quiet,
            Verbose
        }

        public string OptionalConfigFile { get; set; }
        public LogType LogTypeValue { get; set; }
        public bool ContinueAfterCppExceptions { get; set; }
    }
}
