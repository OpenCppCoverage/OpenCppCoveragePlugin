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

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("C/C++")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    sealed class CoverageViewCreationListener : IWpfTextViewCreationListener
    {
        const string HighlightLinesAdornment = "HighlightLines";
        readonly Dictionary<string, IWpfTextView> viewsByPath = new Dictionary<string, IWpfTextView>();
        readonly Dictionary<string, FileCoverage> coverageByFile = new Dictionary<string, FileCoverage>();

        //---------------------------------------------------------------------
        public void TextViewCreated(IWpfTextView textView)
        {
            var optionalFilePath = GetOptionalFilePath(textView);

            if (optionalFilePath != null)
            {
                this.viewsByPath.Add(optionalFilePath, textView);
                textView.Closed += OnTextViewClosed;

                // Here IWpfTextView.TextViewLines is null and so
                // cannot call AddNewHighlightCoverage directly.
                textView.LayoutChanged += OnLayoutChanged;
            }
        }

        //---------------------------------------------------------------------
        // These lines declare new AdornmentLayer.
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(HighlightLinesAdornment)]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        [TextViewRole(PredefinedTextViewRoles.Document)]
        public AdornmentLayerDefinition EditorAdornmentLayer = null;

        //---------------------------------------------------------------------
        [Import]
        ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        //---------------------------------------------------------------------
        public CoverageRate CoverageRate
        {
            set
            {
                this.RemoveHighlightForAllViews();
                this.coverageByFile.Clear();

                var fileCoverageCollection = value.Children.SelectMany(module => module.Children);
                foreach (var fileCoverage in fileCoverageCollection)
                {
                    var normalizedPath = NormalizePath(fileCoverage.Path);
                    IWpfTextView view;
                    if (this.viewsByPath.TryGetValue(normalizedPath, out view))
                        AddNewHighlightCoverage(view, fileCoverage);
                    this.coverageByFile.Add(normalizedPath, fileCoverage);
                }
            }
        }

        //---------------------------------------------------------------------
        string GetOptionalFilePath(IWpfTextView textView)
        {
            ITextDocument textDocument;
            this.TextDocumentFactoryService.TryGetTextDocument(textView.TextBuffer, out textDocument);

            if (textDocument == null)
                return null;

            return NormalizePath(textDocument.FilePath);
        }

        //---------------------------------------------------------------------
        void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var textView = (IWpfTextView)sender;
            textView.LayoutChanged -= OnLayoutChanged;
            var optionalFilePath = GetOptionalFilePath(textView);

            if (optionalFilePath != null)
            {
                FileCoverage fileCoverage;
                if (this.coverageByFile.TryGetValue(optionalFilePath, out fileCoverage))
                    AddNewHighlightCoverage(textView, fileCoverage);
            }
        }

        //---------------------------------------------------------------------
        void OnTextViewClosed(object sender, EventArgs e)
        {
            var textView = sender as IWpfTextView;

            if (textView != null)
            {
                foreach (var kvp in this.viewsByPath)
                {
                    if (kvp.Value == textView)
                    {
                        this.viewsByPath.Remove(kvp.Key);
                        break;
                    }
                }
                textView.Closed -= OnTextViewClosed;
                textView.LayoutChanged -= OnLayoutChanged;
            }
        }

        //---------------------------------------------------------------------
        static string NormalizePath(string path)
        {
            return System.IO.Path.GetFullPath(path).ToLowerInvariant();
        }

        //---------------------------------------------------------------------
        void AddNewHighlightCoverage(IWpfTextView view, FileCoverage fileCoverage)
        {
            var textViewLines = view.TextViewLines.ToList();
            var coverage = fileCoverage.LineCoverages.ToDictionary(line => line.LineNumber);
            var adornmentLayer = view.GetAdornmentLayer(HighlightLinesAdornment);

            int lineNumber = 1;
            foreach (var textViewLine in textViewLines)
            {
                LineCoverage lineCoverage;

                if (coverage.TryGetValue(lineNumber, out lineCoverage))
                {
                    var color = lineCoverage.HasBeenExecuted ? Brushes.PaleGreen : Brushes.LightCoral;

                    AddAdornment(adornmentLayer, view, textViewLine, color);
                }
                ++lineNumber;
            }
        }

        //---------------------------------------------------------------------
        void RemoveHighlightForAllViews()
        {
            foreach (var kvp in this.viewsByPath)
            {
                var view = kvp.Value;
                var adornmentLayer = view.GetAdornmentLayer(HighlightLinesAdornment);
                adornmentLayer.RemoveAllAdornments();
            }
        }

        //---------------------------------------------------------------------
        static void AddAdornment(
            IAdornmentLayer adornmentLayer,
            IWpfTextView view, 
            ITextViewLine line,
            SolidColorBrush colorBrush)
        {
            var rect = new Rectangle()
            {
                Width = Math.Max(view.ViewportWidth, view.MaxTextRightCoordinate),
                Height = line.Height,
                Fill = colorBrush
            };

            Canvas.SetTop(rect, line.Top);
            Canvas.SetLeft(rect, 0);
            adornmentLayer.AddAdornment(line.Extent, null, rect);
        }      
    }
}
