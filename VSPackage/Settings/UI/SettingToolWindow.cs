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
    class SettingToolWindow : ToolWindowPane, IVsExtensibleObject, IVsWindowFrameNotify2
    {
        readonly MainSettingControl mainSettingControl;

        //---------------------------------------------------------------------
        public static readonly string WindowCaption = "Settings";

        //---------------------------------------------------------------------
        public SettingToolWindow() : base(null)
        {
            this.Caption = WindowCaption;
            this.mainSettingControl = new MainSettingControl();

            // This is the user control hosted by the tool window; 
            // Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. 
            // This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.            
            this.Content = this.mainSettingControl;
        }

        //---------------------------------------------------------------------
        public void Init(MainSettingController controller)
        {
            if (this.Controller != null)
                this.Controller.CloseWindowEvent -= Close;

            this.Controller = controller;
            this.mainSettingControl.DataContext = this.Controller;
            this.Controller.CloseWindowEvent += Close;
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
        void Close(object sender, EventArgs e)
        {
            var frame = (IVsWindowFrame)this.Frame;
            frame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }

        //---------------------------------------------------------------------
        // This method is called when closing Visual Studio
        protected override void OnClose()
        {
            this.Controller.SaveSettings();
        }

        //---------------------------------------------------------------------
        public int OnClose(ref uint pgrfSaveOptions)
        {
            this.Controller.SaveSettings();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }
    }
}
