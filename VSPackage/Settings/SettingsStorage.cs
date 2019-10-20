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

using Newtonsoft.Json;
using OpenCppCoverage.VSPackage.Settings.UI;
using System.IO;

namespace OpenCppCoverage.VSPackage.Settings
{
    class SettingsStorage
    {
        readonly string applicationDataFolder;

        //---------------------------------------------------------------------
        public SettingsStorage(string applicationDataFolder)
        {
            this.applicationDataFolder = applicationDataFolder;
        }

        //---------------------------------------------------------------------
        public string Save(
            string optionalProjectPath, 
            string optionalSolutionConfigurationName, 
            UserInterfaceSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings);
            this.CreateConfigfolder(optionalProjectPath);
            var configPath = GetConfigPath(optionalProjectPath, optionalSolutionConfigurationName);
            File.WriteAllText(configPath, json);
            return configPath;
        }

        //---------------------------------------------------------------------
        public UserInterfaceSettings TryLoad(string optionalProjectPath, string optionalSolutionConfigurationName)
        {
            var configPath = this.GetConfigPath(optionalProjectPath, optionalSolutionConfigurationName);
            string json;

            try
            {
                json = File.ReadAllText(configPath);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<UserInterfaceSettings>(json);
            }
            catch (JsonException e)
            {
                var error = string.Format(
                    $"Error when deserializing {configPath} : {e.Message}\nRemoving {configPath} should fix this issue.");
                throw new VSPackageException(error);
            }
        }

        //---------------------------------------------------------------------
        void CreateConfigfolder(string optionalProjectPath)
        {
            var folder = this.GetConfigFolder(optionalProjectPath);
            Directory.CreateDirectory(folder);
        }

        public static string OpenCppCov { get; } = ".opencppcov";
        public static string ApplicationDataSection { get; } = "OpenCppCoverage";
        public static string NoProjectConfigName { get; } = "config";

        //---------------------------------------------------------------------
        string GetConfigFolder(string optionalProjectPath)
        {
            if (optionalProjectPath != null)
            {
                return Path.Combine(
                    Path.GetDirectoryName(optionalProjectPath),
                    OpenCppCov,
                    Path.GetFileNameWithoutExtension(optionalProjectPath));            
            }

            return Path.Combine(this.applicationDataFolder, ApplicationDataSection);
        }

        //---------------------------------------------------------------------
        string GetConfigPath(string optionalProjectPath, string optionalSolutionConfigurationName)
        {
            var folder = this.GetConfigFolder(optionalProjectPath);
            string filename;

            if (optionalSolutionConfigurationName != null)
            {
                filename = optionalSolutionConfigurationName;

                foreach (var c in Path.GetInvalidFileNameChars())
                    filename = filename.Replace(c, '_');
            }
            else
            {
                filename = NoProjectConfigName;
            }
            return Path.Combine(folder, filename + ".json");
        }
    }
}
