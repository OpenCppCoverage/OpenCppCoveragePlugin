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
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using System;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    class CoverageTreeManager
    {
        readonly IWindowFinder windowFinder;
        readonly DTE2 dte;
        readonly ICoverageViewManager coverageViewManager;

        //---------------------------------------------------------------------        
        public CoverageTreeManager(
            IWindowFinder windowFinder, 
            DTE2 dte,
            ICoverageViewManager coverageViewManager)
        {
            this.windowFinder = windowFinder;
            this.dte = dte;
            this.coverageViewManager = coverageViewManager;
        }

        //---------------------------------------------------------------------        
        public void ShowTreeCoverage(CoverageRate coverageRate)
        {
            var window = this.windowFinder.FindToolWindow<CoverageTreeToolWindow>();

            window.Controller.UpdateCoverageRate(
                coverageRate, dte, this.coverageViewManager);

            var frame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}
