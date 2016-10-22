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
using Microsoft.VisualStudio.VCProjectEngine;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using OpenCppCoverage.VSPackage;
using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSPackage_IntegrationTests
{
    class TestHelpers
    {
        static public readonly string CppConsoleApplication = @"CppConsoleApplication\CppConsoleApplication.vcxproj";
        static public readonly string CppConsoleApplication2 = @"CppConsoleApplication2\CppConsoleApplication2.vcxproj";
        static public readonly string CSharpConsoleApplication = @"CSharpConsoleApplication\CSharpConsoleApplication.csproj";
        static public readonly string CppConsoleApplicationDll = @"CppConsoleApplicationDll\CppConsoleApplicationDll.vcxproj";
        static public readonly string ConsoleApplicationInFolder = @"ConsoleApplicationInFolder\ConsoleApplicationInFolder.vcxproj";
        static public readonly string ApplicationName = "CppConsoleApplication.exe";
        static public readonly string ApplicationName2 = "CppConsoleApplication2.exe";
        static public readonly string ConsoleApplicationInFolderName = "ConsoleApplicationInFolder.exe";

        //---------------------------------------------------------------------
        static public string GetOpenCppCoverageMessage()
        {                        
            var uiShell = GetService<IVsUIShell>();

            using (var dialogBoxMessageRetriever = new DialogBoxMessageRetriever(uiShell, TimeSpan.FromSeconds(10)))
            {
                ExecuteOpenCppCoverageCommand();
                return dialogBoxMessageRetriever.GetMessage();
            }
        }

        //---------------------------------------------------------------------
        static public void OpenSolution(params string[] startupProjects)
        {
            OpenSolution(startupProjects, ConfigurationName.Debug, PlatFormName.Win32);
        }

        //---------------------------------------------------------------------
        static public void OpenSolution(
            string startupProjects,
            ConfigurationName configurationName = ConfigurationName.Debug,
            PlatFormName platformName = PlatFormName.Win32)
        {
            OpenSolution(new string[] { startupProjects }, configurationName, platformName);
        }

        //---------------------------------------------------------------------
        static public T GetService<T>() where T : class
        {
            var service = VsIdeTestHostContext.ServiceProvider.GetService(typeof(T)) as T;
            if (service == null)
                throw new Exception("Service is null");

            return service;
        }

        //---------------------------------------------------------------------
        public static MainSettingController ExecuteOpenCppCoverageCommand()
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
        public static void RunCoverage()
        {
            var controller = TestHelpers.ExecuteOpenCppCoverageCommand();
            RunInUIhread(() => { controller.RunCoverageCommand.Execute(null); });
        }

        //---------------------------------------------------------------------
        public static CoverageTreeController RunCoverageAndWait()
        {
            var controller = TestHelpers.ExecuteOpenCppCoverageCommand();
            return ExecuteRunCoverageCommand(controller);
        }

        //---------------------------------------------------------------------
        public static CoverageTreeController ExecuteRunCoverageCommand(
                            MainSettingController controller)
        {
            RunInUIhread(() => { controller.RunCoverageCommand.Execute(null); });
            return TestHelpers.CloseOpenCppCoverageConsole(TimeSpan.FromSeconds(10));
        }

        //---------------------------------------------------------------------
        static T GetController<T>() where T: class
        {
            DTE dte = VsIdeTestHostContext.Dte;
            return TestHelpers.Wait(TimeSpan.FromSeconds(10), () =>
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
        public static T Wait<T>(
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
        public static string GetIntegrationTestsSolutionFolder()
        {
            var currentLocation = typeof(TestHelpers).Assembly.Location;
            var currentDirectory = Path.GetDirectoryName(currentLocation);
            return Path.Combine(currentDirectory, "IntegrationTestsSolution");
        }

        //---------------------------------------------------------------------
        public static CoverageTreeController CloseOpenCppCoverageConsole(TimeSpan waitDuration)
        {
            System.Threading.Thread.Sleep(waitDuration);
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            return GetController<CoverageTreeController>();
        }

        //---------------------------------------------------------------------
        public static void RunInUIhread(Action action)
        {
            UIThreadInvoker.Invoke(action);
        }

        //---------------------------------------------------------------------
        static EnvDTE80.SolutionConfiguration2 OpenSolution(
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
        static void OpenDefaultSolution()
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
        static void WaitForSolutionLoading(TimeSpan timeout)
        {
            TestHelpers.Wait(timeout, () =>
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
