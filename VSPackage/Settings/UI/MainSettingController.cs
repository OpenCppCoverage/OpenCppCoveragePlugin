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
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class MainSettingController : PropertyChangedNotifier
    {
        readonly IOpenCppCoverageCmdLine openCppCoverageCmdLine;
        readonly ISettingsStorage settingsStorage;
        readonly CoverageRunner coverageRunner;
        readonly IStartUpProjectSettingsBuilder startUpProjectSettingsBuilder;

        string selectedProjectPath;
        string solutionConfigurationName;
        bool displayProgramOutput;
        ProjectSelectionKind kind;

        //---------------------------------------------------------------------
        public MainSettingController(
            ISettingsStorage settingsStorage,
            IOpenCppCoverageCmdLine openCppCoverageCmdLine,
            IStartUpProjectSettingsBuilder startUpProjectSettingsBuilder,
            CoverageRunner coverageRunner)
        {
            this.settingsStorage = settingsStorage;
            this.openCppCoverageCmdLine = openCppCoverageCmdLine;
            this.RunCoverageCommand = new RelayCommand(() => OnRunCoverageCommand());
            this.CloseCommand = new RelayCommand(() =>
            {
                this.CloseWindowEvent?.Invoke(this, EventArgs.Empty);
            });
            this.ResetToDefaultCommand = new RelayCommand(
                () => UpdateStartUpProject(ComputeStartUpProjectSettings(kind)));
            this.BasicSettingController = new BasicSettingController();
            this.FilterSettingController = new FilterSettingController();
            this.ImportExportSettingController = new ImportExportSettingController();
            this.MiscellaneousSettingController = new MiscellaneousSettingController();

            this.coverageRunner = coverageRunner;
            this.startUpProjectSettingsBuilder = startUpProjectSettingsBuilder;
        }

        //---------------------------------------------------------------------
        public void UpdateFields(ProjectSelectionKind kind, bool displayProgramOutput)
        {
            var settings = ComputeStartUpProjectSettings(kind);
            this.UpdateStartUpProject(settings);
            this.selectedProjectPath = settings.ProjectPath;
            this.displayProgramOutput = displayProgramOutput;
            this.solutionConfigurationName = settings.SolutionConfigurationName;
            this.kind = kind;

            var uiSettings = this.settingsStorage.TryLoad(this.selectedProjectPath, this.solutionConfigurationName);

            if (uiSettings != null)
            {
                this.BasicSettingController.UpdateSettings(uiSettings.BasicSettingController);
                this.FilterSettingController.UpdateSettings(uiSettings.FilterSettingController);
                this.ImportExportSettingController.UpdateSettings(uiSettings.ImportExportSettingController);
                this.MiscellaneousSettingController.UpdateSettings(uiSettings.MiscellaneousSettingController);
            }
        }

        //---------------------------------------------------------------------
        StartUpProjectSettings ComputeStartUpProjectSettings(ProjectSelectionKind kind)
        {
            return this.startUpProjectSettingsBuilder.ComputeSettings(kind);
        }

        //---------------------------------------------------------------------
        void UpdateStartUpProject(StartUpProjectSettings settings)
        {
            this.BasicSettingController.UpdateStartUpProject(settings);
            this.FilterSettingController.UpdateStartUpProject();
            this.ImportExportSettingController.UpdateStartUpProject();
            this.MiscellaneousSettingController.UpdateStartUpProject();
        }

        //---------------------------------------------------------------------
        public void SaveSettings()
        {
            var uiSettings = new UserInterfaceSettings
            {
                BasicSettingController = this.BasicSettingController.BuildJsonSettings(),
                FilterSettingController = this.FilterSettingController.Settings,
                ImportExportSettingController = this.ImportExportSettingController.Settings,
                MiscellaneousSettingController = this.MiscellaneousSettingController.Settings
            };
            this.settingsStorage.Save(this.selectedProjectPath, this.solutionConfigurationName, uiSettings);
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
                DisplayProgramOutput = this.displayProgramOutput
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
        public static string CommandLineHeader = "Command line";

        public TabItem SelectedTab
        {
            set
            {
                if (value != null && (string)value.Header == CommandLineHeader)
                    this.CommandLineText = this.openCppCoverageCmdLine.Build(this.GetMainSettings(), "\n");
            }
        }
        //---------------------------------------------------------------------
        void OnRunCoverageCommand()
        {
            this.coverageRunner.RunCoverageOnStartupProject(this.GetMainSettings());
        }

        //---------------------------------------------------------------------
        public EventHandler CloseWindowEvent;

        //---------------------------------------------------------------------
        public ICommand CloseCommand { get; }
        public ICommand RunCoverageCommand { get; }
        public ICommand ResetToDefaultCommand { get; }
    }
}
