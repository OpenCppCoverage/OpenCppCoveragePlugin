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
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using OpenCppCoverage.VSPackage.CoverageTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSPackage_IntegrationTests
{
    [TestClass]
    public class CoverageTreeTests: TestHelpers
    {
        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void OpenCurrentFile()
        {
            OpenSolution(CppConsoleApplication);

            var controller = RunCoverageAndWait();
            CloseAllDocuments();
            RunInUIhread(() =>
            {
                var fileTreeNode = GetFileTreeNodes(controller).First();
                var path = fileTreeNode.Coverage.Path;
                Assert.IsFalse(IsDocumentOpen(path));
                controller.Current = fileTreeNode;
                Assert.IsTrue(IsDocumentOpen(path));
            });
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CheckCoverage()
        {
            OpenSolution(CppConsoleApplication);
            var controller = RunCoverageAndWait();
            RunInUIhread(() =>
            {
                var fileTreeNodes = GetFileTreeNodes(controller);
                Assert.IsTrue(fileTreeNodes.Any());
                foreach (var fileTreeNode in fileTreeNodes)
                    CheckFileCoverage(fileTreeNode);
            });
        }

        //---------------------------------------------------------------------
        void CheckFileCoverage(FileTreeNode fileTreeNode)
        {
            var coverage = fileTreeNode.Coverage;
            using (var streamReader = new StreamReader(coverage.Path))
            {
                var content = streamReader.ReadToEnd();
                var uncoveredLineCount = coverage.TotalLineCount - coverage.CoverLineCount;
                Assert.AreEqual(coverage.CoverLineCount, Count(content, CoveredTag));
                Assert.AreEqual(uncoveredLineCount, Count(content, UncoveredTag));
            }
        }

        //---------------------------------------------------------------------
        static bool IsDocumentOpen(string path)
        {
            var serviceProvider = VsIdeTestHostContext.ServiceProvider;
            uint itemID;
            IVsUIHierarchy uiHierarchy;
            IVsWindowFrame windowFrame;

            return VsShellUtilities.IsDocumentOpen(serviceProvider, path, Guid.Empty,
                out uiHierarchy, out itemID, out windowFrame);
        }

        //---------------------------------------------------------------------
        static int Count(string str, string strToSearch)
        {
            int index = 0;
            int count = 0;

            while (true)
            {
                index = str.IndexOf(strToSearch, index);
                if (index == -1)
                    break;
                index += strToSearch.Length;
                ++count;
            }
            
            return count;
        }

        //---------------------------------------------------------------------
        static IEnumerable<FileTreeNode> GetFileTreeNodes(CoverageTreeController controller)
        {
            var root = controller.Root;
            ExpandAllNodes(root);
            var descendants = root.Descendants();

            return descendants.Where(n => n is FileTreeNode).Cast<FileTreeNode>();
        }

        //---------------------------------------------------------------------
        static void ExpandAllNodes(SharpTreeNode node)
        {
            node.EnsureLazyChildren();
            foreach (var child in node.Children)
                ExpandAllNodes(child);
        }
    }
}
