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

using ICSharpCode.TreeView;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    class BasicCoverageTreeNode: SharpTreeNode
    {
        readonly BaseCoverage coverage;
        readonly string name;
        readonly ImageSource icon;

        static readonly string imagesFolder;

        //-----------------------------------------------------------------------
        static BasicCoverageTreeNode()
        {
            var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BasicCoverageTreeNode.imagesFolder = Path.Combine(rootFolder, "CoverageTree", "Images");
        }

        //-----------------------------------------------------------------------
        public BasicCoverageTreeNode(
            string name,
            BaseCoverage coverage,
            string iconFilename,
            bool isLeaf)
        {
            this.name = name;
            this.coverage = coverage;
            this.LazyLoading = !isLeaf;
            this.icon = LoadIcon(iconFilename);
        }

        //-----------------------------------------------------------------------
        public override object Icon
        {
            get
            {
                return icon;
            }
        }

        //-----------------------------------------------------------------------
        public override object Text
        {
            get
            {
                return this.name;
            }
        }

        //-----------------------------------------------------------------------
        public double? CoverageRate
        {
            get
            {
                if (this.TotalLineCount == 0)
                    return null;
                return (double)this.CoveredLineCount / this.TotalLineCount;
            }
        }

        //-----------------------------------------------------------------------
        public double? UncoverageRate
        {
            get
            {
                if (this.TotalLineCount == 0)
                    return null;
                return (double)this.UncoveredLineCount / this.TotalLineCount;
            }
        }

        //-----------------------------------------------------------------------
        public int CoveredLineCount
        {
            get
            {
                return this.coverage.CoverLineCount;
            }
        }

        //-----------------------------------------------------------------------
        public int UncoveredLineCount
        {
            get
            {
                return this.coverage.TotalLineCount - this.coverage.CoverLineCount;
            }
        }

        //-----------------------------------------------------------------------
        public int TotalLineCount
        {
            get
            {
                return this.coverage.TotalLineCount;
            }
        }

        //-----------------------------------------------------------------------
        protected void AddChildrenNode<T>(
            IEnumerable<T> children,
            Func<T, BasicCoverageTreeNode> nodeFactory)
        {
            var childrenNode = children.Select(nodeFactory);
            this.Children.AddRange(childrenNode.OrderBy(c => c.CoverageRate));
        }

        //-----------------------------------------------------------------------
        static ImageSource LoadIcon(string iconFilename)
        {
            var iconPath = Path.Combine(imagesFolder, iconFilename);
            return BitmapFrame.Create(new Uri(iconPath, UriKind.Absolute));
        }
    }
}
