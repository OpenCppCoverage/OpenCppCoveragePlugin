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

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace OpenCppCoverage.VSPackage.Settings.UI
{
    [Guid("1305E50A-2B2B-4168-83A7-0D57ED1EF76A")]
    class SettingToolWindow : ToolWindowPane, IVsExtensibleObject
    {
        //---------------------------------------------------------------------
        public static readonly string WindowCaption = "Settings";

        //---------------------------------------------------------------------
        public SettingToolWindow() : base(null)
        {
            this.Caption = WindowCaption;
            var control = new MainSettingControl();

            this.Controller = new MainSettingController(
                (settings) => OpenCppCoverageCmdLine.Build(settings, "\n"));
            control.DataContext = this.Controller;
            this.Controller.CloseWindowEvent += (o, e) => Close();

            // This is the user control hosted by the tool window; 
            // Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. 
            // This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.            
            this.Content = control;
        }

        //---------------------------------------------------------------------
        public MainSettingController Controller { get; private set; }

        //---------------------------------------------------------------------
        public int GetAutomationObject(string pszPropName, out object ppDisp)
        {
            ppDisp = this.Controller;
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        //---------------------------------------------------------------------
        void Close()
        {
            var frame = (IVsWindowFrame)this.Frame;
            frame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }
    }
}
