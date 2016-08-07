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
using System;
using System.Collections.Generic;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    class TreeNodeVisibilityManager
    {
        //---------------------------------------------------------------------------
        public void UpdateVisibility(RootCoverageTreeNode node, string filter)
        {
            node.EnsureLazyChildren();
            foreach (var module in node.Modules)
            {
                var fileVisibilities = new List<FileVisibility>();
                bool oneChildVisible = false;

                module.EnsureLazyChildren();
                foreach (var file in module.Files)
                {
                    bool newVisibility = NewVisibility(file, filter);
                    oneChildVisible = oneChildVisible || newVisibility;

                    if (newVisibility != !file.IsHidden)
                        fileVisibilities.Add(new FileVisibility { File = file, Visibility = newVisibility });
                }
                module.IsHidden = !oneChildVisible && !NewVisibility(module, filter);
                
                if (!module.IsHidden)
                {
                    foreach (var fileVisibility in fileVisibilities)
                        fileVisibility.File.IsHidden = !fileVisibility.Visibility;
                }
            }
        }

        //---------------------------------------------------------------------------
        class FileVisibility
        {
            public FileTreeNode File { get; set; }
            public bool Visibility { get; set; }
        }

        //---------------------------------------------------------------------------
        static bool NewVisibility(SharpTreeNode node, string filter)
        {
            var text = (string)node.Text;
            return text.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}
