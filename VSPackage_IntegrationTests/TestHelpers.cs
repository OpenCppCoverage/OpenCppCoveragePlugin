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

using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VSSDK.Tools.VsIdeTesting;

using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace VSPackage_IntegrationTests
{
    public class TestHelpers
    {
        internal readonly string CppConsoleApplication = @"CppConsoleApplication\CppConsoleApplication.vcxproj";
        internal readonly string CppConsoleApplication2 = @"CppConsoleApplication2\CppConsoleApplication2.vcxproj";
        internal readonly string CSharpConsoleApplication = @"CSharpConsoleApplication\CSharpConsoleApplication.csproj";
        internal readonly string CppConsoleApplicationDll = @"CppConsoleApplicationDll\CppConsoleApplicationDll.vcxproj";
        internal readonly string ConsoleApplicationInFolder = @"ConsoleApplicationInFolder\ConsoleApplicationInFolder.vcxproj";
        internal readonly string CoveredTag = " COVERED";
        internal readonly string UncoveredTag = " UNCOVERED";

        //---------------------------------------------------------------------
        internal string GetOpenCppCoverageMessage(Action action)
        {                        
            var uiShell = GetService<IVsUIShell>();

            using (var dialogBoxMessageRetriever = new DialogBoxMessageRetriever(uiShell, TimeSpan.FromSeconds(10)))
            {
                action();
                return dialogBoxMessageRetriever.GetMessage();
            }
        }

        //---------------------------------------------------------------------
        internal void OpenSolution(params string[] startupProjects)
        {
            OpenSolution(startupProjects, ConfigurationName.Debug, PlatFormName.Win32);
        }

        //---------------------------------------------------------------------
        internal void OpenSolution(
            string startupProjects,
            ConfigurationName configurationName = ConfigurationName.Debug,
            PlatFormName platformName = PlatFormName.Win32)
        {
            OpenSolution(new string[] { startupProjects }, configurationName, platformName);
        }

        //---------------------------------------------------------------------
        internal T GetService<T>() where T : class
        {
            var service = VsIdeTestHostContext.ServiceProvider.GetService(typeof(T)) as T;
            if (service == null)
                throw new Exception("Service is null");

            return service;
        }

        //---------------------------------------------------------------------
        internal MainSettingController ExecuteOpenCppCoverageCommand()
        {
            MainSettingController controller = null;
            RunInUIhread(() =>
            {
                object Customin = null;
                object Customout = null;
                var commandGuid = OpenCppCoverage.VSPackage.GuidList.guidVSPackageCmdSet;
                string guidString = commandGuid.ToString("B").ToUpper();
                int cmdId = (int)OpenCppCoverage.VSPackage.PkgCmdIDList.RunOpenCppCoverageCommand;
                DTE dte = VsIdeTestHostContext.Dte;

                dte.Commands.Raise(guidString, cmdId, ref Customin, ref Customout);

                controller = GetController<MainSettingController>();
            });

            return controller;
        }

        //---------------------------------------------------------------------
        internal void RunCoverage()
        {
            var controller = ExecuteOpenCppCoverageCommand();
            RunCoverageCommand(controller);
        }

        //---------------------------------------------------------------------
        internal CoverageTreeController RunCoverageAndWait()
        {
            var controller = ExecuteOpenCppCoverageCommand();
            return RunCoverageCommandAndWait(controller);
        }

        //---------------------------------------------------------------------
        internal CoverageTreeController RunCoverageCommandAndWait(
                            MainSettingController controller)
        {
            RunCoverageCommand(controller);
            return CloseOpenCppCoverageConsole(TimeSpan.FromSeconds(10));
        }

        //---------------------------------------------------------------------
        internal void RunCoverageCommand(MainSettingController controller)
        {
            RunInUIhread(() => { controller.RunCoverageCommand.Execute(null); });
        }

        //---------------------------------------------------------------------
        internal void CloseAllDocuments()
        {
            VsIdeTestHostContext.Dte.Documents.CloseAll();
        }

        //---------------------------------------------------------------------
        T GetController<T>() where T: class
        {
            DTE dte = VsIdeTestHostContext.Dte;
            return Wait(TimeSpan.FromSeconds(10), () =>
            {
                foreach (Window window in dte.Windows)
                {
                    var controller = window.Object as T;

                    if (controller != null)
                        return controller;
                }

                return null;
            });
        }

        //---------------------------------------------------------------------
        internal T Wait<T>(
            TimeSpan timeout, 
            Func<T> func, T 
            defaultValue = default(T))
        {
            const int partCount = 50;
            var smallTimeout = new TimeSpan(timeout.Ticks / partCount);

            for (int nbTry = 0; nbTry < partCount; ++nbTry)
            {
                T value = func();

                if (!EqualityComparer<T>.Default.Equals(value, defaultValue))
                    return value;
                System.Threading.Thread.Sleep(smallTimeout);
            }

            return defaultValue;
        }

        //---------------------------------------------------------------------
        internal string GetIntegrationTestsSolutionFolder()
        {
            var currentLocation = typeof(TestHelpers).Assembly.Location;
            var currentDirectory = Path.GetDirectoryName(currentLocation);
            return Path.Combine(currentDirectory, "IntegrationTestsSolution");
        }

        //---------------------------------------------------------------------
        internal CoverageTreeController CloseOpenCppCoverageConsole(TimeSpan waitDuration)
        {
            System.Threading.Thread.Sleep(waitDuration);
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            return GetController<CoverageTreeController>();
        }

        //---------------------------------------------------------------------
        internal void RunInUIhread(Action action)
        {
            UIThreadInvoker.Invoke(action);
        }

        //---------------------------------------------------------------------
        internal void WaitEndOfBuild()
        {
            while (VsIdeTestHostContext.Dte.Solution.SolutionBuild.BuildState == vsBuildState.vsBuildStateInProgress)
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        //---------------------------------------------------------------------
        EnvDTE80.SolutionConfiguration2 OpenSolution(
            string[] startupProjects,
            ConfigurationName configurationName,
            PlatFormName platformName)
        {
            OpenDefaultSolution();
            var startupProjectObjects = new object[startupProjects.Length];
            Array.Copy(startupProjects, startupProjectObjects, startupProjectObjects.Length);
            VsIdeTestHostContext.Dte.Solution.SolutionBuild.StartupProjects = startupProjectObjects;
            return SolutionConfigurationHelpers.SetActiveSolutionConfiguration(configurationName, platformName);
        }

        //---------------------------------------------------------------------
        void OpenDefaultSolution()
        {
            RunInUIhread(() =>
            {
               var solutionService = GetService<IVsSolution>();
               var solutionPath = Path.Combine(GetIntegrationTestsSolutionFolder(), "IntegrationTestsSolution.sln");

               Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                   solutionService.OpenSolutionFile((uint)__VSSLNOPENOPTIONS.SLNOPENOPT_Silent, solutionPath));
            });
            WaitForSolutionLoading(TimeSpan.FromSeconds(30));
        }

        //---------------------------------------------------------------------
        void WaitForSolutionLoading(TimeSpan timeout)
        {
            Wait(timeout, () =>
                {
                    foreach (Project p in VsIdeTestHostContext.Dte.Solution.Projects)
                    {
                        if (p.Object == null)
                            return false;
                    }

                    return true;
                }, false);
        }        
    }
}
