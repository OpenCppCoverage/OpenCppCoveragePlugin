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
using Microsoft.VisualStudio.Shell.Interop;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;

namespace OpenCppCoverage.VSPackage.Settings
{
    class MainWindowsManager
    {
        readonly IWindowFinder windowFinder;
        readonly DTE2 dte;
        readonly CoverageRunner coverageRunner;
        readonly StartUpProjectSettingsBuilder settingsBuilder;
        readonly OpenCppCoverageCmdLine openCppCoverageCmdLine;

        //---------------------------------------------------------------------
        public MainWindowsManager(
            IWindowFinder windowFinder,
            DTE2 dte,
            CoverageRunner coverageRunner,
            StartUpProjectSettingsBuilder settingsBuilder,
            OpenCppCoverageCmdLine openCppCoverageCmdLine)
        {
            this.windowFinder = windowFinder;
            this.dte = dte;
            this.coverageRunner = coverageRunner;
            this.settingsBuilder = settingsBuilder;
            this.openCppCoverageCmdLine = openCppCoverageCmdLine;
        }

        //---------------------------------------------------------------------
        SettingToolWindow ConfigureSettingsWindows(
            ProjectSelectionKind kind,
            bool displayProgramOutput)
        {
            var window = this.windowFinder.FindToolWindow<SettingToolWindow>();

            var controller = new MainSettingController(
                new SettingsStorage(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
                this.openCppCoverageCmdLine);


            window.Init(controller);
            // Inject via properties because these objects require DTE which is not
            // available to MainSettingController constructor
            window.Controller.StartUpProjectSettingsBuilder = this.settingsBuilder;
            window.Controller.CoverageRunner = this.coverageRunner;
            window.Controller.UpdateFields(kind, displayProgramOutput);
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