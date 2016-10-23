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
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    [ImplementPropertyChanged]
    class MiscellaneousSettingController: PropertyChangedNotifier
    {
        //---------------------------------------------------------------------
        public MiscellaneousSettingController()
        {
            this.LogTypeValues = Enum.GetValues(typeof(MiscellaneousSettings.LogType))
                .Cast<MiscellaneousSettings.LogType>();

            this.OpenCppCoverExe = @"C:\Program Files\OpenCppCoverage\OpenCppCoverage.exe";
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject()
        {
            this.HasConfigFile = false;
            this.OptionalConfigFile = null;
            this.LogTypeValue = MiscellaneousSettings.LogType.Normal;
            this.ContinueAfterCppExceptions = false;
        }

        //---------------------------------------------------------------------
        public MiscellaneousSettings GetSettings()
        {
            return new MiscellaneousSettings
            {
                OptionalConfigFile = this.OptionalConfigFile,
                LogTypeValue = this.LogTypeValue,
                ContinueAfterCppExceptions = this.ContinueAfterCppExceptions,
                OpenCppCoverExe = this.OpenCppCoverExe
            };
        }

        //---------------------------------------------------------------------
        public bool HasConfigFile { get; set; }

        //---------------------------------------------------------------------
        public string OptionalConfigFile { get; set; }

        //---------------------------------------------------------------------
        public MiscellaneousSettings.LogType LogTypeValue { get; set; }

        //---------------------------------------------------------------------
        public bool ContinueAfterCppExceptions { get; set; }

        //---------------------------------------------------------------------
        public string OpenCppCoverExe { get; set; }

        //---------------------------------------------------------------------
        public IEnumerable<MiscellaneousSettings.LogType> LogTypeValues { get; private set; }
    }
}
