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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage.CoverageRateBuilder;
using OpenCppCoverage.VSPackage.CoverageTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass]
    public class FileCoverageAggregatorTests
    {
        static readonly string file1 = "file1";
        static readonly string file2 = "file2";
        static readonly string file3 = "file3";

        //---------------------------------------------------------------------
        [TestMethod]
        public void AggregateFileCoveragesOnly()
        {
            var coverageRate = new CoverageRate(string.Empty, 0);
            coverageRate.AddChild(CreateModule(file1.ToUpper(), file2));
            coverageRate.AddChild(CreateModule(file2.ToUpper(), file3.ToUpper()));

            var aggregator = new FileCoverageAggregator();
            var fileCoverageDict = aggregator.Aggregate(coverageRate, str => str.ToLower());
            var fileCoverages = fileCoverageDict.Select(kvp => kvp.Key).ToList();

            CollectionAssert.AreEquivalent(
                new List<string> { file1, file2, file3 }, fileCoverages);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void AggregateLineCoverages()
        {
            var coverageRate = new CoverageRate(string.Empty, 0);
            coverageRate.AddChild(CreateModule(
                file1, 
                new LineCoverage(1, true),
                new LineCoverage(2, true),
                new LineCoverage(3, true),
                new LineCoverage(4, false)));
            coverageRate.AddChild(CreateModule(
                file1,
                new LineCoverage(2, true),
                new LineCoverage(3, false),
                new LineCoverage(4, false),
                new LineCoverage(5, false)));

            var aggregator = new FileCoverageAggregator();
            var coverageByFile = aggregator.Aggregate(coverageRate, str => str);
            var fileCoverage = coverageByFile.Single();

            var expectedLineCoverages = new List<LineCoverage> {
                new LineCoverage(1, true),
                new LineCoverage(2, true),
                new LineCoverage(3, true),
                new LineCoverage(4, false),
                new LineCoverage(5, false)};

            CollectionAssert.AreEqual(
                expectedLineCoverages,
                fileCoverage.Value.LineCoverages.ToList(),
                new LineCoverageComparer());
        }

        //---------------------------------------------------------------------
        class LineCoverageComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var lineCoverage1 = (LineCoverage)x;
                var lineCoverage2 = (LineCoverage)y;

                if (lineCoverage1.LineNumber != lineCoverage2.LineNumber)
                    return lineCoverage1.LineNumber - lineCoverage2.LineNumber;
                return Comparer.Default.Compare(
                    lineCoverage1.HasBeenExecuted, 
                    lineCoverage2.HasBeenExecuted);
            }
        }

        //---------------------------------------------------------------------
        static ModuleCoverage CreateModule(
            params string[] filenames)
        {
            var module = new ModuleCoverage(string.Empty);
            foreach (var filename in filenames)
                module.AddChild(new FileCoverage(filename, new List<LineCoverage>()));
            return module;
        }

        //---------------------------------------------------------------------
        static ModuleCoverage CreateModule(
            string filename,
            params LineCoverage[] lineCoverages)
        {
            var module = new ModuleCoverage(string.Empty);
            module.AddChild(new FileCoverage(filename, lineCoverages.ToList()));
            
            return module;
        }
    }
}