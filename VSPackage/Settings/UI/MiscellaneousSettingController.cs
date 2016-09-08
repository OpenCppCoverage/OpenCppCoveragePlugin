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

using OpenCppCoverage.VSPackage.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class MiscellaneousSettingController: PropertyChangedNotifier
    {
        public enum LogType
        {
            Normal,
            Quiet,
            Verbose
        }

        //---------------------------------------------------------------------
        public MiscellaneousSettingController()
        {
            this.LogTypeValues = Enum.GetValues(typeof(LogType)).Cast<LogType>();
        }

        //---------------------------------------------------------------------
        bool hasConfigFile;
        public bool HasConfigFile
        {
            get { return this.hasConfigFile; }
            set
            {
                if (this.SetField(ref this.hasConfigFile, value) && !value)
                    this.OptionalConfigFile = null;
            }
        }

        //---------------------------------------------------------------------
        string optionalConfigFile;
        public string OptionalConfigFile
        {
            get { return this.optionalConfigFile; }
            set { this.SetField(ref this.optionalConfigFile, value); }
        }

        //---------------------------------------------------------------------
        public LogType LogTypeValue { get; set; }
        public IEnumerable<LogType> LogTypeValues { get; private set; }
        public bool ContinueAfterCppExceptions { get; set; }
    }
}
