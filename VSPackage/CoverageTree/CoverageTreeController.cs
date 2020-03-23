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
using EnvDTE80;
using ICSharpCode.TreeView;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.Helper;
using System;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    class CoverageTreeController: PropertyChangedNotifier
    {
        RootCoverageTreeNode rootNode;
        string filter;
        string warning;
        DTE2 dte;
        ICoverageViewManager coverageViewManager;

        readonly TreeNodeVisibilityManager visibilityManager;

        //-----------------------------------------------------------------------
        public readonly static string WarningMessage 
            = "Warning: Your program has exited with error code: ";

        //-----------------------------------------------------------------------
        public CoverageTreeController()
        {
            this.visibilityManager = new TreeNodeVisibilityManager();
        }

        //-----------------------------------------------------------------------
        public void UpdateCoverageRate(
            CoverageRate coverageRate,
            DTE2 dte,
            ICoverageViewManager coverageViewManager)
        {
            this.dte = dte;
            this.coverageViewManager = coverageViewManager;
            this.Root = new RootCoverageTreeNode(coverageRate);
            this.Filter = "";
            this.DisplayCoverage = true;

            if (coverageRate.ExitCode == 0)
                this.Warning = null;
            else
            {
                this.Warning = WarningMessage + coverageRate.ExitCode;
            }
        }

        //-----------------------------------------------------------------------
        public SharpTreeNode Current
        {
            set
            {
                var fileTreeNode = value as FileTreeNode;
                var fileCoverage = fileTreeNode?.Coverage;

                if (fileCoverage != null)
                {
                    if (this.dte == null)
                    {
                        //outputWindowWriter.WriteLine("ERROR: UpdateCoverageRate should be call first.");
                        throw new InvalidOperationException("UpdateCoverageRate should be call first.");
                    }
                    this.dte.ItemOperations.OpenFile(fileCoverage.Path, Constants.vsViewKindCode);
                }
            }
        }

        //-----------------------------------------------------------------------
        public RootCoverageTreeNode Root
        {
            get { return this.rootNode; }
            private set { this.SetField(ref this.rootNode, value); }
        }

        //-----------------------------------------------------------------------
        public string Filter
        {
            get { return this.filter; }
            set 
            {
                if (SetField(ref this.filter, value))
                {
                    if (this.Root != null && value != null)
                    {
                        this.visibilityManager.UpdateVisibility(this.Root, value);
                        NotifyPropertyChanged("Root");
                    }
                }
            }
        }

        //-----------------------------------------------------------------------
        public string Warning
        {
            get { return this.warning; }
            set { SetField(ref this.warning, value); }
        }

        //-----------------------------------------------------------------------
        bool displayCoverage;
        public bool DisplayCoverage
        {
            get { return this.displayCoverage; }
            set
            {
                if (this.SetField(ref this.displayCoverage, value))
                    this.coverageViewManager.ShowCoverage = value;
            }
        }
    }
}
