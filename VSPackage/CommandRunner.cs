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

using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using OpenCppCoverage.VSPackage.CoverageData;
using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Settings;
using System;
using System.Linq;

namespace OpenCppCoverage.VSPackage
{
    class CommandRunner
    {
        readonly IServiceProvider serviceProvider;
        readonly IWindowFinder windowFinder;

        //---------------------------------------------------------------------
        public CommandRunner(IServiceProvider serviceProvider, IWindowFinder windowFinder)
        {
            this.serviceProvider = serviceProvider;
            this.windowFinder = windowFinder;
        }

        //---------------------------------------------------------------------
        public void OpenSettingsWindow(ProjectSelectionKind kind)
        {
            this.RunCommand(serviceProvider, windowFinder, mainWindowsManager => 
                mainWindowsManager.OpenSettingsWindow(kind)
            );
        }

        //---------------------------------------------------------------------
        public void RunCoverage(ProjectSelectionKind kind)
        {
            this.RunCommand(serviceProvider, windowFinder, mainWindowsManager => 
                mainWindowsManager.RunCoverage(kind)
            );
        }

        //---------------------------------------------------------------------
        void RunCommand(
            IServiceProvider serviceProvider,
            IWindowFinder windowFinder,
            Action<MainWindowsManager> action)
        {
            IVsUIShell uiShell = (IVsUIShell)serviceProvider.GetService(typeof(SVsUIShell));

            var errorHandler = new ErrorHandler(uiShell);
            errorHandler.Execute(() =>
            {
                var dte = (DTE2)serviceProvider.GetService(typeof(EnvDTE.DTE));
                var outputWindow = (IVsOutputWindow)serviceProvider.GetService(typeof(SVsOutputWindow));
                var outputWriter = new OutputWindowWriter(dte, outputWindow);

                errorHandler.OutputWriter = outputWriter;
                var coverageViewManager = GetCoverageViewManager(serviceProvider);
                var coverageTreeManager = new CoverageTreeManager(windowFinder, dte, coverageViewManager);
                var projectBuilder = new ProjectBuilder(dte, errorHandler, outputWriter);
                var deserializer = new CoverageDataDeserializer();
                var openCppCoverageCmdLine = new OpenCppCoverageCmdLine();
                var openCppCoverageRunner = new OpenCppCoverageRunner(outputWriter, openCppCoverageCmdLine);

                var coverageRunner = new CoverageRunner(
                    dte, outputWriter, coverageTreeManager, projectBuilder,
                    coverageViewManager, deserializer, errorHandler, openCppCoverageRunner);

                var configurationManager = new ConfigurationManager();
                var settingsBuilder = new StartUpProjectSettingsBuilder(dte, configurationManager);
                var mainWindowsManager = new MainWindowsManager(windowFinder, dte, coverageRunner, settingsBuilder, openCppCoverageCmdLine);

                action(mainWindowsManager);
            });
        }

        //---------------------------------------------------------------------
        ICoverageViewManager GetCoverageViewManager(IServiceProvider serviceProvider)
        {
            var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
            var exporterProvider = componentModel.DefaultExportProvider;
            return exporterProvider.GetExportedValue<ICoverageViewManager>();
        }
    }
}
