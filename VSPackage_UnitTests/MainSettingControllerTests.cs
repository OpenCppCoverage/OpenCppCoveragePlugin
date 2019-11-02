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
using Moq;
using OpenCppCoverage.VSPackage.Settings;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass]
    public class MainSettingControllerTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void GetMainSettings()
        {
            var project = new StartUpProjectSettings.CppProject
            {
                ModulePath = "ModulePath1",
                SourcePaths = new List<string> { "Source1" },
                Path = "Path"
            };

            var startUpProjectSettings = new StartUpProjectSettings {
                CppProjects = new List<StartUpProjectSettings.CppProject> { project, project}};

            var controller = CreateController(startUpProjectSettings, null);
            controller.UpdateFields(ProjectSelectionKind.StartUpProject, true);

            var selectableProject = controller.BasicSettingController.SelectableProjects.First();
            selectableProject.IsSelected = false;

            var settings = controller.GetMainSettings();
            Assert.AreEqual(project.ModulePath, settings.BasicSettings.ModulePaths.Single());
            CollectionAssert.AreEqual(
                project.SourcePaths.ToList(),
                settings.BasicSettings.SourcePaths.ToList());
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ResetToDefaultCommand()
        {
            var startUpProjectSettings = new StartUpProjectSettings
            {
                WorkingDir = "WorkingDir",
                CppProjects = new List<StartUpProjectSettings.CppProject>()
            };

            var controller = CreateController(startUpProjectSettings, null);
            controller.UpdateFields(ProjectSelectionKind.StartUpProject, true);

            controller.BasicSettingController.BasicSettings.OptionalWorkingDirectory = "WorkingDirectory2";
            var settings = controller.GetMainSettings();

            Assert.AreEqual(
                controller.BasicSettingController.BasicSettings.OptionalWorkingDirectory,
                settings.BasicSettings.WorkingDirectory);

            controller.ResetToDefaultCommand.Execute(null);
            settings = controller.GetMainSettings();
            Assert.AreEqual(
                startUpProjectSettings.WorkingDir,
                settings.BasicSettings.WorkingDirectory);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void CommandLineText()
        {
            string commandLine = "commandLine";
            var startUpProjectSettings = new StartUpProjectSettings()
            {
                CppProjects = new List<StartUpProjectSettings.CppProject>()
            };

            var controller = CreateController(startUpProjectSettings, settings => { return commandLine; });
            Assert.IsNull(controller.CommandLineText);

            controller.SelectedTab = new System.Windows.Controls.TabItem();
            Assert.IsNull(controller.CommandLineText);

            controller.SelectedTab = new System.Windows.Controls.TabItem()
                { Header = MainSettingController.CommandLineHeader };
            Assert.AreEqual(commandLine, controller.CommandLineText);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void MiscellaneousSettingControllerHasConfigFile()
        {
            var controller = new MiscellaneousSettingController();
            controller.HasConfigFile = true;
            controller.Settings.OptionalConfigFile = "configFile";

            controller.HasConfigFile = false;
            Assert.IsNull(controller.Settings.OptionalConfigFile);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void BasicSaveLoad()
        {
            var settings = new StartUpProjectSettings
            {
                CppProjects = new List<StartUpProjectSettings.CppProject>(),
                ProjectPath = "ProjectPath",
                SolutionConfigurationName = "SolutionConfigurationName"
            };
            var settingsStorage = new Mock<ISettingsStorage>();
            var builder = new Mock<IStartUpProjectSettingsBuilder>();
            builder.Setup(b => b.ComputeSettings(ProjectSelectionKind.SelectedProject)).Returns(settings);
            var controller = CreateController(settings, null, builder, settingsStorage.Object);

            controller.UpdateFields(ProjectSelectionKind.SelectedProject, true);
            controller.SaveSettings();
            settingsStorage.Verify(
                s => s.Save(
                    settings.ProjectPath,
                    settings.SolutionConfigurationName,
                    It.IsAny<UserInterfaceSettings>()));

            settingsStorage.Verify(s => s.TryLoad(settings.ProjectPath, settings.SolutionConfigurationName));
            controller.UpdateFields(ProjectSelectionKind.SelectedProject, true);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void FullSaveLoad()
        {
            using (var folder = new TemporayPath())
            {
                var settings = new StartUpProjectSettings
                {
                    CppProjects = new List<StartUpProjectSettings.CppProject> {
                        new StartUpProjectSettings.CppProject{ Path = "path"}
                    },
                };
                var settingsStorage = new SettingsStorage(folder.Path);
                var builder = new Mock<IStartUpProjectSettingsBuilder>();

                var controller = CreateController(settings, null, builder, settingsStorage);
                controller.UpdateFields(ProjectSelectionKind.StartUpProject, true);
                FillController(controller);
                controller.SaveSettings();

                var controller2 = CreateController(settings, null, builder, settingsStorage);
                controller2.UpdateFields(ProjectSelectionKind.StartUpProject, true);

                CheckRecursiveEqual(controller, controller2, c => c.BasicSettingController);
                CheckRecursiveEqual(controller, controller2, c => c.ImportExportSettingController);
                CheckRecursiveEqual(controller, controller2, c => c.MiscellaneousSettingController);
                CheckRecursiveEqual(controller, controller2, c => c.FilterSettingController);
            }
        }

        //---------------------------------------------------------------------
        static MainSettingController CreateController(
            StartUpProjectSettings settings,
            Func<MainSettings, string> buildOpenCppCoverageCmdLine,
            Mock<IStartUpProjectSettingsBuilder> builder,
            ISettingsStorage settingsStorage)
        {
            var controller = new MainSettingController(settingsStorage, buildOpenCppCoverageCmdLine);

            builder.Setup(b => b.ComputeSettings(ProjectSelectionKind.StartUpProject)).Returns(settings);
            controller.StartUpProjectSettingsBuilder = builder.Object;
            return controller;
        }

        //---------------------------------------------------------------------
        static MainSettingController CreateController(
            StartUpProjectSettings settings,
            Func<MainSettings, string> buildOpenCppCoverageCmdLine)
        {
            return CreateController(
                settings,
                buildOpenCppCoverageCmdLine,
                new Mock<IStartUpProjectSettingsBuilder>(),
                new Mock<ISettingsStorage>().Object);
        }
        //---------------------------------------------------------------------
        static void FillController(MainSettingController controller)
        {
            SetPropertiesToNonDefaultValue(controller.BasicSettingController.BasicSettings);
            foreach (var p in controller.BasicSettingController.SelectableProjects)
                SetPropertiesToNonDefaultValue(p);

            SetPropertiesToNonDefaultValue(controller.ImportExportSettingController.Settings);
            SetPropertiesToNonDefaultValue(controller.MiscellaneousSettingController.Settings);
            SetPropertiesToNonDefaultValue(controller.FilterSettingController.Settings);

            // Make properties consistent as we set settings to a not null value.
            controller.MiscellaneousSettingController.HasConfigFile = true;
            controller.BasicSettingController.HasWorkingDirectory = true;

            // Cannnot be set at true because IsCompileBeforeRunningEnabled=false
            controller.BasicSettingController.BasicSettings.CompileBeforeRunning = false;
        }

        //---------------------------------------------------------------------
        static void CheckRecursiveEqual<T>(
            MainSettingController controller1,
            MainSettingController controller2,
            Func<MainSettingController, T> getProperty)
        {
            PropertyHelper.CheckPropertiesEqualRecursive(getProperty(controller1), getProperty(controller2));
        }

        //---------------------------------------------------------------------
        static void SetPropertiesToNonDefaultValue<T>(T value)
        {
            PropertyHelper.SetPropertiesValue(value, new Dictionary<Type, Func<object>>
            {
                {typeof(string), () => Path.GetRandomFileName() },
                {typeof(bool), () => true },
                {typeof(MiscellaneousSettings.LogType), () => MiscellaneousSettings.LogType.Quiet },
            });
        }
    }
}
