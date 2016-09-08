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
using System;
using System.Windows.Input;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    //-------------------------------------------------------------------------
    class MainSettingController
    {
        StartUpProjectSettings settings;

        //---------------------------------------------------------------------
        public MainSettingController()
        {
            this.RunCoverageCommand = new RelayCommand(() => { });
            this.CancelCommand = new RelayCommand(() => {
                this.CloseWindowEvent?.Invoke(this, EventArgs.Empty);
            });
            this.ResetToDefaultCommand = new RelayCommand(() => { });
            this.BasicSettingController = new BasicSettingController();
            this.FilterSettingController = new FilterSettingController();
            this.ImportExportSettingController = new ImportExportSettingController();
            this.MiscellaneousSettingController = new MiscellaneousSettingController();
        }

        //---------------------------------------------------------------------
        public void UpdateStartUpProject(StartUpProjectSettings settings)
        {
            this.settings = settings;
        }

        //---------------------------------------------------------------------
        public StartUpProjectSettings GetSettings()
        {
            return this.settings;
        }

        //---------------------------------------------------------------------
        public BasicSettingController BasicSettingController { get; private set; }
        public FilterSettingController FilterSettingController { get; private set; }
        public ImportExportSettingController ImportExportSettingController { get; private set; }
        public MiscellaneousSettingController MiscellaneousSettingController { get; private set; }

        public EventHandler CloseWindowEvent;

        public string CommandLineText { get; private set; }
        public ICommand RunCoverageCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand ResetToDefaultCommand { get; private set; }
    }
}
