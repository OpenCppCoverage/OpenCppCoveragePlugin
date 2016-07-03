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

using Microsoft.VisualStudio.Shell;
using OpenCppCoverage.VSPackage.Editor;
using System.Runtime.InteropServices;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    [Guid("2731AD3B-8E0A-46D2-B8F6-CA40C387732F")]
    class CoverageTreeToolWindow : ToolWindowPane
    {
        public CoverageTreeToolWindow() : base(null)
        {
            this.Caption = "CoverageTree";

            // This is the user control hosted by the tool window; 
            // Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. 
            // This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var coverageTreeControl = new CoverageTreeControl();

            var editorHighlighter = new EditorHighlighter();
            this.Controller = new CoverageTreeController(editorHighlighter);
            coverageTreeControl.DataContext = this.Controller;
            this.Content = coverageTreeControl;
        }

        public CoverageTreeController Controller { get; private set; }
    }
}
