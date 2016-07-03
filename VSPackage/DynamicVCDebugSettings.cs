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
    class DynamicVCDebugSettings
    {
        //---------------------------------------------------------------------
        public DynamicVCDebugSettings(dynamic settings)
        {
            settings_ = settings;
        }

        //---------------------------------------------------------------------
        public string WorkingDirectory
        {
            get
            {
                return settings_.WorkingDirectory;
            }
        }

        //---------------------------------------------------------------------
        public string CommandArguments
        {
            get
            {
                return settings_.CommandArguments;
            }
        }

        //---------------------------------------------------------------------
        public string Command
        {
            get
            {
                return settings_.Command;
            }
        }


        readonly dynamic settings_;
    }
}
