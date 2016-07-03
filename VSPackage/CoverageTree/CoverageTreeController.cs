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

using ICSharpCode.TreeView;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.Editor;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    class CoverageTreeController: PropertyChangedNotifier
    {
        RootCoverageTreeNode rootNode;
        FileCoverage currentFileCoverage;
        readonly EditorHighlighter editorHighlighter;

        //-----------------------------------------------------------------------
        public CoverageTreeController(EditorHighlighter editorHighlighter)
        {
            this.editorHighlighter = editorHighlighter;
        }

        //-----------------------------------------------------------------------
        public CoverageRate CoverageRate
        {
            set
            {
                this.Root = new RootCoverageTreeNode(value);
            }
        }

        //-----------------------------------------------------------------------
        public SharpTreeNode Current
        {
            set
            {
                var fileTreeNode = value as FileTreeNode;
                var fileCoverage = fileTreeNode != null ? fileTreeNode.Coverage: null;

                if (fileCoverage != currentFileCoverage)
                {
                    if (currentFileCoverage != null)
                        editorHighlighter.RemoveCoverage(currentFileCoverage);
                    if (fileCoverage != null)
                        editorHighlighter.DisplayCoverage(fileCoverage);

                    currentFileCoverage = fileCoverage;
                }
            }
        }

        //-----------------------------------------------------------------------
        public RootCoverageTreeNode Root
        {
            get
            {
                return this.rootNode;
            }

            private set
            {
                if (this.rootNode != value)
                {
                    this.rootNode = value;
                    NotifyPropertyChanged("Root");
                }
            }
        }
    }
}
