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

namespace OpenCppCoverage.VSPackage
{
    class DynamicVCConfiguration
    {
        //---------------------------------------------------------------------
        public DynamicVCConfiguration(dynamic configuration)
        {
            this.configuration_ = configuration;
            this.DebugSettings = new DynamicVCDebugSettings(configuration_.DebugSettings);

            var compilerTool = GetTool(configuration, "VCCLCompilerTool");
            if (compilerTool != null)
                this.OptionalVCCLCompilerTool = new DynamicVCCLCompilerTool(compilerTool);
        }

        //---------------------------------------------------------------------
        static dynamic GetTool(dynamic configuration, string toolKindToFind)
        {
            foreach (dynamic tool in configuration.Tools)
            {
                if (tool.ToolKind == toolKindToFind)
                    return tool;
            }

            return null;
        }

        //---------------------------------------------------------------------
        public string ConfigurationName
        {
            get
            {
                return configuration_.ConfigurationName;
            }
        }

        //---------------------------------------------------------------------
        public string PlatformName
        {
            get
            {
                return configuration_.Platform.Name;
            }
        }

        //---------------------------------------------------------------------
        public string Evaluate(string str)
        {
            return configuration_.Evaluate(str);
        }

        //---------------------------------------------------------------------
        public DynamicVCDebugSettings DebugSettings { get; }

        //---------------------------------------------------------------------
        public DynamicVCCLCompilerTool OptionalVCCLCompilerTool { get; }

        //---------------------------------------------------------------------
        public string PrimaryOutput
        {
            get
            {
                return configuration_.PrimaryOutput;
            }
        }

        readonly dynamic configuration_;
    }
}
