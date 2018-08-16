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
using Newtonsoft.Json;
using OpenCppCoverage.VSPackage.Helper;
using System;
using System.IO;
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
            this.RunCoverageCommand = new RelayCommand(() => OnRunCoverageCommand());
            this.CloseCommand = new RelayCommand(() => {
                this.CloseWindowEvent?.Invoke(this, EventArgs.Empty);
            });
            this.ResetToDefaultCommand = new RelayCommand(
                () => UpdateStartUpProject(ProjectSelectionKind.StartUpProject));
            this.BasicSettingController = new BasicSettingController();
            this.FilterSettingController = new FilterSettingController();
            this.ImportExportSettingController = new ImportExportSettingController();
            this.MiscellaneousSettingController = new MiscellaneousSettingController();
        }

        public CoverageRunner CoverageRunner { get; set; }
        public IStartUpProjectSettingsBuilder StartUpProjectSettingsBuilder { get; set; }

        string settingsFile = "OpenCppCoverage.settings";

        // for serialize
        public class AllControllers
        {
            public BasicSettingController basicSettingController;
            public FilterSettingController filterSettingController;
            public ImportExportSettingController importExportSettingController;
            public MiscellaneousSettingController miscellaneousSettingController;
        }

        //---------------------------------------------------------------------
        public void SaveSettings()
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(new AllControllers
            {
                basicSettingController = this.BasicSettingController,
                filterSettingController = this.FilterSettingController,
                importExportSettingController = this.ImportExportSettingController,
                miscellaneousSettingController = this.MiscellaneousSettingController
            }));
        }

        //---------------------------------------------------------------------
        public void RecoverSettings(AllControllers allControllers)
        {
            this.BasicSettingController = allControllers.basicSettingController;
            this.FilterSettingController = allControllers.filterSettingController;
            this.ImportExportSettingController = allControllers.importExportSettingController;
            this.MiscellaneousSettingController = allControllers.miscellaneousSettingController;
        }

        //---------------------------------------------------------------------
        public void UpdateSettings(ProjectSelectionKind kind)
        {
            if (this.StartUpProjectSettingsBuilder == null)
                throw new InvalidOperationException("StartUpProjectSettingsBuilder should be set.");

            if (File.Exists(settingsFile))
            {
                AllControllers allControllers = JsonConvert.DeserializeObject<AllControllers>(File.ReadAllText(settingsFile));
                if (allControllers.basicSettingController.CurrentProject.Equals(this.StartUpProjectSettingsBuilder.ComputeSettings(kind).ProjectName))
                {
                    RecoverSettings(allControllers);
                    return;
                }
            }

            UpdateStartUpProject(kind);
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject(ProjectSelectionKind kind)
        {
            var settings = this.StartUpProjectSettingsBuilder.ComputeSettings(kind);
            this.BasicSettingController.UpdateStartUpProject(settings);
            this.FilterSettingController.UpdateStartUpProject();
            this.ImportExportSettingController.UpdateStartUpProject();
            this.MiscellaneousSettingController.UpdateStartUpProject();

            SaveSettings();
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
        public BasicSettingController BasicSettingController { get; set; }
        public FilterSettingController FilterSettingController { get; set; }
        public ImportExportSettingController ImportExportSettingController { get; set; }
        public MiscellaneousSettingController MiscellaneousSettingController { get; set; }

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
                    this.CommandLineText = this.buildOpenCppCoverageCmdLine(this.GetMainSettings());
            }
        }
        //---------------------------------------------------------------------
        void OnRunCoverageCommand()
        {
            this.CoverageRunner.RunCoverageOnStartupProject(this.GetMainSettings());
        }

        //---------------------------------------------------------------------
        public EventHandler CloseWindowEvent;
        public ICommand CloseCommand { get; }
        public ICommand RunCoverageCommand { get; }
        public ICommand ResetToDefaultCommand { get; }
    }
}
