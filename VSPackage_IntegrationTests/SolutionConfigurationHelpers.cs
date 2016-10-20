// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2014 OpenCppCoverage
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

using EnvDTE80;
using Microsoft.VisualStudio.VCProjectEngine;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using System;
using System.Collections;
using System.Linq;

namespace VSPackage_IntegrationTests
{
    //---------------------------------------------------------------------
    public enum ConfigurationName
    {
        Debug,
        Release
    }

    //---------------------------------------------------------------------
    public enum PlatFormName
    {
        Win32,
        x64
    }

    //---------------------------------------------------------------------
    class DebugSettingsRestorer : IDisposable
    {
        //---------------------------------------------------------------------
        public DebugSettingsRestorer(VCDebugSettings settings)
        {
            this.Value = settings;
            this.command = settings.Command;
            this.commandArguments = settings.CommandArguments;
            this.workingDirectory = settings.WorkingDirectory;
        }

        //---------------------------------------------------------------------
        public VCDebugSettings Value { get; }

        //---------------------------------------------------------------------
        public void Dispose()
        {
            this.Value.Command = this.command;
            this.Value.CommandArguments = this.commandArguments;
            this.Value.WorkingDirectory = this.workingDirectory;
        }

        readonly string command;
        readonly string commandArguments;
        readonly string workingDirectory;
    }

    //---------------------------------------------------------------------
    class SolutionConfigurationHelpers
    {                
        //---------------------------------------------------------------------
        static public DebugSettingsRestorer GetCurrentDebugSettings(string applicationName)
        {
            var configuration = GetCurrentConfiguration(applicationName);
            var settings = (VCDebugSettings)configuration.DebugSettings;

            return new DebugSettingsRestorer(settings);
        }

        //---------------------------------------------------------------------
        static public void CleanSolution()
        {
            VsIdeTestHostContext.Dte.Solution.SolutionBuild.Clean(true);
        }

        //---------------------------------------------------------------------
        static public SolutionConfiguration2 GetSolutionConfiguration(
            ConfigurationName configurationName,
            PlatFormName platformName)
        {
            var dte = VsIdeTestHostContext.Dte;
            var configurations = dte.Solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration2>();
    
            return configurations.First(
                c => c.Name == configurationName.ToString() && c.PlatformName == platformName.ToString());
        }

        //---------------------------------------------------------------------
        static public SolutionConfiguration2 SetActiveSolutionConfiguration(
            ConfigurationName configurationName,
            PlatFormName platformName)
        {
            var configurationToActivate = GetSolutionConfiguration(configurationName, platformName);
            configurationToActivate.Activate();

            return configurationToActivate;
        }

        //---------------------------------------------------------------------
        static VCConfiguration GetCurrentConfiguration(string applicationName)
        {
            var projects = VsIdeTestHostContext.Dte.Solution.Projects.Cast<EnvDTE.Project>();
            var cppConsoleApplication = projects.First(p => p.UniqueName == applicationName);
            var vcCppConsoleApplication = (VCProject)cppConsoleApplication.Object;
            var configurations = (IEnumerable)vcCppConsoleApplication.Configurations;
            var solutionConfiguration =
                VsIdeTestHostContext.Dte.Solution.SolutionBuild.ActiveConfiguration as SolutionConfiguration2;
                        
            return configurations.Cast<VCConfiguration>().First(
                c => c.ConfigurationName == solutionConfiguration.Name && c.Platform.Name == solutionConfiguration.PlatformName);            
        }                
    }
}
