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
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using System;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    class CoverageTreeManager
    {
        readonly Package package;

        //---------------------------------------------------------------------        
        public CoverageTreeManager(Package package)
        {
            this.package = package;
        }

        //---------------------------------------------------------------------        
        public void ShowTreeCoverage(CoverageRate coverageRate)
        {
            var window = this.package.FindToolWindow(
                typeof(CoverageTreeToolWindow), 0, true) as CoverageTreeToolWindow;
            if (window == null || window.Frame == null)
                throw new NotSupportedException("Cannot create tool window");

            window.Controller.CoverageRate = coverageRate;
            
            var frame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}
