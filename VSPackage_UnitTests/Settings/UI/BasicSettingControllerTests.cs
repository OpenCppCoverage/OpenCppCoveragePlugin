// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2019 OpenCppCoverage
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage.Settings;
using OpenCppCoverage.VSPackage.Settings.UI;
using System.Collections.Generic;

namespace VSPackage_UnitTests.Settings.UI
{
    [TestClass]
    public class BasicSettingControllerTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void BuildJsonSettings()
        {
            var controller = new BasicSettingController();
            var settings = new BasicSettingController.SettingsData 
                    { Data = new BasicSettingController.BasicSettingsData() };
            settings.IsSelectedByProjectPath = new Dictionary<string, bool> 
                    { { "project1", true }, { "project2", false } };
            foreach (var kvp in settings.IsSelectedByProjectPath)
            {
                controller.SelectableProjects.Add(
                    new SelectableProject(new StartUpProjectSettings.CppProject { Path = kvp.Key }));
            }

            controller.UpdateSettings(settings);
            var newSettings = controller.BuildJsonSettings();

            PropertyHelper.CheckPropertiesEqualRecursive(settings, newSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void SpecialSettings()
        {
            var controller = new BasicSettingController();
            var settings = new BasicSettingController.SettingsData
            {
                Data = new BasicSettingController.BasicSettingsData { CompileBeforeRunning = true, OptimizedBuild = true }
            };
            controller.UpdateSettings(settings);
            Assert.IsFalse(controller.HasWorkingDirectory);
            Assert.IsFalse(controller.BasicSettings.CompileBeforeRunning);
            Assert.IsFalse(controller.BasicSettings.OptimizedBuild);

            settings.Data.OptionalWorkingDirectory = "WorkingDirectory";
            controller.IsCompileBeforeRunningEnabled = true;
            controller.IsOptimizedBuildCheckBoxEnabled = true;
            settings.Data.CompileBeforeRunning = true; // Reset by UpdateSettings
            settings.Data.OptimizedBuild = true; // Reset by UpdateSettings

            controller.UpdateSettings(settings);
            Assert.IsTrue(controller.HasWorkingDirectory);
            Assert.IsTrue(controller.BasicSettings.CompileBeforeRunning);
            Assert.IsTrue(controller.BasicSettings.OptimizedBuild);
        }
    }
}
