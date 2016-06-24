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

using OpenCppCoverage.VSPackage.CoverageData;
using System.Linq;

using ProtoBuff = global::OpenCppCoverage.VSPackage.CoverageData.ProtoBuff;

namespace OpenCppCoverage.VSPackage.CoverageRateBuilder
{
    //--------------------------------------------------------------------------
    class CoverageRateBuilder
    {
        //---------------------------------------------------------------------
        public CoverageRate Build(CoverageResult result)
        {
            var protoBuffCoverageData = result.CoverageData;
            var coverageRate = new CoverageRate(
                                    protoBuffCoverageData.Name, 
                                    protoBuffCoverageData.ExitCode);
            foreach (var protoModule in result.Modules)
            {
                var module = new ModuleCoverage(protoModule.Path);
                foreach (var protoFile in protoModule.FilesList)
                    module.AddChild(BuildFileCoverage(protoFile));
                coverageRate.AddChild(module);
            }

            return coverageRate;
        }

        //---------------------------------------------------------------------
        FileCoverage BuildFileCoverage(ProtoBuff.FileCoverage protoFile)
        {
            var lines = protoFile.LinesList;

            return new FileCoverage(
                protoFile.Path,
                lines.Select(l => new LineCoverage(
                    (int)l.LineNumber, l.HasBeenExecuted)).ToList());
        }
    }
}
