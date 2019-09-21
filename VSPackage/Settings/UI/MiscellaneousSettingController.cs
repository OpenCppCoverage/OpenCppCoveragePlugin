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
        public class SettingsData: PropertyChangedNotifier
        {
            //---------------------------------------------------------------------
            string optionalConfigFile;
            public string OptionalConfigFile
            {
                get { return this.optionalConfigFile; }
                set { this.SetField(ref this.optionalConfigFile, value); }
            }

            //---------------------------------------------------------------------
            MiscellaneousSettings.LogType logTypeValue;
            public MiscellaneousSettings.LogType LogTypeValue
            {
                get { return this.logTypeValue; }
                set { this.SetField(ref this.logTypeValue, value); }
            }

            //---------------------------------------------------------------------
            bool continueAfterCppExceptions;
            public bool ContinueAfterCppExceptions
            {
                get { return this.continueAfterCppExceptions; }
                set { this.SetField(ref this.continueAfterCppExceptions, value); }
            }

        }

        //---------------------------------------------------------------------
        public MiscellaneousSettingController()
        {
            this.LogTypeValues = Enum.GetValues(typeof(MiscellaneousSettings.LogType))
                .Cast<MiscellaneousSettings.LogType>();
            this.Settings = new SettingsData();
        }

        //---------------------------------------------------------------------
        public SettingsData Settings { get; private set; }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject()
        {
            this.HasConfigFile = false;
            this.Settings.OptionalConfigFile = null;
            this.Settings.LogTypeValue = MiscellaneousSettings.LogType.Normal;
            this.Settings.ContinueAfterCppExceptions = false;
        }

        //---------------------------------------------------------------------
        public void UpdateSettings(SettingsData settings)
        {
            this.Settings = settings;
            this.HasConfigFile = !string.IsNullOrEmpty(this.Settings.OptionalConfigFile);
        }

        //---------------------------------------------------------------------
        public MiscellaneousSettings GetSettings()
        {
            return new MiscellaneousSettings
            {
                OptionalConfigFile = this.Settings.OptionalConfigFile,
                LogTypeValue = this.Settings.LogTypeValue,
                ContinueAfterCppExceptions = this.Settings.ContinueAfterCppExceptions
            };
        }

        //---------------------------------------------------------------------
        bool hasConfigFile;
        public bool HasConfigFile
        {
            get { return this.hasConfigFile; }
            set
            {
                if (this.SetField(ref this.hasConfigFile, value) && !value)
                    this.Settings.OptionalConfigFile = null;
            }
        }

        //---------------------------------------------------------------------        
        public IEnumerable<MiscellaneousSettings.LogType> LogTypeValues { get; }
    }
}
