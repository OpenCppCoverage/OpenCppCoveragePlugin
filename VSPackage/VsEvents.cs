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

namespace OpenCppCoverage.VSPackage
{
    using EnvDTE;
    using EnvDTE80;

    using Microsoft.VisualStudio.PlatformUI;

    using System.Reflection;
    using Settings.UI;
    using Microsoft.VisualStudio;

    /// <summary>
    /// The vs events.
    /// </summary>
    class VsEvents
    {
        /// <summary>
        ///     The documents events.
        /// </summary>
        public readonly DocumentEvents DocumentsEvents;

        /// <summary>
        ///     The events.
        /// </summary>
        public readonly Events SolutionEvents;

        /// <summary>
        /// The build events.
        /// </summary>
        private BuildEvents buildEvents;

        /// <summary>
        /// The dte2
        /// </summary>
        private readonly DTE2 dte2;

        /// <summary>
        /// The settings
        /// </summary>
        private readonly SettingToolWindow settings;

        /// <summary>
        /// The command events
        /// </summary>
        private readonly CommandEvents CommandEvents;

        /// <summary>
        /// The visual studio events
        /// </summary>
        private readonly DTEEvents visualStudioEvents;

        /// <summary>
        /// The selection events
        /// </summary>
        private readonly SelectionEvents SelectionEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="VsEvents" /> class.
        /// </summary>
        /// <param name="dte2">The dte 2.</param>
        /// <param name="settings">The settings.</param>
        public VsEvents(DTE2 dte2, SettingToolWindow settings)
        {
            this.dte2 = dte2;
            this.settings = settings;
            this.SolutionEvents = dte2.Events;
            this.CommandEvents = dte2.Events.CommandEvents;
            this.visualStudioEvents = dte2.Events.DTEEvents;
            this.buildEvents = dte2.Events.BuildEvents;
            this.DocumentsEvents = this.SolutionEvents.DocumentEvents;
            this.SelectionEvents = this.SolutionEvents.SelectionEvents;

            this.CommandEvents.AfterExecute += this.CommandExecuted;
            this.SelectionEvents.OnChange += this.SelecChangeEvent;
            this.SolutionEvents.SolutionEvents.Opened += this.SolutionOpened;
            this.SolutionEvents.SolutionEvents.AfterClosing += this.SolutionClosed;
            this.SolutionEvents.WindowEvents.WindowActivated += this.WindowActivated;
            this.SolutionEvents.WindowEvents.WindowClosing += this.WindowClosed;
            this.DocumentsEvents.DocumentSaved += this.DoumentSaved;
            this.visualStudioEvents.OnStartupComplete += this.CloseToolWindows;
            this.buildEvents.OnBuildProjConfigDone += this.ProjectHasBuild;

            VSColorTheme.ThemeChanged += this.VSColorTheme_ThemeChanged;
        }

        /// <summary>
        /// The doument saved.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        private void DoumentSaved(Document document)
        {
        }

        /// <summary>
        ///     The solution closed.
        /// </summary>
        private void SolutionClosed()
        {
        }

        /// <summary>
        ///     The solution opened.
        /// </summary>
        private void SolutionOpened()
        {
        }

        /// <summary>
        /// The vs color theme_ theme changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
        }

        /// <summary>
        /// Windows the closed.
        /// </summary>
        /// <param name="window">The window.</param>
        private void WindowClosed(Window window)
        {
        }

        /// <summary>
        /// The window activated.
        /// </summary>
        /// <param name="gotFocus">
        /// The got focus.
        /// </param>
        /// <param name="lostFocus">
        /// The lost focus.
        /// </param>
        private void WindowActivated(Window gotFocus, Window lostFocus)
        {
        }

        /// <summary>
        /// Commands the executed.
        /// </summary>
        /// <param name="Guid">The unique identifier.</param>
        /// <param name="ID">The identifier.</param>
        /// <param name="CustomIn">The custom in.</param>
        /// <param name="CustomOut">The custom out.</param>
        private void CommandExecuted(string Guid, int ID, object CustomIn, object CustomOut)
        {
            if (ID == (uint)VSConstants.VSStd97CmdID.SetStartupProject)
            {
                this.settings.Controller.UpdateStartUpProject();

            }
        }

        /// <summary>
        /// Selecs the change event.
        /// </summary>
        private void SelecChangeEvent()
        {
        }

        /// <summary>
        /// Projects the has build.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectconfig">The projectconfig.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="solutionconfig">The solutionconfig.</param>
        /// <param name="success">if set to <c>true</c> [success].</param>
        private void ProjectHasBuild(string project, string projectconfig, string platform, string solutionconfig, bool success)
        {
        }

        /// <summary>
        /// Closes the tool windows.
        /// </summary>
        private void CloseToolWindows()
        {
        }
    }
}