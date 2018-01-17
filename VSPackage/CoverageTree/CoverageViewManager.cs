﻿// OpenCppCoverage is an open source code coverage for C++.
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
using Microsoft.VisualStudio.PlatformUI;
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
    sealed class CoverageViewManager : IWpfTextViewCreationListener, ICoverageViewManager
    {
        //---------------------------------------------------------------------
        public const string HighlightLinesAdornment = "HighlightLines";
        public static object CoverageTag = new object();
        public static Brush CoveredBrush = Brushes.PaleGreen;
        public static Brush UncoveredBrush = Brushes.LightCoral;
        
        //---------------------------------------------------------------------
        readonly List<IWpfTextView> views = new List<IWpfTextView>();
        readonly FileCoverageAggregator fileCoverageAggregator = new FileCoverageAggregator();

        Dictionary<string, FileCoverage> coverageByFile = new Dictionary<string, FileCoverage>();
        Dictionary<IWpfTextView, EventHandler<TextContentChangedEventArgs>> 
            onTextChangedHanlders = new Dictionary<IWpfTextView, EventHandler<TextContentChangedEventArgs>>();
        bool showCoverage;

        //---------------------------------------------------------------------
        public void TextViewCreated(IWpfTextView textView)
        {
            this.views.Add(textView);
            textView.Closed += OnTextViewClosed;
            textView.LayoutChanged += OnLayoutChanged;
            
            EventHandler<TextContentChangedEventArgs> onTextChanged = 
                (sender, e) => OnTextChanged(textView, e);
            onTextChangedHanlders.Add(textView, onTextChanged);
            textView.TextBuffer.Changed += onTextChanged;

            VSColorTheme.ThemeChanged += OnColorThemeChanged;
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
                this.coverageByFile = fileCoverageAggregator.Aggregate(value, NormalizePath);
                this.RemoveHighlightForAllViews();
                AddHighlightCoverageForExistingViews();
            }
        }

        //---------------------------------------------------------------------
        public bool ShowCoverage
        {
            set
            {
                if (this.showCoverage != value)
                {
                    this.showCoverage = value;
                    this.RemoveHighlightForAllViews();
                    if (this.showCoverage)
                        AddHighlightCoverageForExistingViews();
                }
            }
        }

        //---------------------------------------------------------------------
        void AddHighlightCoverageForExistingViews()
        {
            foreach (var view in this.views)
                AddNewHighlightCoverage(view, view.TextViewLines);
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
            if (this.showCoverage)
                AddNewHighlightCoverage((IWpfTextView)sender, e.NewOrReformattedLines);
        }

        //---------------------------------------------------------------------
        void OnTextViewClosed(object sender, EventArgs e)
        {
            var textView = sender as IWpfTextView;

            if (textView != null)
            {
                this.views.Remove(textView);
                textView.Closed -= OnTextViewClosed;
                textView.LayoutChanged -= OnLayoutChanged;

                var onTextChangedHandler = this.onTextChangedHanlders[textView];
                this.onTextChangedHanlders.Remove(textView);

                textView.TextBuffer.Changed -= onTextChangedHandler;
            }
        }

        //---------------------------------------------------------------------
        static string NormalizePath(string path)
        {
            return System.IO.Path.GetFullPath(path).ToLowerInvariant();
        }

        //---------------------------------------------------------------------
        void AddNewHighlightCoverage(
            IWpfTextView textView,
            IEnumerable<ITextViewLine> textViewLines)
        {
            if (textViewLines.Any())
            {
                var optionalFilePath = GetOptionalFilePath(textView);
                FileCoverage fileCoverage;

                if (optionalFilePath != null && this.coverageByFile.TryGetValue(optionalFilePath, out fileCoverage))
                {
                    var coverage = fileCoverage.LineCoverages.ToDictionary(line => line.LineNumber);
                    var adornmentLayer = textView.GetAdornmentLayer(HighlightLinesAdornment);

                    foreach (var line in textViewLines)
                    {
                        LineCoverage lineCoverage;
                        int lineNumber = textView.TextSnapshot.GetLineNumberFromPosition(line.Extent.Start) + 1;

                        if (coverage.TryGetValue(lineNumber, out lineCoverage))
                        {
                            var color = lineCoverage.HasBeenExecuted ? VSColorTheme.GetThemedColor(VSColor.VSColors.CodeCoveredBrushKey)
                                : VSColorTheme.GetThemedColor(VSColor.VSColors.CodeNotCoveredBrushKey);
                            var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

                            AddAdornment(adornmentLayer, textView, line, brush);
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------
        void RemoveHighlightForAllViews()
        {
            foreach (var view in this.views)
                RemoveHighlight(view);
        }

        //---------------------------------------------------------------------
        void RemoveHighlight(IWpfTextView textView)
        {
            var adornmentLayer = textView.GetAdornmentLayer(HighlightLinesAdornment);
            adornmentLayer.RemoveAllAdornments();
        }

        //---------------------------------------------------------------------
        static void AddAdornment(
            IAdornmentLayer adornmentLayer,
            IWpfTextView view, 
            ITextViewLine line,
            Brush colorBrush)
        {
            var rect = new Rectangle()
            {
                Width = Math.Max(view.ViewportWidth, view.MaxTextRightCoordinate),
                Height = line.Height,
                Fill = colorBrush
            };

            Canvas.SetTop(rect, line.Top);
            Canvas.SetLeft(rect, 0);
            adornmentLayer.AddAdornment(line.Extent, CoverageTag, rect);
        }

        //---------------------------------------------------------------------
        void OnTextChanged(IWpfTextView textView, TextContentChangedEventArgs e)
        {
            var lineChanged = e.Changes.Sum(c => c.LineCountDelta);
            if (lineChanged != 0)
            {
                var optionalFilePath = GetOptionalFilePath(textView);
                if (this.coverageByFile.Remove(optionalFilePath))
                    RemoveHighlight(textView);
            }
        }

        void OnColorThemeChanged(ThemeChangedEventArgs e)
        {
            this.RemoveHighlightForAllViews();
            if (this.showCoverage)
                this.AddHighlightCoverageForExistingViews();
        }
    }
}
