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

using GalaSoft.MvvmLight.Command;
using OpenCppCoverage.VSPackage.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class MainSettingController: PropertyChangedNotifier
    {
        readonly Func<MainSettings, string> buildOpenCppCoverageCmdLine;

        //---------------------------------------------------------------------
        public MainSettingController(
            Func<MainSettings, string> buildOpenCppCoverageCmdLine)
        {
            this.buildOpenCppCoverageCmdLine = buildOpenCppCoverageCmdLine;
            this.runCoverageCommand = new RelayCommand(
                () => OnRunCoverageCommand(),
                () => string.IsNullOrEmpty(this.RunCoverageErrorToolTip));

            this.CloseCommand = new RelayCommand(() => {
                this.CloseWindowEvent?.Invoke(this, EventArgs.Empty);
            });
            this.ResetToDefaultCommand = new RelayCommand(UpdateStartUpProject);
            this.BasicSettingController = new BasicSettingController();
            this.FilterSettingController = new FilterSettingController();
            this.ImportExportSettingController = new ImportExportSettingController();
            this.ImportExportSettingController.ErrorsChanged += ErrorsChangedHandler;
            this.MiscellaneousSettingController = new MiscellaneousSettingController();
        }

        public CoverageRunner CoverageRunner { get; set; }
        public IStartUpProjectSettingsBuilder StartUpProjectSettingsBuilder { get; set; }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject()
        {
            if (this.StartUpProjectSettingsBuilder == null)
                throw new InvalidOperationException("StartUpProjectSettingsBuilder should be set.");

            var settings = this.StartUpProjectSettingsBuilder.ComputeSettings();
            this.BasicSettingController.UpdateStartUpProject(settings);
            this.FilterSettingController.UpdateStartUpProject();
            this.ImportExportSettingController.UpdateStartUpProject();
            this.MiscellaneousSettingController.UpdateStartUpProject();
        }

        //---------------------------------------------------------------------
        public MainSettings GetMainSettings()
        {
            return new MainSettings
            {
                BasicSettings = this.BasicSettingController.GetSettings(),
                FilterSettings = this.FilterSettingController.GetSettings(),
                ImportExportSettings = this.ImportExportSettingController.GetSettings(),
                MiscellaneousSettings = this.MiscellaneousSettingController.GetSettings(),
            };
        }

        //---------------------------------------------------------------------
        public BasicSettingController BasicSettingController { get; }
        public FilterSettingController FilterSettingController { get; }
        public ImportExportSettingController ImportExportSettingController { get; }
        public MiscellaneousSettingController MiscellaneousSettingController { get; }

        //---------------------------------------------------------------------
        string commandLineText;
        public string CommandLineText
        {
            get { return this.commandLineText; }
            private set { this.SetField(ref this.commandLineText, value); }
        }

        //---------------------------------------------------------------------
        string runCoverageErrorToolTip;
        public string RunCoverageErrorToolTip
        {
            get { return this.runCoverageErrorToolTip; }
            private set { this.SetField(ref this.runCoverageErrorToolTip, value); }
        }

        //---------------------------------------------------------------------
        public static string CommandLineHeader = "Command line";

        public TabItem SelectedTab
        {
            set
            {
                if (value != null && (string)value.Header == CommandLineHeader)
                    this.CommandLineText = this.buildOpenCppCoverageCmdLine(this.GetMainSettings());
            }
        }
        //---------------------------------------------------------------------
        void OnRunCoverageCommand()
        {
            this.ImportExportSettingController.UpdateErrorsStatus();

            if (this.runCoverageCommand.CanExecute(null))
                this.CoverageRunner.RunCoverageOnStartupProject(this.GetMainSettings());
        }

        //---------------------------------------------------------------------
        void ErrorsChangedHandler(object sender, IEnumerable<string> errors)
        {
            this.RunCoverageErrorToolTip = errors.FirstOrDefault();
            this.runCoverageCommand.RaiseCanExecuteChanged();
        }

        //---------------------------------------------------------------------
        readonly RelayCommand runCoverageCommand;
        public ICommand RunCoverageCommand
        {
            get { return runCoverageCommand; }
        }

        public EventHandler CloseWindowEvent;
        public ICommand CloseCommand { get; }
        public ICommand ResetToDefaultCommand { get; }
    }
}
