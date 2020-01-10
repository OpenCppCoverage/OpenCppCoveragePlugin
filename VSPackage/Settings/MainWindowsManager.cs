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

using Microsoft.VisualStudio.Shell.Interop;
using OpenCppCoverage.VSPackage.Settings.UI;

namespace OpenCppCoverage.VSPackage.Settings
{
    class MainWindowsManager
    {
        readonly IWindowFinder windowFinder;
        readonly MainSettingController mainSettingController;

        //---------------------------------------------------------------------
        public MainWindowsManager(IWindowFinder windowFinder, MainSettingController mainSettingController)
        {
            this.windowFinder = windowFinder;
            this.mainSettingController = mainSettingController;
        }

        //---------------------------------------------------------------------
        SettingToolWindow ConfigureSettingsWindows(
            ProjectSelectionKind kind,
            bool displayProgramOutput)
        {
            this.mainSettingController.UpdateFields(kind, displayProgramOutput);
            var window = this.windowFinder.FindToolWindow<SettingToolWindow>();
            window.Init(this.mainSettingController);

            return window;
        }

        //---------------------------------------------------------------------
        public void OpenSettingsWindow(ProjectSelectionKind kind)
        {
            var window = ConfigureSettingsWindows(kind, true);
            var frame = (IVsWindowFrame)window.Frame;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }

        //---------------------------------------------------------------------
        public void RunCoverage(ProjectSelectionKind kind)
        {
            var window = ConfigureSettingsWindows(kind, false);
            window.Controller.RunCoverageCommand.Execute(null);
        }
    }
}