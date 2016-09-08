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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class UnifiedDiffs
    {
        public string UnifiedDiffPath { get; set; }
        public string OptionalRootFolder { get; set; }
    }

    //-------------------------------------------------------------------------
    class FilterSettingController: PropertyChangedNotifier
    {
        //---------------------------------------------------------------------
        public FilterSettingController()
        {
            this.SourcePatterns = new List<BindableString>();
            this.ModulePatterns = new List<BindableString>();
            this.ExcludedSourcePatterns = new ObservableCollection<BindableString>();            
            this.ExcludedModulePatterns = new ObservableCollection<BindableString>();
            this.UnifiedDiffs = new ObservableCollection<UnifiedDiffs>();            
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject(StartUpProjectSettings settings)
        {
            this.SourcePatterns = settings.CppProjects
                .SelectMany(project => project.SourcePaths)
                .Select(path => new BindableString(path))
                .ToList();

            this.ModulePatterns = settings.CppProjects
                .Select(project => new BindableString(project.ModulePath))                
                .ToList();
            this.ExcludedSourcePatterns.Clear();
            this.ExcludedModulePatterns.Clear();
            this.UnifiedDiffs.Clear();
        }

        //---------------------------------------------------------------------
        List<BindableString> sourcePatterns;
        public List<BindableString> SourcePatterns
        {
            get { return this.sourcePatterns; }
            private set { this.SetField(ref this.sourcePatterns, value); }
        }

        //---------------------------------------------------------------------
        List<BindableString> modulePatterns;
        public List<BindableString> ModulePatterns
        {
            get { return this.modulePatterns; }
            private set { this.SetField(ref this.modulePatterns, value); }
        }
        
        public ObservableCollection<BindableString> ExcludedSourcePatterns { get; private set; }        
        public ObservableCollection<BindableString> ExcludedModulePatterns { get; private set; }
        public ObservableCollection<UnifiedDiffs> UnifiedDiffs { get; private set; }
    }
}
