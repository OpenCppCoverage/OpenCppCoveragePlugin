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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using OpenCppCoverage.VSPackage.CoverageTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VSPackage_IntegrationTests
{
    [TestClass()]
    public class CoverageViewManagerTests: TestHelpers
    {        
        //---------------------------------------------------------------------
        [TestInitialize]
        public void TestInitialize()
        {
            OpenSolution(CppConsoleApplication);
            VsIdeTestHostContext.Dte.Documents.CloseAll();
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void OpenFileAndComputeCoverage()
        {
            var wpfTextView = OpenMainFile();
            RunCoverageAndWait();
            
            CheckCoverage(wpfTextView);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void ComputeCoverageAndOpenFile()
        {
            RunCoverageAndWait();
            var wpfTextView = OpenMainFile();
            
            CheckCoverage(wpfTextView);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void CloseView()
        {
            var wpfTextView = OpenMainFile();
            RunCoverageAndWait();

            VsIdeTestHostContext.Dte.Documents.CloseAll();
            wpfTextView = OpenMainFile();            
            CheckCoverage(wpfTextView);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void OpenFileAndDisableCoverage()
        {
            var wpfTextView = OpenMainFile();
            var coverageTreeController = RunCoverageAndWait();

            CheckCoverage(wpfTextView);

            RunInUIhread(() => coverageTreeController.DisplayCoverage = false);
            Assert.AreEqual(0, GetLinesWithCoverageTag(wpfTextView).Count);

            RunInUIhread(() => coverageTreeController.DisplayCoverage = true);
            CheckCoverage(wpfTextView);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        [HostType("VS IDE")]
        public void DisableCoverageAndOpenFile()
        {
            var coverageTreeController = RunCoverageAndWait();
            RunInUIhread(() => coverageTreeController.DisplayCoverage = false);

            var wpfTextView = OpenMainFile();
            Assert.AreEqual(0, GetLinesWithCoverageTag(wpfTextView).Count);

            RunInUIhread(() => coverageTreeController.DisplayCoverage = true);
            CheckCoverage(wpfTextView);
        }

        //---------------------------------------------------------------------
        void CheckCoverage(IWpfTextView wpfTextView)
        {            
            var lines = GetLinesWithCoverageTag(wpfTextView);

            Assert.IsTrue(lines.Count > 1);
            foreach (var line in lines)
            {
                var text = line.Item1;

                if (text.EndsWith(" COVERED"))
                    Assert.AreEqual(CoverageViewManager.CoveredBrush, line.Item2);
                else if (text.EndsWith(" UNCOVERED"))
                    Assert.AreEqual(CoverageViewManager.UncoveredBrush, line.Item2);
                else
                    Assert.Fail();
            }
        }

        //---------------------------------------------------------------------
        List<Tuple<string, Brush>> GetLinesWithCoverageTag(IWpfTextView wpfTextView)
        {
            var lines = new List<Tuple<string, Brush>>();

            // AdornmentLayer is filled asynchronously by an event.
            // Wait here to be sure adornmentLayer.Elements is not empty.
            System.Threading.Thread.Sleep(1000);
            RunInUIhread(() =>
            {
                var adornmentLayer = wpfTextView.GetAdornmentLayer(CoverageViewManager.HighlightLinesAdornment);
                var elements = adornmentLayer.Elements.Where(e => e.Tag == CoverageViewManager.CoverageTag);

                foreach (var element in elements)
                {
                    var adornment = (Rectangle)element.Adornment;
                    var line = wpfTextView.GetTextViewLineContainingBufferPosition(element.VisualSpan.Value.Start);
                    var text = line.Extent.GetText();

                    lines.Add(Tuple.Create(text, adornment.Fill));
                }
            });

            return lines;
        }

        //---------------------------------------------------------------------
        IVsTextView OpenTextView(string path)
        {
            var serviceProvider = VsIdeTestHostContext.ServiceProvider;
            uint itemID;
            IVsUIHierarchy uiHierarchy;
            IVsWindowFrame windowFrame;
            IVsTextView textView;

            VsShellUtilities.OpenDocument(serviceProvider, path, Guid.Empty,
                out uiHierarchy, out itemID, out windowFrame, out textView);

            if (textView == null)
                throw new InvalidOperationException("Cannot open document " + path);
            return textView;
        }

        //---------------------------------------------------------------------
        IWpfTextView OpenWpfTextView(string path)
        {
            var textView = OpenTextView(path);
            var userData = textView as IVsUserData;

            if (null == userData)
                throw new InvalidOperationException("Cannot convert IVsTextView to IVsUserData");

            object holder;
            Guid guidViewHost = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out holder);
            var textViewHost = (IWpfTextViewHost)holder;

            return textViewHost.TextView;
        }

        //---------------------------------------------------------------------
        IWpfTextView OpenMainFile()
        {
            var file = System.IO.Path.Combine(
                GetIntegrationTestsSolutionFolder(),
                @"cppconsoleapplication\cppconsoleapplication.cpp");

            return OpenWpfTextView(file);
        }
    }
}
