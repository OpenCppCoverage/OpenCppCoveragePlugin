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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage.Settings;
using OpenCppCoverage.VSPackage.Settings.UI;
using System.Collections.Generic;
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass()]
    class MainSettingControllerTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void GetMainSettings()
        {
            var controller = new MainSettingController();
            var project = new StartUpProjectSettings.CppProject
            {
                ModulePath = "ModulePath1",
                SourcePaths = new List<string> { "Source1" }
            };
            
            var startUpProjectSettings = new StartUpProjectSettings {
                CppProjects = new List<StartUpProjectSettings.CppProject> {
                    new StartUpProjectSettings.CppProject(),
                    project},
            };

            controller.UpdateStartUpProject(startUpProjectSettings);
            var selectableProject = controller.BasicSettingController.SelectableProjects.First();
            selectableProject.IsSelected = true;

            var settings = controller.GetMainSettings();
            Assert.Equals(project.ModulePath, settings.BasicSettings.ModulePaths);
            Assert.Equals(project.SourcePaths, settings.BasicSettings.SourcePaths);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ResetToDefaultCommand()
        {
            var controller = new MainSettingController();
            var startUpProjectSettings = new StartUpProjectSettings { WorkingDir = "WorkingDir" };
            
            controller.UpdateStartUpProject(startUpProjectSettings);
            controller.BasicSettingController.WorkingDirectory = "WorkingDirectory2";
            var settings = controller.GetMainSettings();

            Assert.Equals(
                controller.BasicSettingController.WorkingDirectory, 
                settings.BasicSettings.WorkingDirectory);
            controller.ResetToDefaultCommand.Execute(null);
            Assert.Equals(
                startUpProjectSettings.WorkingDir, 
                settings.BasicSettings.WorkingDirectory);
        }
    }
}
