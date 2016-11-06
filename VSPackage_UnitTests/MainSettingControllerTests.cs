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
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Moq;
using OpenCppCoverage.VSPackage.Settings;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
                SourcePaths = new List<string> { "Source1" }
            };

            var startUpProjectSettings = new StartUpProjectSettings {
                CppProjects = new List<StartUpProjectSettings.CppProject> { project, project}};

            var controller = CreateController(startUpProjectSettings, null);
            controller.UpdateStartUpProject();

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
            controller.UpdateStartUpProject();

            controller.BasicSettingController.OptionalWorkingDirectory = "WorkingDirectory2";
            var settings = controller.GetMainSettings();

            Assert.AreEqual(
                controller.BasicSettingController.OptionalWorkingDirectory,
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
            TestHelper.RunInUIhread(() =>
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
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void MiscellaneousSettingControllerHasConfigFile()
        {
            TestHelper.RunInUIhread(() =>
            {
                var controller = new MiscellaneousSettingController();
                controller.HasConfigFile = true;
                controller.OptionalConfigFile = "configFile";

                controller.HasConfigFile = false;
                Assert.IsNull(controller.OptionalConfigFile);
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ExportPathExistError()
        {
            TestHelper.RunInUIhread(() =>
            {
                var controller = new MainSettingController(s => string.Empty);
                var export = AddExport(
                    controller,
                    ImportExportSettings.Type.Binary,
                    Path.GetRandomFileName());

                CheckRunCoverageCommand(controller, true, null);

                export.Path = Assembly.GetExecutingAssembly().Location;
                CheckRunCoverageCommand(controller, false,
                    string.Format(ImportExportSettingController.ExportPathExistError, export.Path));
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void ExportDirectoryNotEmptyError()
        {
            TestHelper.RunInUIhread(() =>
            {
                var controller = new MainSettingController(s => string.Empty);
                var emptyFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(emptyFolder);

                try
                {
                    var export = AddExport(controller, ImportExportSettings.Type.Html, emptyFolder);
                    CheckRunCoverageCommand(controller, true, null);

                    export.Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    CheckRunCoverageCommand(
                        controller, 
                        false,
                        string.Format(ImportExportSettingController.ExportDirectoryNotEmptyError, 
                        export.Path));
                }
                finally
                {
                    Directory.Delete(emptyFolder);
                }
            });
        }

        //---------------------------------------------------------------------
        static MainSettingController CreateController(
            StartUpProjectSettings settings,
            Func<MainSettings, string> buildOpenCppCoverageCmdLine)
        {
            var controller = new MainSettingController(buildOpenCppCoverageCmdLine);
            var builder = new Mock<IStartUpProjectSettingsBuilder>();

            builder.Setup(b => b.ComputeSettings()).Returns(settings);
            controller.StartUpProjectSettingsBuilder = builder.Object;
            return controller;
        }

        //---------------------------------------------------------------------
        static ImportExportSettingController.Export AddExport(
            MainSettingController controller,
            ImportExportSettings.Type type,
            string path)
        {
            var export = new ImportExportSettingController.Export
            {
                Type = type,
                Path = path
            };

            controller.ImportExportSettingController.Exports.Add(export);
            return export;
        }

        //---------------------------------------------------------------------
        static void CheckRunCoverageCommand(
            MainSettingController controller,
            bool expectedCanExecute,
            string expectedRunCoverageErrorToolTip)
        {
            Assert.AreEqual(expectedCanExecute, controller.RunCoverageCommand.CanExecute(null));
            Assert.AreEqual(expectedRunCoverageErrorToolTip, controller.RunCoverageErrorToolTip);
        }
    }
}
