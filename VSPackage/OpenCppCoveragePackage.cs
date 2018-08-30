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

using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Win32;
using OpenCppCoverage.VSPackage.CoverageData;
using OpenCppCoverage.VSPackage.CoverageTree;
using OpenCppCoverage.VSPackage.Settings;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace OpenCppCoverage.VSPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(CoverageTreeToolWindow),
        Style = Microsoft.VisualStudio.Shell.VsDockStyle.Tabbed,
        MultiInstances = false,
        Transient = true,
        Window = Microsoft.VisualStudio.Shell.Interop.ToolWindowGuids.Outputwindow)]
    [ProvideToolWindow(typeof(SettingToolWindow),
        Style = Microsoft.VisualStudio.Shell.VsDockStyle.Float,
        MultiInstances = false,
        Transient = true)]
    [Guid(GuidList.guidVSPackagePkgString)]
    [ProvideBindingPath]
    public sealed class OpenCppCoveragePackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public OpenCppCoveragePackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(
                    GuidList.guidVSPackageCmdSet, 
                    (int)PkgCmdIDList.RunOpenCppCoverageCommand);
                MenuCommand menuItem = new MenuCommand(
                    RunCoverageForStartUpProject, menuCommandID );
                mcs.AddCommand( menuItem );

                CommandID menuSelectedProjCommandID = new CommandID(
                    GuidList.guidVSPackageCmdSet, 
                    (int)PkgCmdIDList.RunOpenCppCoverageFromSelectedProjectCommand);
                MenuCommand selectedProjMenuItem = new MenuCommand(
                    RunCoverageForSelectedProject, menuSelectedProjCommandID);
                mcs.AddCommand(selectedProjMenuItem);
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        void RunCoverageForStartUpProject(object sender, EventArgs e)
        {
            RunCoverage(ProjectSelectionKind.StartUpProject);
        }

        //---------------------------------------------------------------------
        void RunCoverageForSelectedProject(object sender, EventArgs e)
        {
            RunCoverage(ProjectSelectionKind.SelectedProject);
        }

        //---------------------------------------------------------------------
        void RunCoverage(ProjectSelectionKind kind)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));

            var errorHandler = new ErrorHandler(uiShell);
            errorHandler.Execute(() =>
            {
                var dte = (DTE2)GetService(typeof(EnvDTE.DTE));
                var outputWindow = (IVsOutputWindow)GetService(typeof(SVsOutputWindow));
                var outputWriter = new OutputWindowWriter(dte, outputWindow);

                errorHandler.OutputWriter = outputWriter;
                var mainSettingsManager = new MainSettingsManager(this, dte);
                var coverageViewManager = GetCoverageViewManager();
                var coverageTreeManager = new CoverageTreeManager(this, dte, coverageViewManager);
                var projectBuilder = new ProjectBuilder(dte, errorHandler, outputWriter);
                var deserializer = new CoverageDataDeserializer();
                var openCppCoverageRunner = new CoverageRunner(
                    dte, outputWriter, coverageTreeManager, projectBuilder,
                    coverageViewManager, deserializer, errorHandler);

                mainSettingsManager.ShowSettingsWindows(openCppCoverageRunner, kind);
            });
        }

        //---------------------------------------------------------------------
        CoverageViewManager GetCoverageViewManager()
        {
            var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            var exporterProvider = componentModel.DefaultExportProvider;
            var listeners = exporterProvider.GetExportedValues<IWpfTextViewCreationListener>();
            var listener = listeners.First(l => l is CoverageViewManager);

            return (CoverageViewManager)listener;
        }
    }
}
