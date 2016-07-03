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

using System.Collections.Generic;
using System.Linq;

namespace OpenCppCoverage.VSPackage.CoverageRateBuilder
{
    //-------------------------------------------------------------------------
    class BaseCoverage
    {
        protected BaseCoverage() {}
        public int CoverLineCount { get; protected set; }
        public int TotalLineCount { get; protected set; }
    }

    //-------------------------------------------------------------------------
    class HierarchicalCoverage<T> : BaseCoverage where T : BaseCoverage
    {
        readonly List<T> children;

        //---------------------------------------------------------------------
        public HierarchicalCoverage(string name)
        {
            this.Name = name;
            this.children = new List<T>();
        }

        //---------------------------------------------------------------------
        public string Name { get; private set; }

        //---------------------------------------------------------------------
        public IEnumerable<T> Children
        {
            get
            {
                return this.children;
            }
        }

        //---------------------------------------------------------------------
        public void AddChild(T child)
        {
            this.children.Add(child);
            this.CoverLineCount += child.CoverLineCount;
            this.TotalLineCount += child.TotalLineCount;
        }
    }

    //-------------------------------------------------------------------------
    class LineCoverage
    {
        public LineCoverage(int lineNumber, bool hasBeenExecuted)
        {
            this.LineNumber = lineNumber;
            this.HasBeenExecuted = hasBeenExecuted;
        }

        public int LineNumber { get; private set;  }
        public bool HasBeenExecuted { get; private set; }
    }
    
    //-------------------------------------------------------------------------
    class FileCoverage : BaseCoverage
    {
        //---------------------------------------------------------------------
        public FileCoverage(string path, List<LineCoverage> lineCoverages )
        {
            this.CoverLineCount = lineCoverages.Where(l => l.HasBeenExecuted).Count();
            this.TotalLineCount = lineCoverages.Count;
            this.Path = path;
            this.LineCoverages = lineCoverages;
        }

        //---------------------------------------------------------------------
        public string Path { get; private set; }

        //---------------------------------------------------------------------
        public IEnumerable<LineCoverage> LineCoverages { get; private set; }
    }

    //-------------------------------------------------------------------------
    class ModuleCoverage : HierarchicalCoverage<FileCoverage>
    {
        public ModuleCoverage(string name) : base(name) { }
    }

    //-------------------------------------------------------------------------
    class CoverageRate : HierarchicalCoverage<ModuleCoverage>
    {
        public CoverageRate(string name, int exitCode) 
            : base(name)
        {
            this.ExitCode = exitCode;
        }

        //---------------------------------------------------------------------
        public int ExitCode { get; private set; }
    }
}
