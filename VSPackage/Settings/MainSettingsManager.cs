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

using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;

namespace OpenCppCoverage.VSPackage.Settings
{
    class MainSettingsManager
    {
        readonly Package package;
        readonly DTE2 dte;

        //---------------------------------------------------------------------
        public MainSettingsManager(Package package, DTE2 dte)
        {
            this.package = package;
            this.dte = dte;
        }

        //---------------------------------------------------------------------
        public void ShowSettingsWindows(CoverageRunner coverageRunner)
        {
            var configurationManager = new ConfigurationManager();
            var settingsBuilder = new StartUpProjectSettingsBuilder(this.dte, configurationManager);

            var window = this.package.FindToolWindow(
                    typeof(SettingToolWindow), 0, true) as SettingToolWindow;
            if (window == null || window.Frame == null)
                throw new NotSupportedException("Cannot create tool window");

            window.Controller.StartUpProjectSettingsBuilder = settingsBuilder;
            window.Controller.CoverageRunner = coverageRunner;
            window.Controller.UpdateStartUpProject();
            var vsEvents = new VsEvents(dte, window);
            var frame = (IVsWindowFrame)window.Frame;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}
