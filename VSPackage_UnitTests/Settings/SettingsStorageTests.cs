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
using OpenCppCoverage.VSPackage;
using OpenCppCoverage.VSPackage.Settings;
using OpenCppCoverage.VSPackage.Settings.UI;
using System.IO;

namespace VSPackage_UnitTests.Settings
{
    [TestClass]
    public class SettingsStorageTests
    {
        TemporayPath path;
        UserInterfaceSettings settings;

        //---------------------------------------------------------------------
        [TestInitialize]
        public void TestInitialize()
        {
            this.path = new TemporayPath();
            this.settings = new UserInterfaceSettings();
            this.settings.BasicSettingController = new BasicSettingController.SettingsData();
            this.settings.BasicSettingController.Data = new BasicSettingController.BasicSettingsData();
            this.settings.BasicSettingController.Data.ProgramToRun = "programToRun";
        }

        //---------------------------------------------------------------------
        [TestCleanup]
        public void TestCleanup()
        {
            this.path.Dispose();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void SaveLoad()
        {
            var settingsStorage = new SettingsStorage(null);
            const string projectName = "Project.csproj";
            const string configurationName = "Configuration";
            var projectPath = Path.Combine(this.path.Path, projectName);
            var fullPath = settingsStorage.Save(projectPath, configurationName, this.settings);

            var expectedPath = Path.Combine(
                    this.path.Path, SettingsStorage.OpenCppCov,
                    Path.GetFileNameWithoutExtension(projectName), configurationName + ".json");

            Assert.AreEqual(expectedPath, fullPath);

            var loadedSettings = settingsStorage.TryLoad(projectPath, configurationName);
            AssertEqual(this.settings, loadedSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void SaveLoadNoProject()
        {
            var settingsStorage = new SettingsStorage(this.path.Path);
            var fullPath = settingsStorage.Save(null, null, this.settings);

            var expectedPath = Path.Combine(
                    this.path.Path, SettingsStorage.ApplicationDataSection, 
                    SettingsStorage.NoProjectConfigName + ".json");

            Assert.AreEqual(expectedPath, fullPath);

            var loadedSettings = settingsStorage.TryLoad(null, null);
            AssertEqual(this.settings, loadedSettings);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void InvalidFileNameChars()
        {
            var settingsStorage = new SettingsStorage(this.path.Path);
            var fullPath = settingsStorage.Save(
                Path.Combine(this.path.Path, "Project"),
                "Con|fig>ura<tion", this.settings);

            Assert.IsTrue(fullPath.EndsWith("Con_fig_ura_tion.json"));
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void TryLoadNotFound()
        {
            var settingsStorage = new SettingsStorage(null);

            Assert.IsNull(settingsStorage.TryLoad("FolderNotFound", null));            
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void InvalidJson()
        {
            var settingsStorage = new SettingsStorage(this.path.Path);
            var fullPath = settingsStorage.Save(null, null, this.settings);

            Assert.IsNotNull(settingsStorage.TryLoad(null, null));

            File.AppendAllText(fullPath, "InvalidJson");
            TestHelper.AssertThrows<VSPackageException>(() => {
                settingsStorage.TryLoad(null, null);
                Assert.Fail();
            });
        }

        //---------------------------------------------------------------------
        static void AssertEqual(UserInterfaceSettings settings1, UserInterfaceSettings settings2)
        {
            Assert.AreEqual(
                settings1?.BasicSettingController?.Data?.ProgramToRun, 
                settings2?.BasicSettingController?.Data?.ProgramToRun);
        }
    }
}
