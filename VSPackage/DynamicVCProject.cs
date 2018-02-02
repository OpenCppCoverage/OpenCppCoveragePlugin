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

namespace OpenCppCoverage.VSPackage
{
    class DynamicVCProject
    {
        //---------------------------------------------------------------------
        public DynamicVCProject(dynamic project) 
        {
            project_ = project;
        }

        //---------------------------------------------------------------------
        public List<DynamicVCConfiguration> Configurations
        {
            get
            {
                var configurations = new List<DynamicVCConfiguration>();
                foreach (var configuration in project_.Configurations)
                {
                    try
                    {
                        configurations.Add(new DynamicVCConfiguration(configuration));
                    }
                    catch (System.Exception /*exception*/)
                    {
                        // skip failed configuration
                    }
                }
                return configurations;
            }
        }

        //---------------------------------------------------------------------
        public List<DynamicVCFile> Files
        {
            get
            {
                var files = new List<DynamicVCFile>();
                foreach (var file in project_.Files)
                    files.Add(new DynamicVCFile(file));
                return files;
            }
        }

        readonly dynamic project_;
    }
}
