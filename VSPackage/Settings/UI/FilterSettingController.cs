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
using System.Collections.ObjectModel;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class FilterSettingController: PropertyChangedNotifier
    {
        //---------------------------------------------------------------------
        public class SettingsData
        {
            public SettingsData()
            {
                this.AdditionalSourcePatterns = new ObservableCollection<BindableString>();
                this.AdditionalModulePatterns = new ObservableCollection<BindableString>();
                this.ExcludedSourcePatterns = new ObservableCollection<BindableString>();
                this.ExcludedModulePatterns = new ObservableCollection<BindableString>();
                this.UnifiedDiffs = new ObservableCollection<FilterSettings.UnifiedDiff>();
            }

            public ObservableCollection<BindableString> AdditionalSourcePatterns { get; }
            public ObservableCollection<BindableString> AdditionalModulePatterns { get; }
            public ObservableCollection<BindableString> ExcludedSourcePatterns { get; }
            public ObservableCollection<BindableString> ExcludedModulePatterns { get; }
            public ObservableCollection<FilterSettings.UnifiedDiff> UnifiedDiffs { get; }
        }


        //---------------------------------------------------------------------
        public FilterSettingController()
        {
            this.Settings = new SettingsData();
        }

        //---------------------------------------------------------------------
        SettingsData settings;
        public SettingsData Settings
        {
            get { return this.settings; }
            private set { this.SetField(ref this.settings, value); }
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject()
        {
            this.Settings.AdditionalSourcePatterns.Clear();
            this.Settings.AdditionalModulePatterns.Clear();
            this.Settings.ExcludedSourcePatterns.Clear();
            this.Settings.ExcludedModulePatterns.Clear();
            this.Settings.UnifiedDiffs.Clear();
        }

        //---------------------------------------------------------------------
        public void UpdateSettings(SettingsData settings)
        {
            this.Settings = settings;
        }

        //---------------------------------------------------------------------
        public FilterSettings GetSettings()
        {
            return new FilterSettings
            {
                AdditionalSourcePaths = this.Settings.AdditionalSourcePatterns.ToStringList(),
                AdditionalModulePaths = this.Settings.AdditionalModulePatterns.ToStringList(),
                ExcludedSourcePaths = this.Settings.ExcludedSourcePatterns.ToStringList(),
                ExcludedModulePaths = this.Settings.ExcludedModulePatterns.ToStringList(),
                UnifiedDiffs = this.Settings.UnifiedDiffs
            };
        }
    }
}
