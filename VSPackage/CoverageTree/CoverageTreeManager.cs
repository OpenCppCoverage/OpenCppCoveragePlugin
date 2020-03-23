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
        //---------------------------------------------------------------------        
        public CoverageTreeManager(IWindowFinder windowFinder)
        {
            this.windowFinder = windowFinder;
        }

        //---------------------------------------------------------------------        
        public void ShowTreeCoverage(
            DTE2 dte,
            ICoverageViewManager coverageViewManager, 
            CoverageRate coverageRate)
        {
            ShowTreeCoverage(window => window.Controller.UpdateCoverageRate(
                coverageRate, dte, coverageViewManager));
        }

        //---------------------------------------------------------------------        
        public void ShowTreeCoverage()
        {
            ShowTreeCoverage(windows => {});
        }

        //---------------------------------------------------------------------        
        void ShowTreeCoverage(Action<CoverageTreeToolWindow> action)
        {
            //var window = this.windowFinder.FindToolWindow<CoverageTreeToolWindow>();

            //action(window);
            //var frame = (IVsWindowFrame)window.Frame;
            //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }
    }
}
