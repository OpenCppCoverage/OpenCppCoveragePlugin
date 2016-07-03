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

using OpenCppCoverage.VSPackage.CoverageRateBuilder;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    //---------------------------------------------------------------------------
    class RootCoverageTreeNode : BasicCoverageTreeNode
    {
        readonly CoverageRate coverage;

        //-----------------------------------------------------------------------
        public RootCoverageTreeNode(CoverageRate coverage)
            : base(coverage.Name, coverage, "48px-Gnome-folder.svg.png", false)
        {
            this.coverage = coverage;
        }

        //-----------------------------------------------------------------------
        protected override void LoadChildren()
        {
            this.AddChildrenNode(this.coverage.Children, c => new ModuleTreeNode(c));
        }
    }

    //-----------------------------------------------------------------------
    class ModuleTreeNode : BasicCoverageTreeNode
    {
        readonly ModuleCoverage coverage;

        //-----------------------------------------------------------------------
        public ModuleTreeNode(ModuleCoverage coverage)
            : base(coverage.Name, coverage, "48px-Gnome-application-x-executable.svg.png", false)
        {
            this.coverage = coverage;
        }

        //-----------------------------------------------------------------------
        protected override void LoadChildren()
        {
            this.AddChildrenNode(this.coverage.Children, c => new FileTreeNode(c.Path, c));
        }
    }

    //-----------------------------------------------------------------------
    class FileTreeNode : BasicCoverageTreeNode
    {
        //-----------------------------------------------------------------------
        public FileTreeNode(string name, FileCoverage coverage)
            : base(name, coverage, "48px-Gnome-text-x-generic.svg.png", true)
        {
            this.Coverage = coverage;
        }

        //-----------------------------------------------------------------------
        public FileCoverage Coverage { get; private set; }
    }
}
