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

using System.Collections.ObjectModel;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class FilterSettingController
    {
        //---------------------------------------------------------------------
        public FilterSettingController()
        {
            this.AdditionalSourcePatterns = new ObservableCollection<BindableString>();
            this.AdditionalModulePatterns = new ObservableCollection<BindableString>();
            this.ExcludedSourcePatterns = new ObservableCollection<BindableString>();            
            this.ExcludedModulePatterns = new ObservableCollection<BindableString>();
            this.UnifiedDiffs = new ObservableCollection<FilterSettings.UnifiedDiff>();            
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject()
        {
            this.AdditionalSourcePatterns.Clear();
            this.AdditionalModulePatterns.Clear();
            this.ExcludedSourcePatterns.Clear();
            this.ExcludedModulePatterns.Clear();
            this.UnifiedDiffs.Clear();
        }

        //---------------------------------------------------------------------
        public FilterSettings GetSettings()
        {
            return new FilterSettings
            {
                AdditionalSourcePaths = this.AdditionalSourcePatterns.ToStringList(),
                AdditionalModulePaths = this.AdditionalModulePatterns.ToStringList(),
                ExcludedSourcePaths = this.ExcludedSourcePatterns.ToStringList(),
                ExcludedModulePaths = this.ExcludedModulePatterns.ToStringList(),
                UnifiedDiffs = this.UnifiedDiffs
            };
        }

        //---------------------------------------------------------------------
        public ObservableCollection<BindableString> AdditionalSourcePatterns { get; }
        public ObservableCollection<BindableString> AdditionalModulePatterns { get; }
        public ObservableCollection<BindableString> ExcludedSourcePatterns { get; }
        public ObservableCollection<BindableString> ExcludedModulePatterns { get; }
        public ObservableCollection<FilterSettings.UnifiedDiff> UnifiedDiffs { get; }
    }
}
